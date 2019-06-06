using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
	
	public int type = 1; // The power-up type (1: GUN, 2: MISSILE, 3: BOMB, 4: HEALTH)
	
	public Sprite[] sprites;
	
	void Awake(){
		
		type = Random.Range(1, 5); // Set random type on wake
		
		transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[type - 1]; // Set sprite
		
	}
	
	public void Remove(){
		
		// Function to remove the power-up (on shoot, or pick-up by player)
		
		Game.powerCount--; // Subtract amount of power-ups currently in the world
		Destroy(gameObject); // Destroy itself
		
	}
	
	void OnTriggerEnter(Collider other){
		
		// When power-up gets hit by other collider
		
		if(other.transform.GetComponent<Bullet>()){
			
			// If the other collider is a bullet...
			
			Destroy(other.transform.gameObject); // Remove the bullet
			Remove(); // Remove itself
			
		}
		
	}
	
}