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
	const float jumpForce = 5;
	const float sneakSpeed = 2;
	const float walkSpeed = 4;
	const float runSpeed = 8;
	const float transitionSpeed = 3f;
	float moveSpeed = walkSpeed;
	[System.NonSerialized]
	public bool sneaking, running, jumping, looking, moving, camDisabled, bobDisabled = false;

	public float walkingBobbingSpeed = 20f;
	public float bobbingAmount = 0.1f;
	float timer = 0;

	public float fov;
	public Vector2 mouseSensitivity = new Vector2(1, 1);
	public Transform camTrans;
	public Camera cam;
	public PostProcessVolume ppv;
	public MeshRenderer playerMesh;
	CharacterController controller;
	float pitch;
	Quaternion camRotation;

	float velocityY;
	Vector3 jumpDir, moveDir;

	Vignette vignette;

	void Start()
	{
		cam = camTrans.GetComponent<Camera>();

		if (IsLocalPlayer)//!
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
		//if (IsLocalPlayer)
		//{
			if (Mathf.Abs(moveDir.x) > 0.1f || Mathf.Abs(moveDir.z) > 0.1f) moving = true;
			else moving = false;

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
				if (!camDisabled) MoveCamera();
				GetInputs();
				Look();
				Sneak();
				Run();
				Jump();
				MovePlayer();
			}
			else if (jumping)
			{
				Jump();
				MovePlayer();
			}

			if (!bobDisabled) HeadBob();
		//}
	}

	public void MoveCamera()
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

	public void GetInputs()
	{
		// play can only sprint forwards, cannot jump while sneaking or vice versa,  walks at sneak speed backwards.
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
		{
			if (!sneaking)
			{
				running = true;
			}
		}
		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
		{
			running = false;
		}
		if (Input.GetKey(KeyCode.LeftControl) && !jumping)
		{
			sneaking = true;
			running = false;
		}
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			sneaking = false;
		}

		if (Input.GetKey(KeyCode.S)) moveSpeed = sneakSpeed;
		if (Input.GetKeyUp(KeyCode.S)) moveSpeed = walkSpeed;

		if (Input.GetKey(KeyCode.LeftAlt))
		{
			looking = true;
		}
		if (Input.GetKeyUp(KeyCode.LeftAlt))
		{
			looking = false;
		}

	}

	public void MovePlayer()
	{
		velocityY -= gravity * Time.deltaTime;
		Vector3 velocity = transform.TransformDirection(moveDir) * moveSpeed + Vector3.up * velocityY;
		controller.Move(velocity * Time.deltaTime);
		jumpDir = moveDir;
	}

	public void Jump()
	{
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

	public void HeadBob()
	{
		float camPosY = sneaking ? 0 : 0.5f;
		float speedMod;
		if (sneaking) speedMod = 0.5f;
		else if (running) speedMod = 2;
		else speedMod = 1;

		if (moving && !jumping)
		{
			timer += Time.deltaTime * walkingBobbingSpeed;
			cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, camPosY + Mathf.Sin(timer) * bobbingAmount * speedMod, cam.transform.localPosition.z);
		}
		else
		{
			timer = 0;
			cam.transform.localPosition = new Vector3(Mathf.Lerp(cam.transform.localPosition.x, 0, Time.deltaTime * walkingBobbingSpeed), Mathf.Lerp(cam.transform.localPosition.y, camPosY, Time.deltaTime * walkingBobbingSpeed), cam.transform.localPosition.z);
		}
	}
	public void Look()
	{
		Quaternion lookRotation = Quaternion.Euler(0, 120, 0);
		if (cam.transform.localRotation.y < 0.005)
		{
			camRotation = Quaternion.Euler(cam.transform.localRotation.x * 100, 0, 0);
			camDisabled = false;
		}
		
		if (looking)
		{
			camDisabled = true;
			cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, lookRotation, Time.deltaTime * transitionSpeed * 2);
		}
		else
		{
			cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, camRotation, Time.deltaTime * transitionSpeed * 2);
		}
	}

	public void Sneak()
	{
		float maxVignetteIntensity = 0.2f;

		if (sneaking)
		{
			if (cam.transform.localPosition.y > 0.4) cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, 0.4f, cam.transform.localPosition.z);
			moveSpeed = sneakSpeed;
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime * transitionSpeed);
			if (vignette.intensity < maxVignetteIntensity)
			{
				vignette.intensity.value += 0.001f;
			}
		}
		else
		{
			if (cam.transform.localPosition.y < 0.1) cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, 0.1f, cam.transform.localPosition.z);
			moveSpeed = running ? runSpeed : walkSpeed;
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0.5f, 0), Time.deltaTime * transitionSpeed);
			if (vignette.intensity > 0)
			{
				vignette.intensity.value -= 0.001f;
			}
		}

		if (cam.transform.localPosition.y < 0.1 || cam.transform.localPosition.y > 0.4)
		{
			bobDisabled = false;
		}
		else
		{
			bobDisabled = true;
		}
	}

	public void Run()
	{
		float maxFovMultiplier = 1.1f;
		if (running)
		{
			moveSpeed = runSpeed;
			if (cam.fieldOfView < maxFovMultiplier * fov)
			{
				cam.fieldOfView += 0.1f;
			}
		}
		else
		{
			moveSpeed = sneaking ? sneakSpeed : walkSpeed;
			if (cam.fieldOfView > fov)
			{
				cam.fieldOfView -= 0.1f;
			}
		}
	}
}