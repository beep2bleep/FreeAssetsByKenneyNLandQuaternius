using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour{
	
	// Public
	
	public Tilemap tilemap;
	
	// Private
	
	private Vector3 playerStart;
	
    void Awake(){
		
		// Disable the 2D tilemap
		
		tilemap.gameObject.SetActive(false);
		
		// Generate 3D map
		
		GenerateMap();
		
	}
	
	void GenerateMap(){
		
		// Get all tiles on map
		
		BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
		
		for(int x = 0; x < bounds.size.x; x++){
            for(int y = 0; y < bounds.size.y; y++){
				
				// Get tile
                
				TileBase tile = allTiles[x + y * bounds.size.x];
				
				Vector2Int tilePosition = new Vector2Int(x + bounds.x, y + bounds.y);
                
				if(tile != null){
					
					// Load prefab
					
                    GameObject prefabLoad = Resources.Load("Prefabs/" + tile.name) as GameObject;
					
					if(prefabLoad != null){
						
						// Place prefab in map
						
						GameObject prefab = Instantiate(prefabLoad, new Vector3(tilePosition.x + 0.5f, 0, tilePosition.y + 0.5f), Quaternion.identity, transform);
						
						prefab.name = tile.name;
						
						// Tile rotation
						
						Matrix4x4 matrix = tilemap.GetTransformMatrix(new Vector3Int(tilePosition.x, tilePosition.y, 0));
					
						prefab.transform.eulerAngles = new Vector3(0, matrix.rotation.eulerAngles.z, 0); // Change Z to Y rotation
						
					}
					
					// Place floor tile when not a wall
					
					if(tile.name != "wall"){
					
						GameObject prefabFloor = Instantiate(Resources.Load("Prefabs/floor"), new Vector3(tilePosition.x + 0.5f, 0, tilePosition.y + 0.5f), Quaternion.identity, transform) as GameObject;
					
					}
					
                }
				
            }
        }
		
	}
	
}
