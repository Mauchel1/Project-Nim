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

    public float radius = 1.5f;
    public float power = 2.0f;
	
	
	RaycastHit hit = new RaycastHit();


    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);



	    if (Physics.Raycast(ray.origin, ray.direction, out hit))
	    {
	    Debug.DrawRay(hit.point, hit.normal, Color.red);

		    if (Input.GetMouseButtonUp(0))
		    {

			    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			    sphere.transform.position = hit.point;
			    Mesh mesh = hit.collider.gameObject.GetComponent<Mesh>();
			    Debug.Log(mesh.vertices.Length);
				//mesh.transform.position += new Vector3(0, 15f, 0);

		    }

		    if (Input.GetMouseButton(0))
		    {
				ModifyMesh();
		    }
	    }
    }

	void ModifyMesh()
	{

	}

}
