using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Door : MonoBehaviour{
	
	Vector3 doorTarget = Vector3.zero;
	
	public bool locked = false;
	public bool ending = false;
	
	void Update(){
		
		// Animate the door to open/close
		
		transform.localPosition = Vector3.Lerp(transform.localPosition, doorTarget, Time.deltaTime * 6.0f);
		
	}
	
	// Player can open the door
	
	public void Open(bool hasKey){
		
		if(!locked || hasKey){
		
			doorTarget = new Vector3(-0.6f, 0, 0);
			
			Audio.Play("door");
			
			Invoke("Close", 6); // Close door after 6 seconds
			
		}
		
	}
	
	// Closing happens after 6 seconds (see above)
	
	public void Close(){
		
		doorTarget = new Vector3(0, 0, 0);
		
	}
	
}