using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prime31;

public class MainMenu : MonoBehaviour 
{
	public Texture2D mainMenuBackground;
	public Texture2D gamelogo;
	public GUIStyle customButtonStyle;
	public static MainMenu Instance { get; private set;}
	public bool displayMainMenu;
	public bool displayGameOver;
	public bool displayMainMenuPaused;
	
	private float screenWidth;
	private float screenHeight;
	private float buttonWidth;
	private float buttonHeight;
	private List<GameCenterLeaderboard> _leaderboards;
	private bool _hasLeaderboardData;
	private bool displayExtraLives;
	
	
	
	void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		
		displayMainMenu = false;
		displayGameOver = false;
		displayMainMenuPaused = false;
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		buttonWidth = screenWidth*0.6f;
		buttonHeight = screenHeight*0.08f;
		
		//GameCenter startup
		GameCenterManager.categoriesLoaded += GetLeaderBoards;
		
		//start iAd
		AdBinding.createAdBanner(true);
		
		//set up encrypted prefs
		// this array should be filled before you can use EncryptedPlayerPrefs :
		EncryptedPlayerPrefs.keys=new string[5];
		EncryptedPlayerPrefs.keys[0]="2xzjgQLP";
		EncryptedPlayerPrefs.keys[1]="GpgZzHrN";
		EncryptedPlayerPrefs.keys[2]="K0SLeYel";
		EncryptedPlayerPrefs.keys[3]="P2JBRziX";
		EncryptedPlayerPrefs.keys[4]="LURkoNgv";
		
		/*
		EncryptedPlayerPrefs.SetString("test_string", "Hello World"
		EncryptedPlayerPrefs.SetInt("test_int", 500);
		EncryptedPlayerPrefs.SetFloat("test_float", 500.456);
		*/
		
		//keep this object in memory
		DontDestroyOnLoad (transform.gameObject);
	}
	
	
	void GetLeaderBoards(List<GameCenterLeaderboard> leaderboards)
	{
		_leaderboards = leaderboards;
		_hasLeaderboardData = _leaderboards != null && _leaderboards.Count > 0;
	}
	
	

	
	void OnGUI()
	{		
		if (displayMainMenu)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,screenWidth,screenHeight),mainMenuBackground,new Rect(0f,0f,5f,5f));
			
			GUI.DrawTexture (new Rect(screenWidth*0.2f,screenHeight*0.1f, screenWidth*0.6f, screenHeight*0.1f), gamelogo);
			
			if (displayMainMenuPaused == false)
			{
				if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.3f, buttonWidth, buttonHeight), "TRY AGAIN", customButtonStyle))
				{
					displayMainMenu = false;
					GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
					//Application.LoadLevel("game");
					Debug.Log ("TRY BUTTON CLICKED!");
				}
			}
			else
			{
				if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.3f, buttonWidth, buttonHeight), "RESUME GAME", customButtonStyle))
				{
					displayMainMenu = false;
					GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
					//Application.LoadLevel("game");
					Debug.Log ("TRY BUTTON CLICKED!");
				}
			}
			
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.4f, buttonWidth, buttonHeight), "LEADERBOARD", customButtonStyle))
			{
				GameCenterBinding.authenticateLocalPlayer();
				GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.5f, buttonWidth, buttonHeight), "EXTRA LIVES", customButtonStyle))
			{
				displayExtraLives = true;
				Debug.Log ("EXTRA LIVES BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.6f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
			{
				Debug.Log ("REMOVE ADS BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.7f, buttonWidth, buttonHeight), "RATE", customButtonStyle))
			{
				Debug.Log ("RATE BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.8f, buttonWidth, buttonHeight), "MORE GAMES", customButtonStyle))
			{
				Debug.Log ("MORE GAMES BUTTON CLICKED!");
			}
		}
		
		
		//EXTRA LIVES
		if (displayExtraLives)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,screenWidth,screenHeight),mainMenuBackground,new Rect(0f,0f,5f,5f));
			
			GUI.DrawTexture (new Rect(screenWidth*0.2f,screenHeight*0.1f, screenWidth*0.6f, screenHeight*0.1f), gamelogo);
			
			GUI.Label(new Rect(0.5f,0.2f,screenWidth*0.5f, screenHeight*0.15f), "BUY EXTRA LIVES");
			
			GUI.Label(new Rect(0.5f,0.6f,screenWidth*0.5f, screenHeight*0.3f), "Survive longer each game by buying the extra lives option. This gives you 3 extra lives each go so you can grab those high scores!");
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.6f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
			{
				Debug.Log ("REMOVE ADS BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.6f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
			{
				Debug.Log ("REMOVE ADS BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.6f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
			{
				Debug.Log ("REMOVE ADS BUTTON CLICKED!");
			}
			
		
		}
		
		
		
	}
	
}
