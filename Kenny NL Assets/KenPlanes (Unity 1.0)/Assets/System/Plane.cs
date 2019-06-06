using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Plane : MonoBehaviour {
	
	[Header("Settings")]
	
	public int player = 1;				// Player number (for controls)
	public Color color;					// Player color
	
	public float flyingSpeed  = 10.0f;	// Maximum speed of plane
	public float turningSpeed = 150.0f;	// Turn speed of plane
	
	public float fireRate  = 0.25f;		// Rate of firing (seconds)
	public float boostRate = 2.0f;		// Rate of speed boost (seconds)
	
	[Header("Controls")]				// Controls
	
	public KeyCode keyLeft;
	public KeyCode keyRight;
	public KeyCode keyBoost;
	public KeyCode keyFire;
	
	[Header("Components")]
	
	public Transform smokeLight;		// Light smoke (particle effect)
	public Transform smokeHeavy;		// Heavy smoke (particle effect)
	
	[Header("Interface")]
	
	public Transform stats;				// Stats component in interface
	
	private Text  statsAmmo;
	private Image statsHealth;
	
	// Private (employees only)
	
	private float health = 100;			// Current health of plane
	private float speed = 0.0f;			// Current speed of plane
	
	private int powerup		= 0;		// Current power-up
	private int powerupAmmo = 0;		// Current power-up ammo
	
	private float targetSpeed;			// Target speed of plane
	private float targetTilt;			// Target tilt of plane
	
	private float nextFire;				// Cooldown for rate of fire
	private float nextBoost;			// Cooldown for rate of boost
	
	private Vector3 startPosition;		// Start position, to reset to on respawn
	private Transform planeModel;		// The plane model holder to tilt on rotation
	
	// Functions
	
	void Awake(){
		
		startPosition = transform.position; // Set start position
		
		statsAmmo   = stats.GetChild(0).GetComponent<Text>(); // Set text to edit in interface
		statsHealth = stats.GetChild(1).GetChild(0).GetComponent<Image>(); // Set image to edit in interface
		
		planeModel  = transform.GetChild(0); // Set plane model holder to tilt
		
		targetSpeed = flyingSpeed; // Set target speed to the maximum flying speed
		
		SetColor(); // Set plane color
		
	}
	
	void Update(){
		
		targetTilt = 0.0f;
		
		// Controls (perform actions on key presses if plane is active)
		
		if(health > 0){
			
			if(Input.GetKey(keyLeft)){  transform.Rotate(Vector3.down * turningSpeed * Time.deltaTime); targetTilt =  45.0f; }
			if(Input.GetKey(keyRight)){ transform.Rotate(Vector3.up   * turningSpeed * Time.deltaTime); targetTilt = -45.0f; }
		
			if(Input.GetKey(keyBoost) && Time.time > nextBoost)  { Boost(); }
			if(Input.GetKeyDown(keyFire) && Time.time > nextFire){ Fire();  }
			
		}
		
		// Movement (apply movement)
		
		speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 4.0f); // Smoothly change plane speed
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		
		// Tilt (smoothly set rotation of Z axis for the plane model holder based on the target tilt)
		
		planeModel.localRotation = Quaternion.Lerp(planeModel.localRotation, Quaternion.Euler(planeModel.localRotation.x, planeModel.localRotation.y, targetTilt), Time.deltaTime * 10.0f);
		
		// Interface (show properties on screen)
		
		stats.position = Camera.main.WorldToScreenPoint(transform.position); // Set position of stats UI element to the player in world position
		
		if(powerupAmmo > 0){ statsAmmo.text = powerupAmmo.ToString(); }else{ statsAmmo.text = ""; } // Only if the player has ammo, show ammo count
		
		statsHealth.fillAmount = Mathf.Lerp(statsHealth.fillAmount, health / 100, Time.deltaTime * 10.0f);
		
		// Health effects
		
		if(health >= 75){ smokeLight.gameObject.SetActive(false); smokeHeavy.gameObject.SetActive(false); }
		if(health <  75){ smokeLight.gameObject.SetActive(true); }
		if(health <  25){ smokeLight.gameObject.SetActive(false); smokeHeavy.gameObject.SetActive(true); }
		
	}
	
	public void Boost(){
		
		// Temporary speed boost
		
		speed = flyingSpeed * 6;
		
		nextBoost = Time.time + boostRate; // Set cooldown period
		
	}
	
	public void Fire(){
		
		// Fire a bullet
		
		if(powerupAmmo > 0){
			
			// Only if the user has ammo
			
			switch(powerup){
				
				case 1: // GUN
				
					GameObject objectBullet = Instantiate(Resources.Load("Prefabs/bullet"), transform.position, transform.rotation) as GameObject; // Create bullet
					objectBullet.GetComponent<Bullet>().player = player; // Set the bullet owner to the current player
				
					Destroy(objectBullet, 1.0f); // Remove the bullet after time period
				
					break;
					
				case 2: // MISSILE
				
					GameObject objectMissile = Instantiate(Resources.Load("Prefabs/missile"), transform.position, transform.rotation) as GameObject; // Create missile
					objectMissile.GetComponent<Missile>().player = player; // Set the missile owner to the current player
				
					break;
					
				case 3: // BOMB
				
					Instantiate(Resources.Load("Prefabs/bomb"), transform.position, transform.rotation); // Create bomb
				
					break;
					
			}
		
			powerupAmmo--;
			
			nextFire = Time.time + fireRate; // Set cooldown period
			
		}
		
	}
	
	public void Respawn(){
		
		// Reset the plane to original position and properties
		
		health		= 100;
		
		powerup		= 0;
		powerupAmmo = 0;
		
		speed		= 0.0f;
		targetSpeed = flyingSpeed;
		
		planeModel.gameObject.SetActive(true);
		
		transform.position = startPosition; // Set position to start
		
	}
	
	public void Hit(Vector3 hitPosition, int points = 10){
		
		if(health > 0){
		
			// Get hit by bullet
			
			health -= points;
			
			if(health <= 0){
				
				GameObject objectExplosion = Instantiate(Resources.Load("Prefabs/explosion"), transform.position, transform.rotation) as GameObject; // Create explosion
				
				Destroy(objectExplosion, 2.0f);
				
				targetSpeed = 0.0f;
				planeModel.gameObject.SetActive(false);
				
				Invoke("Respawn", 4.0f); // Respawn after 4 seconds
				
			}
			
			// Particle effect
			
			GameObject particle = Instantiate(Resources.Load("Prefabs/hit"), hitPosition, transform.rotation) as GameObject;
			
			Destroy(particle, 1.0f);
			
		}
		
	}
	
	public void SetColor(){
		
		statsHealth.color = color;
		
		Color darkerColor = new Color(color.r * 0.85f, color.g * 0.85f, color.b * 0.85f);
		
		// Select all renderers of plane
		
		foreach(Renderer renderer in transform.GetComponentsInChildren<Renderer>()){
			
			// Select all materials
			
			foreach(Material m in renderer.materials){
				
				if(m.name.Contains("plane")){ m.SetColor("_Color", color); }
				if(m.name.Contains("planeDark")){ m.SetColor("_Color", darkerColor); }
				
			}
			
		}
		
	}
	
	void OnTriggerEnter(Collider other){
		
		// On collision with a power-up
		
		if(other.transform.GetComponent<Powerup>()){
			
			int powerupType = other.transform.GetComponent<Powerup>().type; // Get power-up type
			
			if(powerupType != 4){ powerup = powerupType; } // If type isn't health, set power-up
			
			if(powerupType == 1){ powerupAmmo = 10; } // If type is 1 (GUN), set ammo to 10
			if(powerupType == 2){ powerupAmmo = 3;  } // If type is 2 (MISSILE), set ammo to 3
			if(powerupType == 3){ powerupAmmo = 1;  } // If type is 3 (BOMB), set ammo to 1
			if(powerupType == 4){ health = 100; 	} // If type is 4 (HEALTH), set health to 100
			
			other.transform.GetComponent<Powerup>().Remove(); // Remove power-up
			
		}
		
		// On collision with a bullet
		
		if(other.transform.GetComponent<Bullet>()){
			
			if(player != other.transform.GetComponent<Bullet>().player){
				
				// If the bullet is shot by a different player
				
				Destroy(other.gameObject); // Remove the bullet
				Hit(other.transform.position, 10); // Get hit (subtract health or respawn), send hit position
				
			}
			
		}
		
		// On collision with missile
		
		if(other.transform.GetComponent<Missile>()){
			
			if(player != other.transform.GetComponent<Missile>().player){
				
				// If the missile is shot by a different player
				
				other.transform.GetComponent<Missile>().Explode(); // Explode the missile
				Hit(other.transform.position, 25); // Get hit (subtract health or respawn), send hit position
				
			}
			
		}
		
		// On collision with explosion
		
		if(other.transform.name == "explosion"){
			
			Hit(other.transform.position, 50);
			
		}
		
	}
	
}
