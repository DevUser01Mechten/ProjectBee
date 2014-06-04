using UnityEngine;
using System.Collections;

public class beeControl : MonoBehaviour {
	Vector3 moveToPos;
	Camera myCam;
	public Material[] beeMaterials;
	float transPVal = 1;
	bool blinkOut = true;
	bool isDead = false;
	int blinkCount = 3;
	float speedBlink = 0.1f;
	bool gameStarted = false;
	bool veryDead = false;

	beeGUI myBeeGui;

	GameObject instruction;
	float counter = 5;
	
	void Start()
	{
		instruction = transform.root.FindChild ("instruction").gameObject;
		myBeeGui = transform.root.GetComponent<beeGUI> ();
		for(int i =0; i < beeMaterials.Length; i++)
		{
			beeMaterials[i].SetColor("_Color", new Color(beeMaterials[i].color.r,beeMaterials[i].color.g,beeMaterials[i].color.b, 1));
		}
		myCam = transform.parent.camera;
	}

	void Update()
	{

		// When the bee collids with the wall it loses a life
		if(isDead)
		{
			// count down the blinks
			if(blinkCount > 0)
			{
				// blinking out
				if(blinkOut)
				{
					transPVal = Mathf.Lerp(transPVal, 0, speedBlink);
					if(transPVal < 0.05f)
					{

						blinkOut = false;
					}
					for(int i =0; i < beeMaterials.Length; i++)
					{
						beeMaterials[i].SetColor("_Color", new Color(beeMaterials[i].color.r,beeMaterials[i].color.g,beeMaterials[i].color.b, transPVal));
					}

					
				}
				// blinking in
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

					for(int i =0; i < beeMaterials.Length; i++)
					{
						beeMaterials[i].SetColor("_Color", new Color(beeMaterials[i].color.r,beeMaterials[i].color.g,beeMaterials[i].color.b, transPVal));
					}

				}
			}
			else
			{
				isDead = false;
			}
		}

		// completely dead
		if(myBeeGui.numLifes <= 0 && veryDead == false)

		{
			counter -= Time.deltaTime;
			if(counter < 0)
			{
				veryDead = true;
				MainMenu.Instance.displayMainMenu = true;
				transform.root.GetComponent<beeGUI>().dispMenu = false;
				myBeeGui.SaveScore();
			}
			
		}
		
		
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
		if(Input.GetKeyDown(KeyCode.Space))
		{
			// start game
			if(gameStarted == false  && myBeeGui.numLifes > 0)
			{
				transform.root.GetComponent<beeGUI>().gameStarted = true;
				transform.root.GetComponent<createGround>().startGame = true;
				Destroy(instruction);
			}
		}


		for(int i=0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
			{
				// start game
				if(gameStarted == false && myBeeGui.numLifes >0)
				{
					transform.root.GetComponent<beeGUI>().gameStarted = true;
					transform.root.GetComponent<createGround>().startGame = true;
					Destroy(instruction);
				}

				if(myBeeGui.numLifes > 0)
				{
					RaycastHit hit = new RaycastHit();
					Ray ray = myCam.ScreenPointToRay(touch.position);
					if(Physics.Raycast(ray, out hit, 100))
					{
						moveToPos = new Vector3(hit.point.x, transform.position.y, transform.position.z);
						transform.position = moveToPos;
					}
				}

			}
		}
#endif
	}

	// when the bee hits the wall, this sets the variable
	void KillBee(GameObject obj)
	{
		if(isDead == false)
		{
			BlinkObj(obj);
			isDead = true;
			blinkOut = true;
			blinkCount = 3;
			myBeeGui.numLifes--;
			if(myBeeGui.numLifes <=0)
			{
				myBeeGui.gameStarted = false;
				transform.root.GetComponent<createGround>().startGame = false;
				transform.root.audio.Play();
				GetComponent<Animation>().Play("Die");
				GetComponent<Collider>().enabled = false;
			}
			else
			{
				float PathXPos = transform.root.GetComponent<createGround> ().GetXPosForBee ();
				transform.position = new Vector3 (PathXPos, transform.position.y, transform.position.z);
			}
		}
		else
		{
			float PathXPos = transform.root.GetComponent<createGround> ().GetXPosForBee ();
			transform.position = new Vector3 (PathXPos, transform.position.y, transform.position.z);
		}
	}

	// collider to check if we hit the wall
	void OnTriggerEnter(Collider obj)
	{
		if(obj.collider.tag != "path")
		{
			// collided with wall
			KillBee(obj.gameObject);

		}
	}

	// start the blinking
	void BlinkObj(GameObject obj)
	{
		obj.GetComponent<blinkGround> ().enabled = true;
		obj.GetComponent<blinkGround> ().StartBlink ();
	}

}
