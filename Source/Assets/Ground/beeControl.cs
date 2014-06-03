using UnityEngine;
using System.Collections;

public class beeControl : MonoBehaviour {
	Vector3 moveToPos;

	Camera myCam;

	void Start()
	{
		myCam = transform.parent.camera;
	}

	void Update()
	{
#if UNITY_STANDALONE
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit = new RaycastHit();
			Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, 100))
			{
				moveToPos = new Vector3(hit.point.x, transform.position.y, transform.position.z);
				transform.position = moveToPos;
			}
		}
#endif

#if UNITY_STANDALONE_OSX || UNITY_IPHONE
<<<<<<< HEAD
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
=======
		if(Input.touchCount > 0 )
>>>>>>> FETCH_HEAD
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				RaycastHit hit = new RaycastHit();
				Ray ray = myCam.ScreenPointToRay(Input.GetTouch(0).position);
				if(Physics.Raycast(ray, out hit, 100))
				{
					moveToPos = new Vector3(hit.point.x, transform.position.y, transform.position.z);
					transform.position = moveToPos;
				}
			}
		}
#endif
	}



	void OnTriggerEnter(Collider obj)
	{
		if(obj.collider.tag != "path")
		{
			// collided with wall
			Debug.Log("Hit Wall");
		}
	}
}
