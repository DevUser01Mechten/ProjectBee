using UnityEngine;
using System.Collections;

public class createGround : MonoBehaviour {
	public GameObject flower;
	public GameObject grass;

	float sizeOfSquare;
	float originalSizeOfSquare;
	float percentChange;
	Vector3 startPos;
	// Use this for initialization
	void Start () {
		CreateStartGround ();

	}

	void CreateStartGround()
	{
		startPos = new Vector3 (0, 0, -20);
		sizeOfSquare = Screen.width / 5 / 10;
		
		originalSizeOfSquare = flower.GetComponent<MeshRenderer> ().bounds.size.x;
		
		percentChange = (sizeOfSquare / originalSizeOfSquare);
		Debug.Log (Screen.width/10);
		for(int r=0; r < 10; r++)
		{
			for(int c= 0; c < 5; c++)
			{
				GameObject temp = (GameObject)Instantiate(grass, new Vector3(startPos.x-((c-(5/2))*sizeOfSquare), 0, startPos.z+(r*sizeOfSquare)), Quaternion.identity);
				temp.transform.localScale = temp.transform.localScale * percentChange;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {


	}
}
