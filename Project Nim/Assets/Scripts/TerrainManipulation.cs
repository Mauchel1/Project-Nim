using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManipulation : MonoBehaviour
{

    public enum ExtrudeMethod
    {
        Vertical,
        MeshNormal
    }


	public TerrainGenerator terrainGenerator;

    public float radius = 3f;
    public float power = 2.0f;
	
	
	RaycastHit hit = new RaycastHit();
	private MeshFilter filter;


    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);



	    if (Physics.Raycast(ray.origin, ray.direction, out hit))
	    {
	    Debug.DrawRay(hit.point, hit.normal, Color.red);
		    if (Input.GetMouseButtonDown(0))
		    {

			    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			    sphere.transform.position = hit.point;
			    //GameObject chunk = hit.collider.gameObject;
			    //filter = hit.collider.gameObject.GetComponent<MeshFilter>();

				//Debug.Log(mesh.vertices.Length);
				//mesh.transform.position += new Vector3(0, 15f, 0);
				//List<Terrainchunk> terrainchunks = terrainGenerator.GetTerrainchunks(hit.point, radius);
			}
		    
		    if (Input.GetMouseButton(0))
		    {
				List<Terrainchunk> terrainchunks = terrainGenerator.GetTerrainchunks(hit.point, radius);
				foreach (Terrainchunk t in terrainchunks)
                {
					if (t != null)
					{
						ModifyMesh(t, hit.point);
					}
			    }
		    }
	    }
    }

	void ModifyMesh(Terrainchunk t, Vector3 hitpoint)
	{
		Mesh mesh = t.meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;

		for (int i = 0; i < vertices.Length; ++i)
		{
			Vector3 v = t.meshFilter.transform.TransformPoint(vertices[i]);
			//vertices[i] += hit.normal * Gaussian(v, hit.point, radius);
			vertices[i] += Vector3.up * Gaussian(v, hit.point, radius);

		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();

		if (t.meshCollider != null)
		{
			var colliMesh = new Mesh();
			colliMesh.vertices = mesh.vertices;
			colliMesh.triangles = mesh.triangles;
			t.meshCollider.sharedMesh = colliMesh;
		}

	}

	static float Gaussian(Vector3 pos, Vector3 mean, float dev)
	{
		float x = pos.x - mean.x;
		float y = pos.y - mean.y;
		float z = pos.z - mean.z;
		if (Mathf.Abs(x) > dev || Mathf.Abs(z) > dev) 
		{
			return 0; 
		}
		float n = 1.0f / (2.0f * Mathf.PI * dev * dev);
		n *= Mathf.Pow(2.718281828f, -(x * x + y * y + z * z) / (2.0f * dev * dev));
		//n = (n < 0.1) ? 0 : n ;
		return n;
	}


}
