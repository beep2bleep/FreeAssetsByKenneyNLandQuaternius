using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Track : MonoBehaviour{
	
    void Awake(){
		
		foreach(Transform t in GetComponentsInChildren<Transform>()){
			
			if(t.name.Contains("%physics")){ t.gameObject.AddComponent<PhysicsObject>(); }
			if(t.name.Contains("%billboard")){ t.gameObject.GetComponent<MeshCollider>().enabled = false; }
			
			if(t.name.Contains("%water")){
				
				t.gameObject.GetComponent<MeshCollider>().enabled = false;
				t.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				
			}
			
		}
		
	}
	
}