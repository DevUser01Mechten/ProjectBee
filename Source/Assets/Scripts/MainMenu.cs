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
	private beeGUI bee;
	
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
		bee = Camera.main.GetComponent<beeGUI>();
		
		//start iAd
		AdBinding.createAdBanner(true);
		
		// always authenticate at every launch
		GameCenterBinding.authenticateLocalPlayer();
		GameCenterBinding.loadLeaderboardTitles();
		
		//Get in-app purchase product data
		storeKitController.RequestProductData();
		
		//start loading ad
		AdBinding.initializeInterstitial();
		
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
	
	
	
	void Update()
	{
	/*
		if (Time.frameCount%50==0)
		{
			//Debug.Log ("displayMainMenu=" + displayMainMenu.ToString ()+"   displayExtraLives="+displayExtraLives.ToString ()+"   displayRemoveAds="+displayRemoveAds.ToString ()+"   displayGameOver="+displayGameOver.ToString()+"   displayMainMenuPaused="+displayMainMenuPaused.ToString());
			Debug.Log (_hasLeaderboardData.ToString ());
			Debug.Log("Leaderboards count="+ _leaderboards.Count);
		}
	*/
	}
	
	
	
	void OnGUI()
	{	
		//MAIN MENU
		if (displayMainMenu)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			if (displayMainMenuPaused == false)
			{
				if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.3f, buttonWidth, buttonHeight), "TRY AGAIN", customButtonStyle))
				{
					//show intersitial ad
					if (AdBinding.isInterstitalLoaded())
					{
						AdBinding.showInterstitial ();
						AdBinding.initializeInterstitial();
					}
					displayMainMenu = false;
					Camera.main.GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
					Application.LoadLevel("game");
					bee = Camera.main.GetComponent<beeGUI>();
				}
			}
			else
			{
				if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.3f, buttonWidth, buttonHeight), "RESUME GAME", customHighlightStyle))
				{
					displayMainMenu = false;
					displayMainMenuPaused = false;
					Camera.main.GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
				}
			}
			
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.4f, buttonWidth, buttonHeight), "LEADERBOARD", customButtonStyle))
			{
				if (_hasLeaderboardData)
				{
					GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
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
			
			
			if (EncryptedPlayerPrefs.GetInt("xtralives") != 1)
			{
				GUI.Label(rRect(0.25f,0.3f,0.5f,0.05f), "BUY EXTRA LIVES", customHighlightStyle);	
				GUI.Label(rRect(0.125f,0.4f,0.75f,0.25f), "Survive longer each game by buying an extra life option. This gives you an extra 3 or a whopping 10 extra lives each go!", customPaddedStyle);
				
				if (GUI.Button(rRect(0.125f,0.7f,0.375f,0.05f), "BUY 3 EXTRA LIVES", customHighlightStyle))
				{
					BuyExtraLives ();
				}
				
				if (GUI.Button(rRect(0.5f,0.7f,0.375f,0.05f), "BUY 10 EXTRA LIVES", customHighlightStyle))
				{
					Buy10ExtraLives ();
				}
				
				if (GUI.Button(rRect(0.125f,0.78f,0.375f,0.05f), "RESTORE", customHighlightStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(rRect(0.5f,0.78f,0.375f,0.05f), "CANCEL", customHighlightStyle))
				{
					CancelButtonPressed ();
				}
			}
		}
		
		
		//GAME OVER
		if (displayGameOver)
		{
			GUI.DrawTextureWithTexCoords(rRect(0f,0f,1f,1f),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (rRect(0.2f,0.1f,0.6f,0.1f), gamelogo);
		
			GUI.Label (rRect(0.3f,0.35f,0.4f,0.05f),"GAME OVER", customHighlightStyle);
			GUI.Label (rRect(0.3f,0.4f,0.4f,0.05f),"SCORE", customButtonStyle);
			GUI.Label (rRect(0.3f,0.45f,0.4f,0.05f), ((int)Camera.main.GetComponent<beeGUI>().score).ToString (), customButtonStyle);
			GUI.Label (rRect(0.3f,0.5f,0.4f,0.05f),"HIGH", customButtonStyle);
			GUI.Label (rRect(0.3f,0.55f,0.4f,0.05f),EncryptedPlayerPrefs.GetInt ("highScoore").ToString (), customButtonStyle);
		
			if (GUI.Button(rRect(0.3f,0.65f,0.4f,0.05f), "RATE", customHighlightStyle))
			{
				Application.OpenURL ("itms-apps://itunes.apple.com/app/id875911453");
			}
			
			if (GUI.Button(rRect(0.3f,0.72f,0.4f,0.05f), "LEADERBOARD", customHighlightStyle))
			{
				if (_hasLeaderboardData)
				{
					GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
				}
			}
			
			if (GUI.Button(rRect(0.3f,0.79f,0.4f,0.05f), "TRY AGAIN", customHighlightStyle))
			{
				displayMainMenu = false;
				displayGameOver = false;
				Camera.main.GetComponent<beeGUI>().dispMenu = true;
				Time.timeScale = 1;
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
				GUI.Label(rRect(0.125f,0.4f,0.75f,0.25f), "Have and advert-free experience by purchasing the option below. Then experience uninterrupted gameplay!",customPaddedStyle);
				
				
				if (GUI.Button(rRect(0.125f, 0.7f, 0.25f, 0.05f), "REMOVE ADS", customHighlightStyle))
				{
					BuyRemoveAds();
				}
				
				if (GUI.Button(rRect(0.375f, 0.7f, 0.25f, 0.05f), "RESTORE", customHighlightStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(rRect(0.625f, 0.7f, 0.25f, 0.05f), "CANCEL", customHighlightStyle))
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
			GUI.Label(new Rect(sW*0.25f,sH*0.4f,sW*0.5f, sH*0.2f), "Sorry, there appears to be a problem. Please can you try again later?");
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.5f, buttonWidth, buttonHeight), "BACK", customButtonStyle))
			{
				displayMainMenu = true;
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
			GameCenterBinding.reportScore(highScore, _leaderboards[0].leaderboardId );
		}
	}

	
	// IN-APP PURCHASE LOGIC	
	void RestoreButtonPressed()
	{
		storeKitController.Restore ();
		displayMainMenu = true;
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
				displayMainMenu = true;
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
				displayMainMenu = true;
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
				displayMainMenu = true;
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
		displayMainMenu = true;
		displayExtraLives = false;
		displayRemoveAds = false;
	}
	
	
	void PurchaseNotCompleted()
	{
		displayProblem = true;
		displayMainMenu = false;
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
		}
	}
	
	
	public void SuccessfulRestore()
	{
		//Do nothing, purchaseSuccessfulEvent will fire and be handled by separate method
	}
}
