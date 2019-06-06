using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour{
	
	public Vehicle vehicle;
	
	Dictionary<string, bool> controls;
	
	void Awake(){
		
		controls = new Dictionary<string, bool>(){
			
			{"left", false},
			{"right", false},
			{"accelerate", false},
			{"brake", false}
			
		};
		
	}
	
	void Update(){
		
		if(controls["left"]){ vehicle.ControlSteer(-1); }
		if(controls["right"]){ vehicle.ControlSteer(1); }
		if(controls["accelerate"]){ vehicle.ControlAccelerate(); }
		if(controls["brake"]){ vehicle.ControlBrake(); }
		
	}
	
	public void PressJump(){ vehicle.ControlJump(); }
	
	public void PressButton(string b){ controls[b] = true; }
	public void ReleaseButton(string b){ controls[b] = false; }
	
}
