using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static int powerCount = 0; // Amount of power-ups on screen
	
	public GameObject[] planes; // All planes in the game
	public Transform cameraRig; // The camera rig to rotate and position
	
	void Awake(){
		
		InvokeRepeating("PlacePower", 2.0f, 1.0f); // Place power-up every second after 2 seconds delay
		
	}
	
	void Update(){
		
		// Find the center of all planes
		
		Vector3 center = Vector3.zero;
		
		for(int i = 0; i < planes.Length; i++){ center += planes[i].transform.position; }
		
		center /= planes.Length;
		
		// Then rotate the camera accordingly
		
		float cameraRotation = 60 - center.z;
		cameraRotation = Mathf.Clamp(cameraRotation, 20.0f, 60.0f); // Make sure camera stays between two values
		
		cameraRig.eulerAngles = Vector3.Lerp(cameraRig.eulerAngles, new Vector3(cameraRotation, 0, 0), Time.deltaTime * 2.0f);
		
	}
	
	public void PlacePower(){
		
		// If there's less than 2 power-ups on the screen
		
		if(powerCount < 2){
			
			// Create a new power-up and keep count
		
			Instantiate(Resources.Load("Prefabs/powerup"), new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-10.0f, 10.0f)), Quaternion.identity);
		
			powerCount++;
			
		}
		
	}
	
}
