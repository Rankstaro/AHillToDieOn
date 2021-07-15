using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float viewRadius = 15;
    float soundRadius;
    [Range(0,360)]
    public float viewAngle = 90;
    public float moveSpeed = 1;
    const float turnSpeed = 4;

    public LayerMask playerMask;
    public LayerMask obstacleMask;
    Vector3 velocity, dirToTarget;
    bool patrolling, chasing = false;

    public List<Transform> visibleTargets = new List<Transform>();

    // Detection, pathfinding, breaking boards, reaching through boards, attacking player.

    void Start()
    {
        soundRadius = viewRadius / 2;
        // look for targets incrementally
        StartCoroutine(FindTargetsWithDelay(.2f));
    }

    void Update()
    {
        // If there are targets within detection
        if (visibleTargets.Count != 0)
        {
            // stop patrolling or chasing, target closest player.
            patrolling = false;
            chasing = false;
            Transform target = visibleTargets[FindClosestTargetIndex()];
            dirToTarget = (target.position - transform.position).normalized;
            dirToTarget.y = 0;
            float distance = Vector3.Distance(transform.position, target.position);
            float fatness = gameObject.GetComponent<CapsuleCollider>().radius + 1;

            // if the player is too close, stop moving
            if (distance < fatness)
            {
                velocity = Vector3.zero;
            }
            else
            {
                velocity = (dirToTarget * moveSpeed) * Time.deltaTime;
            }
        }
        // Once you lose track of player, start chasing.  
        else if (!chasing) 
        {
            StartCoroutine(Chase());
            velocity = (dirToTarget * moveSpeed) * Time.deltaTime;
        }
        // While patrolling
        else
        {
            velocity = (dirToTarget * moveSpeed) * Time.deltaTime;
        }
        // If you have a target, look towards it
        if (dirToTarget != Vector3.zero)
        {
            transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToTarget), Time.deltaTime * turnSpeed);
        }

        transform.localPosition += velocity;
    }

    // Continues in the direction where it last detected player for 10 seconds.
    IEnumerator Chase()
    {
        chasing = true;
        yield return new WaitForSeconds(10);
        StartCoroutine(Patrol());
    }

    // If you haven't detected the player in the last 10 seconds, move around aimlessly.
    IEnumerator Patrol()
    {
        patrolling = true;
        while(patrolling)
        {
            dirToTarget = Vector3.zero;
            yield return new WaitForSeconds(Random.Range(0, 10));
            dirToTarget = (new Vector3((float)Random.Range(-10, 11) / 10, 0, (float)Random.Range(-10, 11) / 10));
            yield return new WaitForSeconds(Random.Range(10, 30));
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    // Returns the index of the closest player in visible targets list 
    public int FindClosestTargetIndex()
    {
        int minDistIdx = 0;
        float minDist, crntDist;
        for (int i = 1; i < visibleTargets.Count; i++)
        {
            minDist = Vector3.Distance(transform.position, visibleTargets[minDistIdx].position);
            crntDist = Vector3.Distance(transform.position, visibleTargets[i].position);
            if (crntDist < minDist)
            {
                minDistIdx = i;
            }
        }
        return minDistIdx;
    }

    // populates visibleTargets list with player colliders if they meet detection conditions
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distToTarget = Vector3.Distance(transform.position, target.position);
            bool sneaking = target.GetComponentInParent<FirstPerson>().sneaking;
            bool running = target.GetComponentInParent<FirstPerson>().running;

            // if the target is running within view radius it is detected
            if (running)
            {
                visibleTargets.Add(target);
            }
            // if the target is walking within soundRadius it is detected
            else if (!sneaking && !running && distToTarget <= soundRadius)
            {
                visibleTargets.Add(target);
            }
            // if the player is within your field of vision and not obstructed by obstacles, it is detected
            else if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}