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
	
	private float screenWidth;
	private float screenHeight;
	private float buttonWidth;
	private float buttonHeight;
	private List<GameCenterLeaderboard> _leaderboards;
	private bool _hasLeaderboardData;
	
	
	
	void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		buttonWidth = screenWidth*0.6f;
		buttonHeight = screenHeight*0.08f;
		
		//GameCenter startup
		GameCenterManager.categoriesLoaded += GetLeaderBoards;
				
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
		
		//start iAd
		AdBinding.createAdBanner(true);
		
		DontDestroyOnLoad (transform.gameObject);
	}
	
	
	void GetLeaderBoards(List<GameCenterLeaderboard> leaderboards)
	{
		_leaderboards = leaderboards;
		_hasLeaderboardData = _leaderboards != null && _leaderboards.Count > 0;
	}
	
	
	
	
	
	
	void OnGUI()
	{		
		GUI.DrawTextureWithTexCoords(new Rect(0f,0f,screenWidth,screenHeight),mainMenuBackground,new Rect(0f,0f,5f,5f));
		
		GUI.DrawTexture (new Rect(screenWidth*0.2f,screenHeight*0.1f, screenWidth*0.6f, screenHeight*0.1f), gamelogo);
	
	
		if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.3f, buttonWidth, buttonHeight), "TRY AGAIN", customButtonStyle))
		{
			Debug.Log ("TRY BUTTON CLICKED!");
		}
			
		if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.4f, buttonWidth, buttonHeight), "LEADERBOARD", customButtonStyle))
		{
			GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
		}
		
		if (GUI.Button(new Rect((screenWidth-buttonWidth)/2, screenHeight*0.5f, buttonWidth, buttonHeight), "EXTRA LIVES", customButtonStyle))
		{
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
	
}
