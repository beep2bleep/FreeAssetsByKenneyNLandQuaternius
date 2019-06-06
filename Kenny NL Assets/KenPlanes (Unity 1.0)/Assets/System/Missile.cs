using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	public int player = 1;	 // Who shot the missile?
	public Transform target; // Target of missile to fly to
	private float speed;	 // Travel speed of missile
	
	void Start(){
		
		// Random speed for diversity
		
		speed = Random.Range(15.0f, 35.0f);
		
		// Pick target
		
		List<Plane> planes = new List<Plane>();
		planes.AddRange(FindObjectsOfType<Plane>()); // Get all planes
		
		foreach(Plane plane in planes){ if(plane.player == player){ planes.Remove(plane); break; } }; // Remove owner
		
		target = planes[Random.Range(0, planes.Count)].transform;
		
		// Timer (explode after 2 seconds)
		
		Invoke("Explode", 2.0f);
		
	}
	
	void Update(){
		
		// Move the missile forward according to 'speed' variable
		
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		
		Vector3 direction = (target.position - transform.position).normalized;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10.0f);
		
	}
	
	public void Explode(){
		
		speed = 0.0f;
		
		transform.GetChild(0).gameObject.SetActive(false); // Disable missile
		
		// Explosion
		
		GameObject objectExplosion = Instantiate(Resources.Load("Prefabs/explosion"), transform.position, transform.rotation) as GameObject; // Create explosion

		Destroy(objectExplosion, 2.0f);
		
		Destroy(gameObject); // Destroy itself
		
	}
	
}
