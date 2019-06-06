using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	public int player = 1;		 // Who shot the bullet?
	private float speed = 75.0f; // Travel speed of bullet

	void Update(){
		
		// Move the bullet forward according to 'speed' variable
		
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		
	}
	
}