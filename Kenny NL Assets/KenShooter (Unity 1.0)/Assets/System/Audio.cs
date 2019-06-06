using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour{
	
	public static AudioSource audioSource;
	public static Object[] sounds;
	
	void Awake(){
		
		// Load all audio files in project
		
		sounds = Resources.LoadAll("Audio", typeof(AudioClip));
		audioSource = GetComponent<AudioSource>();
		
	}
	
	// Play audio using general audio source
	
	public static void Play(string s){
		
		audioSource.pitch = Random.Range(0.9f, 1.1f);
		
		// Find correct sound to play
		
		foreach(AudioClip audioClip in sounds){
			
			if(audioClip.name == s){
				
				audioSource.PlayOneShot(audioClip, 1.0f);
				
				break;
				
			}
			
		}
		
	}
	
}