using UnityEngine;
using System.Collections;

public class blinkGround : MonoBehaviour {
	bool isHit = false;
	int blinkCount = 3;
	bool blinkOut = true;
	float transPVal = 1;
	float speedBlink = 0.1f;

	Material myMat;

	void Start()
	{
		myMat = renderer.material;
		myMat.SetColor("_Color", new Color(0,myMat.color.g,myMat.color.b, myMat.color.a));
	}

	public void StartBlink()
	{
		isHit = true;
		blinkCount = 3;
		blinkOut = false;
		transPVal = 1;

	}
	
	// Update is called once per frame
	void Update () {
		if(isHit)
		{
			if(blinkCount > 0)
			{
				if(blinkOut)
				{
					//transPVal -= (speedBlink*Time.deltaTime);
					transPVal = Mathf.Lerp(transPVal, 0, speedBlink);
					if(transPVal < 0.05f)
					{
						
						blinkOut = false;
						blinkCount--;
					}

					myMat.SetColor("_Color", new Color(transPVal,myMat.color.g,myMat.color.b, myMat.color.a));

					
					
				}
				else
				{
					//transPVal += (speedBlink*Time.deltaTime);
					transPVal = Mathf.Lerp(transPVal, 1, speedBlink);
					if(transPVal >= 0.95f)
					{
						Debug.Log("stop blinking");
						transPVal = 1;
						blinkOut = true;
						blinkCount--;
						
					}
					
					
					myMat.SetColor("_Color", new Color(transPVal,myMat.color.g,myMat.color.b, myMat.color.a));

					
				}
			}
			else
			{
				isHit = false;
				GetComponent<blinkGround>().enabled = false;
			}
			
		}
	}
}
