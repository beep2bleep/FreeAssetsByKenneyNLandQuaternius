using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	private float speed = 10.0f; // Travel speed of bomb
	public Transform explosion;  // Transform that holds explosion particles and collider
	
	void Awake(){
		
		explosion.gameObject.SetActive(false);
		
		// Timer (explode after 2 seconds, destroy after 4 seconds)
		
		Destroy(gameObject, 3.0f);
		Invoke("Explode", 2.0f);
		
	}

	void Update(){
		
		// Move the bomb forward according to 'speed' variable, divide speed to slow down over time
		
		transform.Translate(-Vector3.forward * speed * Time.deltaTime);
		transform.Rotate(-Vector3.forward * 20.0f * Time.deltaTime);
		
		speed *= 0.95f;
		
	}
	
	void Explode(){
		
		transform.GetChild(0).gameObject.SetActive(false); // Disable bomb
		explosion.gameObject.SetActive(true); // Enable explosion
		
	}
	
}
