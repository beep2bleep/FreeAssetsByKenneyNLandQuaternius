using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class Playfield : MonoBehaviour{
	
	[Header("Settings")]
	
	public Vector2 spawnPosition = new Vector2(5, 18);
	public int levelGoal = 10;
	
	[Header("Interface")]
	
	public Text displayScore;
	public Text displayLines;
	public Text displayLevel;
	
	public Transform gameOver;
	public Transform next;
	
	int score = 0;
	int lines = 0;
	int level = 1;
	
	[Header("Controls")]
	
	public KeyCode controlLeft	 = KeyCode.LeftArrow;
	public KeyCode controlRight	 = KeyCode.RightArrow;
	public KeyCode controlRotate = KeyCode.UpArrow;
	public KeyCode controlDrop	 = KeyCode.DownArrow;
	
	float keyDelay  = 0.10f; // Key press delay
    float keyPassed = 0f;
	
	[Header("Sounds")]
	
	public AudioClip soundPlace;
	public AudioClip soundLine;
	public AudioClip soundCombo;
	public AudioClip soundOver;
	public AudioClip soundRotate;
	
	AudioSource audioSource;
	
	[Header("Blocks")]
	
	public GameObject[] blocks;
	
	// Private

	float lastUpdate = 0;
	
	int nextBlock;
	int levelGoalSet;
	
	float fallSpeed = 1.0f;
	
	Transform fallingBlock;
	
	// Grid
	
	int gridWidth  = 10;
    int gridHeight = 40;
	
    Transform[,] grid;
	
	// Methods
	
	void Awake(){
		
		grid = new Transform[gridWidth, gridHeight];
		
		audioSource = GetComponent<AudioSource>();
		
		levelGoalSet = levelGoal;
		
		UpdateInterface();
		
		RandomBlock();
		SpawnBlock();
		
	}
	
	// Pick a random block, spawn a new block at the top of the screen
	
	public void RandomBlock(){
		
		// Pick random value
		
		int i = Random.Range(0, blocks.Length);
		
		// Remove previous 'next block'
		
		foreach(Transform child in next){ GameObject.Destroy(child.gameObject); }
		
		// Add new block
		
		GameObject nextFallingBlock = Instantiate(blocks[i], Vector3.zero, transform.rotation, next);
		
		// Get center of block
		
		Vector3 blockCenter = CalculateCentroid(nextFallingBlock.transform.Cast<Transform>().ToArray());
		
		// Offset position
		
		nextFallingBlock.transform.localPosition = new Vector2(-blockCenter.x - 0.5f, -blockCenter.y);
		
		nextBlock = i;
		
	}
	
	public void SpawnBlock(){
		
		fallingBlock = Instantiate(blocks[nextBlock], Vector3.zero, transform.rotation, transform.GetChild(1)).transform;
		fallingBlock.localPosition = new Vector2(spawnPosition.x, spawnPosition.y);
		
		RandomBlock();
		
		// If the new block collides with other blocks, it's game over
		
		if(!CheckValidPosition()){
			
			gameOver.gameObject.SetActive(true);
			
			audioSource.PlayOneShot(soundOver, 0.5f);
			Destroy(fallingBlock.gameObject);
			
		}
		
	}
	
	// Update interface
	
	void UpdateInterface(){
		
		// Display score, lines and level statistics
		
		displayScore.text = score.ToString();
		displayLines.text = lines.ToString();
		displayLevel.text = level.ToString();
		
	}
	
	// Update
	
	void Update(){
		
		// Key repeating
		
		keyPassed += Time.deltaTime;
		
		if(fallingBlock != null){
		
			// Move Left
			
			if(Input.GetKeyDown(controlLeft) || (Input.GetKey(controlLeft) && keyPassed >= keyDelay)){
			
				fallingBlock.localPosition += new Vector3(-1, 0, 0);
			
				if(CheckValidPosition()){
				
					UpdateGrid(); // Move to new position
				
				}else{
				
					fallingBlock.localPosition += new Vector3(1, 0, 0); // Move to previous position
			
				}
				
				keyPassed = 0f;
			
			}
			
			// Move right
			
			else if(Input.GetKeyDown(controlRight) || (Input.GetKey(controlRight) && keyPassed >= keyDelay)){
			
				fallingBlock.localPosition += new Vector3(1, 0, 0);
			
				if(CheckValidPosition()){
				
					UpdateGrid(); // Move to new position
				
				}else{
				
					fallingBlock.localPosition += new Vector3(-1, 0, 0); // Move to previous position
			
				}
				
				keyPassed = 0f;
			
			}
			
			// Rotate
			
			else if(Input.GetKeyDown(controlRotate)){
				
				fallingBlock.Rotate(0, 0, -90);
				
				if(CheckValidPosition()){
				
					UpdateGrid(); // Rotate to new orientation
				
				}else{
				
					fallingBlock.Rotate(0, 0, 90); // Rotate to previous orientation
			
				}
				
				audioSource.PlayOneShot(soundRotate, 0.5f);
				
			}
			
			// Move downwards and fall

			else if((Input.GetKey(controlDrop) && keyPassed >= keyDelay / 3) || Time.time - lastUpdate >= fallSpeed){
				
				fallingBlock.localPosition += new Vector3(0, -1, 0);
				
				if(CheckValidPosition()){
					
					UpdateGrid(); // Moving down...
					
				}else{
					
					// Collision with other blocks or the bottom
					
					fallingBlock.localPosition += new Vector3(0, 1, 0);
					
					DeleteFullRows();
					
					SpawnBlock();
					
					audioSource.PlayOneShot(soundPlace, 1.0f);
					
				}
				
				lastUpdate = Time.time;
				
				keyPassed = 0f;
				
			}
			
		}
		
	}
	
	// Check if each tile in block has a valid position
	
	bool CheckValidPosition(){
		
		// Check each tile of the block

		foreach(Transform child in fallingBlock){

			Vector2 v = RoundVector(transform.InverseTransformPoint(child.position));

			if(!InsidePlayfield(v)){ return false; }

			if(grid[(int)v.x, (int)v.y] != null && grid[(int)v.x, (int)v.y].parent != fallingBlock){ return false; }
			
		}
		
		return true;
		
	}
	
	// Upgrade grid
	
	void UpdateGrid(){
		
		for(int y = 0; y < gridHeight; y++){
			for(int x = 0; x < gridWidth; x++){
				
				if(grid[x, y] != null){
					
					if(grid[x, y].parent == fallingBlock){
						
						grid[x, y] = null;
						
					}
					
				}
				
			}
			
		}
		
		// Check each tile of the block
		
		foreach(Transform child in fallingBlock){
			
			Vector2 v = RoundVector(transform.InverseTransformPoint(child.position));
			grid[(int)v.x, (int)v.y] = child;
			
		}     
		
	}
	
	// Round to vector2
	
	public Vector2 RoundVector(Vector2 v){

		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
		
	}
	
	// Inside border
	
	public bool InsidePlayfield(Vector2 pos){

		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
		
	}
	
	// Delete row
	
	public void DeleteRow(int y){
		
		for(int x = 0; x < gridWidth; ++x){
			
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
			
		}
		
	}
	
	// Decrease row
	
	public void DecreaseRow(int y){
		
		for(int x = 0; x < gridWidth; ++x){
			
			if(grid[x, y] != null){

				grid[x, y - 1] = grid[x, y];
				grid[x, y] = null;

				grid[x, y - 1].position += new Vector3(0, -1, 0);
			
			}
			
		}
		
	}
	
	// Decrease row above

	public void DecreaseRowsAbove(int y){
		
		for(int i = y; i < gridHeight; ++i){ DecreaseRow(i); }
		
	}
	
	// Check if row is complete
	
	public bool CheckFullRow(int y) {
		
		for(int x = 0; x < gridWidth; x++){
			
			if(grid[x, y] == null){ return false; }
			
		}
		
		return true;
		
	}
	
	// Delete full row(s)
	
	public void DeleteFullRows(){
		
		int rows = 0;
    
		for(int y = 0; y < gridHeight; ++y){
			
			if(CheckFullRow(y)){
				
				DeleteRow(y);
				DecreaseRowsAbove(y + 1);
				
				rows++; lines++; UpdateInterface();
				
				levelGoal--;
            
				y--;
				
			}
			
		}
		
		// Add score depending on number of rows
		
		AddScore(rows);
		
		// Sound effects
		
		if(rows > 3){
			
			audioSource.PlayOneShot(soundCombo, 0.50f);
			
		}else if(rows > 0){
			
			audioSource.PlayOneShot(soundLine, 0.15f);
			
		}
		
		// Level up
		
		if(levelGoal <= 0){ NextLevel(); }
		
	}
	
	// Next level
	
	public void NextLevel(){
		
		fallSpeed = Mathf.Clamp(1.0f - (level / 10), 0.1f, 1.0f);
		
		level++;
		
		levelGoal = levelGoalSet;
		
	}
	
	// Center of multiple objects
	
	public Vector3 CalculateCentroid(Transform[] transforms){
		
		Vector3 centroid = Vector3.zero;
		
		foreach(Transform t in transforms){
			
			centroid += t.localPosition;
			
		}
		
		centroid /= transforms.Length;
		
		return centroid;
		
	}
	
	// Add score
	
	public void AddScore(int lines){
		
		int addScore = 0;
		
		// Score based on amount of lines
		
		switch(lines){
			
			case 1: addScore = 10; break;
			case 2: addScore = 30; break;
			case 3: addScore = 50; break;
			case 4: addScore = 80; break;
			
		}
		
		// Score based on current level
		
		addScore *= level;
		
		score += addScore;
		UpdateInterface();
		
	}
	
}