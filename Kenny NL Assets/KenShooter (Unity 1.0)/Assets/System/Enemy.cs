using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Enemy : MonoBehaviour{
	
	// Grave to spawn once dead
	
	public GameObject grave;
	
	// Private
	
    private int health = 50; // Health points
	private bool detected = false; // Enemy has seen player
	
	private float speed = 1.0f; // Walking speed, gets randomized
	private float cooldown; // Cooldown for hitting player

	private Transform player;
	
	void Start(){
		
		player = GameObject.Find("player").transform;
		
		speed = Random.Range(0.75f, 2.0f); // Random walking speed
		
	}
	
	void Update(){
		
		// Can enemy see player?
		
		RaycastHit hit;
		
		Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
		Vector3 target = player.position + new Vector3(0, 0.5f, 0);
		
		if(Physics.Raycast(origin, (target - origin), out hit, 8.0f)){
			
			if(hit.transform == player){
				
				// Is the enemy close to the player?
				
				if(Vector3.Distance(transform.position, player.position) > 1.0f){
				
					// Walk towards the player
				
					transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
					
					if(!detected){
						
						// Play sound the first time enemy sees player
						
						Audio.Play("detect");
						
						detected = true;
						
					}
					
				}else{
					
					// Hit the player!
					
					if(cooldown <= Time.time){
						
						player.GetComponent<Player>().Hurt();
					
						cooldown = Time.time + 1.0f; // Hit rate
						
					}
					
				}
				
			}
		
		}
		
	}
	
	// When hit by bullet
	
	public void Hit(){
		
		health -= 10; // Hit health
		
		if(health <= 0){ Dead(); }
		
	}
	
	// When health is 0 or below
	
	void Dead(){
		
		Audio.Play("death");
		
		GameObject g = Instantiate(grave, transform.position, Quaternion.identity) as GameObject;
		
		Destroy(gameObject);
		
	}
	
}