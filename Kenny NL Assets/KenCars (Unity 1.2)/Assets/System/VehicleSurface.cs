using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSurface : MonoBehaviour{
	
    // Optional component for sampling surface
	
	Material surfaceMaterial;
	string surfaceMaterialName; // Name without "(Instance)"
	
	void FixedUpdate(){
		
		SurfaceSample();
		
		// Example: Disable jumping when driving on material named 'dirt'
		
		GetComponent<Vehicle>().jumpAbility = surfaceMaterialName != "dirt";
		
	}
	
	void SurfaceSample(){
		
		RaycastHit hit;
		
		if(Physics.Raycast(transform.position, Vector3.down, out hit, 1.25f)){
			
			Renderer renderer = hit.collider.GetComponent<Renderer>();
			MeshCollider meshCollider = hit.collider as MeshCollider;
			
			if(renderer == null || renderer.sharedMaterial == null || meshCollider == null){ return; }
			
			int materialIndex = -1;
 
			Mesh mesh = meshCollider.sharedMesh;
			
			int triangleIndex = hit.triangleIndex;
			
			int lookupId1 = mesh.triangles[triangleIndex * 3];
			int lookupId2 = mesh.triangles[triangleIndex * 3 + 1];
			int lookupId3 = mesh.triangles[triangleIndex * 3 + 2];
			
			int subMeshesIndex = mesh.subMeshCount;
			
			for(int i = 0; i < subMeshesIndex; i++){

				int[] triangles = mesh.GetTriangles(i);
				
				for(int j = 0; j < triangles.Length; j += 3){
					
					if(triangles[j] == lookupId1 && triangles[j + 1] == lookupId2 && triangles[j + 2] == lookupId3){
						
						materialIndex = i; break;
						
					}
					
				}
				
				if(materialIndex != -1) break;
				
			}
			
			if(materialIndex != -1){
				
				surfaceMaterial = renderer.materials[materialIndex];
				surfaceMaterialName = surfaceMaterial.name.Replace(" (Instance)", "");
				
			}else{ surfaceMaterial = null; surfaceMaterialName = null; }
			
		}
		
	}
	
}
