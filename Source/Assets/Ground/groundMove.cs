using UnityEngine;
using System.Collections;

public class groundMove : MonoBehaviour {
	float speed = 5;
	Vector3 tempPos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		tempPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z-(speed*Time.deltaTime));
		transform.position = tempPos;
	
	}
}
