using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MLAPI;

[RequireComponent(typeof(CharacterController))]
public class FirstPerson : NetworkBehaviour
{
	public bool lockCursor;

	const float gravity = 20;
	const float jumpForce = 8;
	const float sneakSpeed = 2;
	const float walkSpeed = 4;
	const float runSpeed = 8;
	float moveSpeed = walkSpeed;
	[System.NonSerialized]
	public bool sneaking, running, jumping = false;

	public float fov;
	public Vector2 mouseSensitivity = new Vector2(1, 1);
	public Transform camTrans;
	public Camera cam;
	public PostProcessVolume ppv;
	public MeshRenderer playerMesh;
	CharacterController controller;
	float pitch;

	float velocityY;
	Vector3 jumpDir, moveDir;

	Vignette vignette;

	void Start()
	{
		cam = camTrans.GetComponent<Camera>();

		if (!IsLocalPlayer)
		{
			camTrans.GetComponent<AudioListener>().enabled = false;
			cam.enabled = false;
			ppv.enabled = false;
		}
		else
		{
			playerMesh.enabled = false;
			vignette = ppv.profile.GetSetting<Vignette>();
			controller = GetComponent<CharacterController>();
			if (lockCursor)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}


	void Update()
	{
		if (IsLocalPlayer)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (PauseMenu.paused == false)
				{
					PauseMenu.Pause();
				}
				else
				{
					PauseMenu.Resume();
				}
			}

			if (PauseMenu.paused == false)
			{			
				moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
				moveCamera();
				getInputs();
			}
			movePlayer();
		}
	}


	public void moveCamera()
	{
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity.x);
		pitch += mouseInput.y * mouseSensitivity.y;
		pitch = ClampAngle(pitch, -90, 90);
		Quaternion yQuaternion = Quaternion.AngleAxis(pitch, Vector3.left);
		cam.transform.localRotation = yQuaternion;
	}

	static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
		return Mathf.Clamp(angle, min, max);
	}

	public void getInputs()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (!sneaking)
			{
				running = true;
				StartCoroutine(runTransition());
			}
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			running = false;
			StartCoroutine(runTransition());
		}
		if (Input.GetKey(KeyCode.LeftControl) && !jumping)
		{
			sneaking = true;
			running = false;
			StartCoroutine(sneakTransition());
		}
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			sneaking = false;
			StartCoroutine(sneakTransition());
		}

		if (Input.GetKey(KeyCode.S)) moveSpeed = sneakSpeed;
		if (Input.GetKeyUp(KeyCode.S)) moveSpeed = walkSpeed;

		if (controller.isGrounded)
		{
			jumping = false;
			velocityY = -gravity;
			if (Input.GetKeyDown(KeyCode.Space) && !sneaking)
			{
				jumping = true;
				velocityY = jumpForce;
			}
		}
		else
		{
			moveDir = jumpDir;
		}
	}

	public void movePlayer()
	{
		velocityY -= gravity * Time.deltaTime;
		Vector3 velocity = transform.TransformDirection(moveDir) * moveSpeed + Vector3.up * velocityY;
		controller.Move(velocity * Time.deltaTime);
		jumpDir = moveDir;
	}

	IEnumerator sneakTransition()
	{
		float timeSinceStarted = 0f;
		float maxVignetteIntensity = 0.33f;
		Vector3 crouching = new Vector3(0, 0, 0);
		Vector3 standing = new Vector3(0, 0.5f, 0);
		Vector3 newPosition = sneaking ? crouching : standing;

		if (sneaking)
		{
			moveSpeed = sneakSpeed;
			while (sneaking)
			{
				timeSinceStarted += Time.deltaTime / 50;
				cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPosition, timeSinceStarted);

				if (vignette.intensity < maxVignetteIntensity)
				{
					vignette.intensity.value += 0.0001f;
				}

				if (sneaking && cam.transform.localPosition == crouching)
				{
					yield break;
				}

				yield return null;
			}
		}
		else
		{
			moveSpeed = walkSpeed;
			while (!sneaking)
			{
				timeSinceStarted += Time.deltaTime / 50;
				cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPosition, timeSinceStarted);

				if (vignette.intensity > 0)
				{
					vignette.intensity.value -= 0.001f;
				}

				if (!sneaking && cam.transform.localPosition == standing)
				{
					yield break;
				}

				yield return null;
			}
		}
	}

	IEnumerator runTransition()
	{
		float maxFovMultiplier = 1.2f;

		if (running)
		{
			moveSpeed = runSpeed;
			while (running)
			{
				if (cam.fieldOfView < maxFovMultiplier * fov)
				{
					cam.fieldOfView += 0.01f;
				}

				yield return null;
			}
		}
		else
		{
			moveSpeed = walkSpeed;
			while (!running)
			{
				if (cam.fieldOfView > fov)
				{
					cam.fieldOfView -= 0.1f;
				}

				yield return null;
			}
		}
	}
}