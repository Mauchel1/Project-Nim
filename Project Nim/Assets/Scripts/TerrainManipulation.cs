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


	public Dictionary<Vector2, Terrainchunk> terrainChunkDict;

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
			    GameObject chunk = hit.collider.gameObject;
			    filter = hit.collider.gameObject.GetComponent<MeshFilter>();
				
			    //Debug.Log(mesh.vertices.Length);
			    //mesh.transform.position += new Vector3(0, 15f, 0);

		    }
		    
		    if (Input.GetMouseButton(0))
		    {
			    if (filter != null && filter.mesh != null)
			    {
					ModifyMesh(filter, hit.point);
			    }
		    }
	    }
    }

	void ModifyMesh(MeshFilter f, Vector3 hitpoint)
	{
		Mesh mesh = f.mesh;
		Vector3[] vertices = mesh.vertices;

		for (int i = 0; i < vertices.Length; ++i)
		{
			Vector3 v = f.transform.TransformPoint(vertices[i]);
			//vertices[i] += hit.normal * Gaussian(v, hit.point, radius);
			vertices[i] += Vector3.up * Gaussian(v, hit.point, radius);

		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();

		MeshCollider collider = f.GetComponent<MeshCollider>();
		if (collider != null)
		{
			var colliMesh = new Mesh();
			colliMesh.vertices = mesh.vertices;
			colliMesh.triangles = mesh.triangles;
			collider.sharedMesh = colliMesh;
		}

	}

	static float Gaussian(Vector3 pos, Vector3 mean, float dev)
	{
		float x = pos.x - mean.x;
		float y = pos.y - mean.y;
		float z = pos.z - mean.z;
		float n = 1.0f / (2.0f * Mathf.PI * dev * dev);
		return n * Mathf.Pow(2.718281828f, -(x * x + y * y + z * z) / (2.0f * dev * dev));
	}


}
