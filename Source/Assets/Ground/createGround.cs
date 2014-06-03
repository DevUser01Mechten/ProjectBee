using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class createGround : MonoBehaviour {
	public GameObject flower;
	public GameObject grass;

	float sizeOfSquare;
	float originalSizeOfSquare;
	float percentChange;
	Vector3 startPos;
	List<GameObject> grounds;
	public float speed = 5;
	Vector3 pos;
	float lastZPos;
	int lastRowNum = 0;
	int firstRowNum = -1;
	int ind = 0;
	// Use this for initialization
	void Start () {
		grounds = new List<GameObject>();


		CreateStartGround ();
		lastZPos = transform.position.z;
	}


	void CreateStartGround()
	{
		startPos = new Vector3 (0, 0, -20);
		sizeOfSquare = Screen.width / 5 / 10;
		
		originalSizeOfSquare = flower.GetComponent<MeshRenderer> ().bounds.size.x;
		
		percentChange = (sizeOfSquare / originalSizeOfSquare);

		for(int r=0; r < 7; r++)
		{
			for(int c= 0; c < 5; c++)
			{
				GameObject temp = (GameObject)Instantiate(grass, new Vector3(startPos.x-((c-(5/2))*sizeOfSquare), 0, startPos.z+(r*sizeOfSquare)), Quaternion.identity);
				temp.transform.localScale = temp.transform.localScale * percentChange;
				temp.GetComponent<typeInfo>().index = ind;
				temp.GetComponent<typeInfo>().row = r;
				temp.GetComponent<typeInfo>().col = c;
				temp.GetComponent<typeInfo>().isPath = true;
				grounds.Add(temp);
				ind++;
			}
			firstRowNum++;
		}
	}

	void DeleteLastRow()
	{
		List<int> toDelete = new List<int>();
		for(int i=0; i<grounds.Count; i++)
		{
			if(grounds[i].GetComponent<typeInfo>().row == lastRowNum)
			{
				Destroy(grounds[i]);
				toDelete.Add(i);
			}
		}

		for(int i=toDelete.Count-1; i >=0; i--)
		{
			grounds.RemoveAt(toDelete[i]);
		}

		lastRowNum++;
	}
	float GetZPos()
	{
		for(int i=0; i < grounds.Count; i++)
		{
			if(grounds[i].GetComponent<typeInfo>().row == firstRowNum)
			{
				return grounds[i].transform.position.z;
			}
		}
		return -100f;
	}
	int GetXIndex()
	{
		for(int i=0; i < grounds.Count; i++)
		{
			if(grounds[i].GetComponent<typeInfo>().row == firstRowNum)
			{
				if(grounds[i].GetComponent<typeInfo>().isPath == true)
					return grounds[i].GetComponent<typeInfo>().col;
			}
		}
		return -100;
	}

	void GenerateNewRow()
	{
		float zPos = GetZPos ();
		int xIndex = GetXIndex ();
		int junctIndex = -100;
		bool isJunction = false;
		firstRowNum++;

		int rand = Random.Range(0,10);
		// create a junction
		if(rand < 6)
		{
			isJunction = true;
			bool goLeft = false;
			int rand2 = (int)Random.value;
			if(rand2 %2 == 0 && xIndex < 4)
			{
				goLeft = true;
			}
			else
			{
				if(xIndex == 0)
					goLeft = true;
			}

			// go left the tunnel
			if(goLeft)
			{
				junctIndex = xIndex+1;
			}
			// go right
			else
			{
				junctIndex = xIndex -1;
			}
		}


		for(int i =0; i < 5; i++)
		{
			// generate a path
			if(i == xIndex || junctIndex == i)
			{
				GameObject temp = (GameObject)Instantiate(grass, new Vector3(startPos.x-((i-(5/2))*sizeOfSquare), 0, zPos+sizeOfSquare), Quaternion.identity);
				temp.transform.localScale = temp.transform.localScale * percentChange;
				temp.GetComponent<typeInfo>().index = ind;
				temp.GetComponent<typeInfo>().row = firstRowNum;
				temp.GetComponent<typeInfo>().col = i;
				temp.GetComponent<typeInfo>().isPath = true;
				grounds.Add(temp);
				ind++;


			}
			// it's not a path
			else
			{
				GameObject temp = (GameObject)Instantiate(flower, new Vector3(startPos.x-((i-(5/2))*sizeOfSquare), 0, zPos+sizeOfSquare), Quaternion.identity);
				temp.transform.localScale = temp.transform.localScale * percentChange;
				temp.GetComponent<typeInfo>().index = ind;
				temp.GetComponent<typeInfo>().row = firstRowNum;
				temp.GetComponent<typeInfo>().col = i;
				temp.GetComponent<typeInfo>().isPath = false;
				grounds.Add(temp);
				ind++;
			}
		}

		if(isJunction)
		{
			for(int i =0; i < grounds.Count; i++)
			{
				if(grounds[i].GetComponent<typeInfo>().col == xIndex &&
				   grounds[i].GetComponent<typeInfo>().row == firstRowNum)	
				{
					grounds[i].GetComponent<typeInfo>().isPath = false;
				}
			}
		}
	


	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		MoveCamera ();
		if(transform.position.z > lastZPos + sizeOfSquare)
		{
			lastZPos = transform.position.z;

			DeleteLastRow();
			GenerateNewRow();

		}

		/*for(int i=0; i < grounds.Count; i++)
		{
			if(grounds[i].transform.position.z < (-20-sizeOfSquare))
			{

				GameObject temp = (GameObject)Instantiate(grass, new Vector3(grounds[i].transform.position.x, 0, 14-sizeOfSquare), Quaternion.identity);
				temp.transform.localScale = temp.transform.localScale * percentChange;
				grounds.Add(temp);
				GameObject.Destroy(grounds[i]);
				grounds.RemoveAt(i);
			}
		}*/

	}

	void MoveCamera()
	{
		pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z + (speed*Time.deltaTime));
		transform.position = pos;
	}
}
