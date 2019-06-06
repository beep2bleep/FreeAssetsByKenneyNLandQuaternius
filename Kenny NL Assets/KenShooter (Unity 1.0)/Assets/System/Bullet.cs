using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{
	
	public GameObject impact;
	
    void Update(){
		
		// Move bullet forward, 10.0f is the speed of the bullet
		
		transform.Translate(Vector3.forward * 10.0f * Time.deltaTime);
		
		// There's no ceiling, so if the bullet goes too high it'll remove itself
		
		if(transform.position.y < 0 || transform.position.y > 1){ Hit(); }
		
	}
	
	// On impact with collider or floor/ceiling
	
	void Hit(){
		
		GameObject i = Instantiate(impact, transform.position, transform.rotation) as GameObject;
		
		Destroy(i, 1.0f);
		Destroy(gameObject);
		
		Audio.Play("impact");
		
	}
	
	void OnTriggerEnter(Collider other){
		
		if(other.transform.GetComponent<Enemy>()){
			
			other.transform.GetComponent<Enemy>().Hit();
			
		}
		
		Hit();
		
	}
	
}