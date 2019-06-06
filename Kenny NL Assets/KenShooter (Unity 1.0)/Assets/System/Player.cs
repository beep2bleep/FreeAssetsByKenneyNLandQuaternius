using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Player:MonoBehaviour{
	
	public GameObject bullet;
	
	// Private
	
	private int health = 100; // Health points
	private bool alive = true; // Is player alive
	
	private int gems = 0;
	private bool key = false;
	
	private float cooldown; // Cooldown for shooting
	
	// Display values on screen
	
	private Text gemsDisplay;
	private Text healthDisplay;
	
	void Awake(){
		
		// Get display text for health and gems
		
		gemsDisplay   = GameObject.Find("gems").GetComponent<Text>();
		healthDisplay = GameObject.Find("health").GetComponent<Text>();
		
	}
	
	void Update(){
		
		// Shoot raycast
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.SphereCast(ray, 0.05f, out hit, 1.0f)){
			
			// When collided
			
			if(hit.transform.GetComponent<Door>()){
				
				// If door, open door when pressing spacebar
				
				if(Input.GetKeyDown("space")){
					
					hit.transform.GetComponent<Door>().Open(key);
					
					if(hit.transform.GetComponent<Door>().ending){
						
						DisplayMessage("level complete");
						
					}
					
				}
				
			}
			
		}
		
		// Shoot bullet
		
		if(alive && cooldown <= Time.time && Input.GetMouseButton(0)){
			
			GameObject b = Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), transform.rotation) as GameObject;
			
			b.transform.rotation = Quaternion.Euler(transform.GetChild(0).eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			
			Destroy(b, 1.0f);
			
			Audio.Play("shoot");
			
			cooldown = Time.time + 0.25f; // Fire rate
			
		}
		
	}
	
	// When hit by enemy
	
	public void Hurt(){

		if(alive){
			
			Audio.Play("hurt");
			
			health -= 25; // Hit health
			
			healthDisplay.text = health.ToString();
			
			// Game over :(
			
			if(health < 0){
				
				healthDisplay.text = "0";
				
				DisplayMessage("game over");
				
			}
			
		}
		
	}
	
	// Display message like "level complete" or "game over", stops the game
	
	public void DisplayMessage(string t){
		
		alive = false; GetComponent<Control>().enabled = false;
		
		GameObject.Find("crosshair").SetActive(false);
		GameObject.Find("message").GetComponent<Text>().text = t;
		
	}
	
	// When close to items or objects
	
	private void OnTriggerEnter(Collider other){
		
		if(other.transform.name == "gem"){
			
			// Collect gem
			
			gems++;
			
			gemsDisplay.text = gems.ToString();
			
			Audio.Play("gem");
			
			Destroy(other.gameObject);
			
		}
		
		if(other.transform.name == "key"){
			
			// Collect key
			
			key = true;
			
			Audio.Play("key");
			
			Destroy(other.gameObject);
			
		}
		
    }
    
}