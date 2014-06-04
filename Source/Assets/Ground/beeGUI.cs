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
		lableH = screenH / 15;
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
			score += (Time.deltaTime* 10);
	}

	void OnGUI()
	{
		if(dispMenu)
		{
			GUI.Label (new Rect (screenW - (lableW * 1.5f), lableH * 1.5f, lableW, lableH), "Score: " + (int)score, myStyle);
			GUI.Label (new Rect (lableW * 1.5f, lableH * 1.5f, lableW, lableH), "Lives: " + numLifes, myStyle);
			//menu
			if (GUI.Button (new Rect (0f, 0f, lableW, lableH), "MENU", myButtonStyle))
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
		if(highS < score)
			EncryptedPlayerPrefs.SetInt("highScoore", (int)score);
	}
}
