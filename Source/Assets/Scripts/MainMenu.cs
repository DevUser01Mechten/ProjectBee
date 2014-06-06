using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prime31;

public class MainMenu : MonoBehaviour 
{
	public static MainMenu Instance { get; private set;}
	
	public Texture2D mainMenuBackground;
	public Texture2D gamelogo;
	public GUIStyle customButtonStyle;
	public GUIStyle customHighlightStyle;
	public GUIStyle customPaddedStyle;
	public bool displayMainMenu;
	public bool displayGameOver;
	public bool displayMainMenuPaused;
	
	private float sW;
	private float sH;
	private float buttonWidth;
	private float buttonHeight;
	private bool _hasLeaderboardData;
	private bool displayExtraLives;
	private bool displayRemoveAds;
	private bool displayProblem;
	private StoreKitController storeKitController;
	private List<GameCenterLeaderboard> _leaderboards;
	private bool showAds;
	private bool gameOver;
	private bool isPad;
	
	void Awake()
	{
		//set up encrypted prefs
		// this array should be filled before you can use EncryptedPlayerPrefs :
		EncryptedPlayerPrefs.keys=new string[5];
		EncryptedPlayerPrefs.keys[0]="2xzjgQLP";
		EncryptedPlayerPrefs.keys[1]="GpgZzHrN";
		EncryptedPlayerPrefs.keys[2]="K0SLeYel";
		EncryptedPlayerPrefs.keys[3]="P2JBRziX";
		EncryptedPlayerPrefs.keys[4]="LURkoNgv";
	}
	
	
	void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		
		displayMainMenu = false;
		displayGameOver = false;
		displayMainMenuPaused = false;
		
		sW = Screen.width;
		sH = Screen.height;
		buttonWidth = sW*0.6f;
		buttonHeight = sH/20.0f;
		storeKitController = GetComponent<StoreKitController>();
		
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
		GameCenterBinding.loadLeaderboardTitles();
		
		//Get in-app purchase product data
		storeKitController.RequestProductData();
		
		showAds = true;
		int Ads = EncryptedPlayerPrefs.GetInt("removeadverts");
		if (Ads == 1)
		{
			if (EncryptedPlayerPrefs.CheckEncryption ("removeadverts","int","1"))
			{
				showAds = false;
			}
		}
		else
		{
			//start loading ad
			AdBinding.initializeInterstitial();
			//start iAd
			AdBinding.createAdBanner(true);	
		}
		
		// hack to detect iPad 3 until Unity adds official support
		this.isPad = ( Screen.width >= 1536 || Screen.height >= 1536 );
		if( isPad )
		{
			customButtonStyle.fontSize = 64;
			customHighlightStyle.fontSize = 64;
			customPaddedStyle.fontSize = 64;
		}
		
		//keep this object in memory
		DontDestroyOnLoad (transform.gameObject);
	}
	
	
	void OnEnable()
	{
		//Storekit event handlers
		GameCenterManager.categoriesLoaded += GetLeaderBoards;
		StoreKitManager.purchaseSuccessfulEvent += SuccessfulPurchase;
		StoreKitManager.purchaseFailedEvent += FailedPurchase;
		StoreKitManager.purchaseCancelledEvent += FailedPurchase;
		StoreKitManager.productListRequestFailedEvent += FailedPurchase;
		StoreKitManager.restoreTransactionsFinishedEvent += SuccessfulRestore;
		StoreKitManager.restoreTransactionsFailedEvent += FailedPurchase;
	}
	
	
	void OnDisable()
	{
		//must release event handler prior to object destruction, otherwise memory leaks
		GameCenterManager.categoriesLoaded -= GetLeaderBoards;
		StoreKitManager.purchaseSuccessfulEvent -= SuccessfulPurchase;
		StoreKitManager.purchaseFailedEvent -= FailedPurchase;
		StoreKitManager.purchaseCancelledEvent -= FailedPurchase;
		StoreKitManager.productListRequestFailedEvent -= FailedPurchase;
		StoreKitManager.restoreTransactionsFinishedEvent -= SuccessfulRestore;
		StoreKitManager.restoreTransactionsFailedEvent -= FailedPurchase;
	}
	
	
	void GetLeaderBoards(List<GameCenterLeaderboard> leaderboards)
	{
		_leaderboards = leaderboards;
		_hasLeaderboardData = _leaderboards != null && _leaderboards.Count > 0;
	}
	
	
	void OnGUI()
	{	
		//PAUSE MENU
		if (displayMainMenu)
		{
			gameOver = false;
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.3f, buttonWidth, buttonHeight), "RESUME GAME", customHighlightStyle))
			{
				displayMainMenu = false;
				displayMainMenuPaused = false;
				Camera.main.GetComponent<beeGUI>().dispMenu = true;
				Time.timeScale = 1;
			}
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.4f, buttonWidth, buttonHeight), "LEADERBOARD", customButtonStyle))
			{
				if (GameCenterBinding.isPlayerAuthenticated())
				{
					if (_hasLeaderboardData)
					{
						GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
					}
				}
				else
				{
					GameCenterBinding.authenticateLocalPlayer();
					GameCenterBinding.loadLeaderboardTitles();
				}
			}
			
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.5f, buttonWidth, buttonHeight), "RATE", customHighlightStyle))
			{
				Application.OpenURL ("itms-apps://itunes.apple.com/app/id875911453");
			}
			
			
			if (EncryptedPlayerPrefs.GetInt ("10xtralives") != 1)
			{
				if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.6f, buttonWidth, buttonHeight), "EXTRA LIVES", customButtonStyle))
				{
					displayExtraLives = true;
					displayMainMenu = false;
				}
			}
			
			
			if (EncryptedPlayerPrefs.GetInt ("removeadverts") != 1)
			{
				if (EncryptedPlayerPrefs.GetInt ("10xtralives") != 1)
				{
					if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.7f, buttonWidth, buttonHeight), "REMOVE ADS", customHighlightStyle))
					{
						displayRemoveAds = true;
						displayMainMenu = false;
					}
				}
				else
				{
					if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.6f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
					{
						displayRemoveAds = true;
						displayMainMenu = false;
					}
				}
			}
		}
		
		
		//EXTRA LIVES
		if (displayExtraLives)
		{
			GUI.DrawTextureWithTexCoords(rRect(0f,0f,1f,1f),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (rRect(0.2f,0.1f,0.6f,0.1f), gamelogo);
			
			
			if (EncryptedPlayerPrefs.GetInt("10xtralives") != 1)
			{
				GUI.Label(rRect(0.25f,0.3f,0.5f,0.05f), "BUY EXTRA LIVES", customHighlightStyle);	
				GUI.Label(rRect(0.125f,0.35f,0.75f,0.25f), "Survive longer each game by buying extra life options. You can buy an extra 3 or a whopping 10 extra lives!", customPaddedStyle);
				
				if (GUI.Button(rRect(0.125f,0.6f,0.375f,0.05f), "3 EXTRA LIVES", customHighlightStyle))
				{
					BuyExtraLives ();
				}
				
				if (GUI.Button(rRect(0.5f,0.6f,0.375f,0.05f), "RESTORE", customHighlightStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(rRect(0.125f,0.65f,0.375f,0.05f), "10 EXTRA LIVES", customHighlightStyle))
				{
					Buy10ExtraLives ();
				}
				
				if (GUI.Button(rRect(0.5f,0.65f,0.375f,0.05f), "CANCEL", customHighlightStyle))
				{
					CancelButtonPressed ();
				}
			}
		}
		
		
		//GAME OVER
		if (displayGameOver)
		{
			gameOver = true;
			
			if (showAds && AdBinding.isInterstitalLoaded())
			{
				AdBinding.destroyAdBanner();
				AdBinding.showInterstitial ();
			}
			
			GUI.DrawTextureWithTexCoords(rRect(0f,0f,1f,1f),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (rRect(0.2f,0.1f,0.6f,0.1f), gamelogo);
		
			GUI.Label (rRect(0.3f,0.25f,0.4f,0.05f),"GAME OVER", customHighlightStyle);
			GUI.Label (rRect(0.3f,0.3f,0.4f,0.05f),"SCORE", customButtonStyle);
			GUI.Label (rRect(0.3f,0.35f,0.4f,0.05f), ((int)Camera.main.GetComponent<beeGUI>().score).ToString (), customButtonStyle);
			GUI.Label (rRect(0.3f,0.4f,0.4f,0.05f),"HIGH", customButtonStyle);
			GUI.Label (rRect(0.3f,0.45f,0.4f,0.05f),EncryptedPlayerPrefs.GetInt ("highScoore").ToString (), customButtonStyle);
			
			if (GUI.Button(rRect(0.3f,0.52f,0.4f,0.05f), "RATE", customHighlightStyle))
			{
				Application.OpenURL ("itms-apps://itunes.apple.com/app/id875911453");
			}
			
			if (GUI.Button(rRect(0.3f,0.59f,0.4f,0.05f), "LEADERBOARD", customHighlightStyle))
			{
				if (GameCenterBinding.isPlayerAuthenticated())
				{
					if (_hasLeaderboardData)
					{
						GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
					}
				}
				else
				{
					GameCenterBinding.authenticateLocalPlayer();
					GameCenterBinding.loadLeaderboardTitles();
				}
			}
			
			float yoffset = 0f;
			
			if (EncryptedPlayerPrefs.GetInt ("10xtralives") != 1)
			{
				if (GUI.Button(rRect(0.3f,0.66f,0.4f,0.05f), "EXTRA LIVES", customHighlightStyle))
				{
					displayExtraLives = true;
					displayGameOver = false;
				}
			}
			else
			{
				yoffset -= 0.07f;
			}
			
			if (showAds == true)
			{
				if (GUI.Button(rRect(0.3f,0.73f+yoffset,0.4f,0.05f), "REMOVE ADS", customHighlightStyle))
				{
					displayRemoveAds = true;
					displayGameOver = false;
				}
			}
			else
			{
				yoffset -= 0.07f;
			}
			
			if (GUI.Button(rRect(0.3f,0.80f+yoffset,0.4f,0.05f), "TRY AGAIN", customHighlightStyle))
			{
				displayMainMenu = false;
				displayGameOver = false;
				Camera.main.GetComponent<beeGUI>().dispMenu = true;
				Time.timeScale = 1;
				if (showAds)
				{
					AdBinding.createAdBanner(true);
					AdBinding.initializeInterstitial();
				}
				Application.LoadLevel("game");
			}
		}
		
		
		//REMOVE ADS PURCHASE
		if (displayRemoveAds)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			
			if (EncryptedPlayerPrefs.GetInt("removeadverts") != 1)
			{
				GUI.Label(rRect(0.25f,0.3f,0.5f, 0.05f), "REMOVE ADVERTS", customHighlightStyle);	
				GUI.Label(rRect(0.125f,0.35f,0.75f,0.25f), "Have and advert-free experience by purchasing the option below. Then experience uninterrupted gameplay!",customPaddedStyle);
				
				
				if (GUI.Button(rRect(0.125f, 0.6f, 0.25f, 0.05f), "NO ADS!", customHighlightStyle))
				{
					BuyRemoveAds();
				}
				
				if (GUI.Button(rRect(0.375f, 0.6f, 0.25f, 0.05f), "RESTORE", customHighlightStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(rRect(0.625f, 0.6f, 0.25f, 0.05f), "CANCEL", customHighlightStyle))
				{
					CancelButtonPressed ();
				}
			}
		}
		
		
		//PROBLEM WITH IN-APP PURCHASE
		if (displayProblem)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			GUI.Label(rRect(0.25f,0.3f,0.5f, 0.05f), "PROBLEM", customHighlightStyle);	
			GUI.Label(rRect(0.125f,0.35f,0.75f,0.25f), "There appears to be a problem in doing that action. Please can you try again later?",customPaddedStyle);
			
			
			if (GUI.Button(rRect(0.375f, 0.6f, 0.25f, 0.05f), "HOME", customHighlightStyle))
			{
				if (gameOver)
				{
					displayGameOver = true;
				}
				else
				{
					displayMainMenu = true;
				}
				displayProblem = false;
			}
		}
	}
	
		
	Rect rRect(float x, float y, float w, float h)
	{
		return new Rect(sW*x, sH*y, sW*w, sH*h);
	}
					

	public void SetHighScore(int highScore)
	{
		if (_hasLeaderboardData)
		{
			Debug.Log ("SUBMITTING HIGH SCORE TO LEADERBOARD:" + _leaderboards[0].leaderboardId.ToString ());
			GameCenterBinding.reportScore(highScore, _leaderboards[0].leaderboardId );
		}
	}

	
	// IN-APP PURCHASE LOGIC	
	void RestoreButtonPressed()
	{
		storeKitController.Restore ();
		if (gameOver)
		{
			displayGameOver = true;
		}
		else
		{
			displayMainMenu = true;
		}
		displayExtraLives = false;
		displayRemoveAds = false;
	}
	
	
	void BuyExtraLives()
	{
		if (storeKitController.CanMakePayments ())
		{
			if (!storeKitController.PurchaseExtraLives ())
			{
				PurchaseNotCompleted();
			}
			else
			{
				if (gameOver)
				{
					displayGameOver = true;
				}
				else
				{
					displayMainMenu = true;
				}
				displayExtraLives = false;
			}
		}
		else
		{
			PurchaseNotCompleted();
		}
	}
	
	
	void Buy10ExtraLives()
	{
		if (storeKitController.CanMakePayments ())
		{
			if (!storeKitController.Purchase10ExtraLives ())
			{
				PurchaseNotCompleted();
			}
			else
			{
				if (gameOver)
				{
					displayGameOver = true;
				}
				else
				{
					displayMainMenu = true;
				}
				displayExtraLives = false;
			}
		}
		else
		{
			PurchaseNotCompleted();
		}
	}
	
	
	void BuyRemoveAds()
	{
		if (storeKitController.CanMakePayments ())
		{
			if (!storeKitController.PurchaseRemoveAds ())
			{
				PurchaseNotCompleted();
			}
			else
			{
				if (gameOver)
				{
					displayGameOver = true;
				}
				else
				{
					displayMainMenu = true;
				}
				displayRemoveAds = false;
			}
		}
		else
		{
			PurchaseNotCompleted();
		}
	}
	
	
	void CancelButtonPressed()
	{
		//Return
		if (gameOver)
		{
			displayGameOver = true;
		}
		else
		{
			displayMainMenu = true;
		}
		displayExtraLives = false;
		displayRemoveAds = false;
	}
	
	
	void PurchaseNotCompleted()
	{
		displayProblem = true;
		displayMainMenu = false;
		displayGameOver = false;
		displayExtraLives = false;
		displayRemoveAds = false;
	}
	
	
	//EVENTS
	public void FailedPurchase(string error)
	{
		PurchaseNotCompleted();
	}
	
	
	public void SuccessfulPurchase(StoreKitTransaction transaction)
	{
		if (transaction.productIdentifier == "eu.machten.Bee.extralives")
		{
			EncryptedPlayerPrefs.SetInt ("xtralives",1);
		}
		else if (transaction.productIdentifier == "eu.machten.Bee.10extralives")
		{
			EncryptedPlayerPrefs.SetInt ("10xtralives",1);
		}
		else if (transaction.productIdentifier == "eu.machten.Bee.removeads")
		{
			EncryptedPlayerPrefs.SetInt ("removeadverts",1);
			showAds = false;
			AdBinding.destroyAdBanner ();
		}
	}
	
	
	public void SuccessfulRestore()
	{
		//Do nothing, purchaseSuccessfulEvent will fire and be handled by separate method
	}
}
