using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Control:MonoBehaviour{
	
	bool gameLocked = true;
	
	[Header("Movement")]
	
	public float mouseSensitivity = 0.5f;
	public float walkSpeed = 2.0f;
	
	[Header("Controllers")]
	
	public Camera cameraPlayer;
	
	// Private
	
	CharacterController controller;
	
	// Parameters
	
	Vector2 mouseDelta;
	Vector2 mouseMomentum;
	Vector2 moveMomentum;
	
	// Gravity

	float gravity	 = 0.0f;
	float gravityMax = 20.0f;
	float gravityPrevious = 0.0f;

	bool moving = false;
	
	// Functions

	public void Start(){

		controller = GetComponent<CharacterController>();
		
		InvokeRepeating("Step", 0.0f, 0.5f);
		
	}
	
	public void Step(){
		
		if(moveMomentum.magnitude > 1.0f){ Audio.Play("step"); }
		
	}
	
	public void Update(){
		
		// Mouse lock
	
		if(gameLocked){
			
			mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X") * (mouseSensitivity * 80), Input.GetAxisRaw("Mouse Y") * (mouseSensitivity * 80));
			Cursor.lockState = CursorLockMode.Locked;
			
		}else{
			
			mouseDelta = new Vector2(0, 0);
			Cursor.lockState = CursorLockMode.None;
			
		}
		
		// Unlock
		
		if(Input.GetKeyDown(KeyCode.Escape)){ gameLocked = false; }
		
		// Lock
		
		if(Input.GetMouseButtonDown(0)){ gameLocked = true; }
		
		// Movement
		
		Vector2 movementDirection = Vector2.zero;

		if(gameLocked){
		
			if(Input.GetKey("w")){ movementDirection.x =  walkSpeed; }
			if(Input.GetKey("s")){ movementDirection.x = -walkSpeed; }
			
			if(Input.GetKey("a")){ movementDirection.y = -walkSpeed; }
			if(Input.GetKey("d")){ movementDirection.y =  walkSpeed; }
			
		}

		moveMomentum = Vector2.Lerp(moveMomentum, movementDirection, Time.deltaTime * 8.0f);
		
		Vector3 moveDirection = transform.TransformDirection(moveMomentum.x * Vector3.forward) + transform.TransformDirection(moveMomentum.y * Vector3.right);
		
		// Gravity

		float gravityPrevious = gravity;

		if(!controller.isGrounded){

			gravity += 0.3f;

		}else{

			if(gravity > -10.0f){ gravity = 1.0f; }

		}
		
		gravity = Mathf.Clamp(gravity, -100.0f, gravityMax);
		
		moveDirection.y = -gravity;
		
		// Apply movement
		
		controller.Move(moveDirection * Time.deltaTime);
		
		// Mouse movement and smoothing
		
		mouseMomentum = Vector2.Lerp(mouseMomentum, new Vector2(mouseDelta.x, mouseDelta.y), Time.deltaTime * 16.0f);
		
		Quaternion cameraPlayer_rotation = cameraPlayer.transform.localRotation;
		
		cameraPlayer_rotation.x -= (mouseMomentum.y * Time.deltaTime) / 100.0f;
		cameraPlayer_rotation.x = Mathf.Clamp(cameraPlayer_rotation.x, -0.7f, 0.7f);
		cameraPlayer_rotation.z = Mathf.Lerp(cameraPlayer_rotation.z, 0.0f, Time.deltaTime * 10.0f);
		
		cameraPlayer.transform.localRotation = cameraPlayer_rotation;
		transform.eulerAngles += new Vector3(0.0f, mouseMomentum.x * Time.deltaTime, 0.0f);
		
	}
	
}
