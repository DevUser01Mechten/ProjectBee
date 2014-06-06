using UnityEngine;
using System.Collections;

public class beeGUI : MonoBehaviour {
	public float score = 0.0f;
	public int numLifes = 1;
	public GUIStyle myStyle;
	public GUIStyle myButtonStyle;

	int screenW;
	int screenH;

	int lableW;
	int lableH;
	public bool gameStarted = false;
	public bool dispMenu = true;
	int isExtra = 0;
	
	void Awake()
	{
		isExtra = EncryptedPlayerPrefs.GetInt("extraLives");
		if(isExtra == 1)
			numLifes = 4;
	}

	void Start()
	{
		screenW = Screen.width;
		screenH = Screen.height;

		lableW = screenW / 8;
		lableH = screenH / 20;
		score = 0.0f;
		
		if (EncryptedPlayerPrefs.CheckEncryption ("xtralives","int","1"))
		{
			numLifes = 4;
		}
		
		if (EncryptedPlayerPrefs.CheckEncryption ("10xtralives","int","1"))
		{
			numLifes = 11;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(gameStarted)
		{
			score += (Time.deltaTime* 10);
			//make speed increase gradually
			if ((int)score%100 == 0)
			{
				transform.root.GetComponent<createGround>().speed = 10f + (score * 0.01f);
			}
		}
	}

	void OnGUI()
	{
		if(dispMenu)
		{
			GUI.Label (new Rect (screenW - (lableW * 3f), 0f, lableW*3f, lableH), "Score: " + (int)score, MainMenu.Instance.customHighlightStyle);
			GUI.Label (new Rect (lableW, 0f, lableW*1.5f, lableH), "Lives: " + numLifes, MainMenu.Instance.customHighlightStyle);
			//menu
			if (GUI.Button (new Rect (0f, 0f, lableW, lableH), "PAUSE", MainMenu.Instance.customButtonStyle))
			{
				MainMenu.Instance.displayMainMenu = true;
				Time.timeScale = 0.0000001f;
				dispMenu = false;
			}
		}
	}

	public void SaveScore()
	{
		int highS = EncryptedPlayerPrefs.GetInt("highScoore");
		if (EncryptedPlayerPrefs.CheckEncryption ("highScoore","int",highS.ToString ()) || highS == 0)
		{
			if(highS < score)
			{
				EncryptedPlayerPrefs.SetInt("highScoore", (int)score);
				MainMenu.Instance.SetHighScore (highS);
			}
		}
	}
}
