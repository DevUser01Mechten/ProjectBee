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
		buttonHeight = sH*0.08f;
		storeKitController = GetComponent<StoreKitController>();
		
		//start iAd
		AdBinding.createAdBanner(true);
		
		//Get in-app purchase product data
		storeKitController.RequestProductData();
		
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
		if (Time.frameCount%50==0)
		{
			Debug.Log ("displayMainMenu=" + displayMainMenu.ToString ()+"   displayExtraLives="+displayExtraLives.ToString ()+"   displayRemoveAds="+displayRemoveAds.ToString ()+"   displayGameOver="+displayGameOver.ToString()+"   displayMainMenuPaused="+displayMainMenuPaused.ToString());
		}
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
					displayMainMenu = false;
					Camera.main.GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
					Application.LoadLevel("game");
					Debug.Log ("TRY BUTTON CLICKED!");
				}
			}
			else
			{
				if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.3f, buttonWidth, buttonHeight), "RESUME GAME", customButtonStyle))
				{
					displayMainMenu = false;
					Camera.main.GetComponent<beeGUI>().dispMenu = true;
					Time.timeScale = 1;
					Debug.Log ("TRY BUTTON CLICKED!");
				}
			}
			
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.4f, buttonWidth, buttonHeight), "LEADERBOARD", customButtonStyle))
			{
				if (_hasLeaderboardData)
				{
					GameCenterBinding.authenticateLocalPlayer();
					GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
				}
			}
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.5f, buttonWidth, buttonHeight), "EXTRA LIVES", customButtonStyle))
			{
				displayExtraLives = true;
				displayMainMenu = false;
				displayGameOver = false;
			}
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.6f, buttonWidth, buttonHeight), "RATE", customButtonStyle))
			{
				Debug.Log ("RATE BUTTON CLICKED!");
			}
			
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.7f, buttonWidth, buttonHeight), "MORE GAMES", customButtonStyle))
			{
				Debug.Log ("MORE GAMES BUTTON CLICKED!");
			}
			
			if (EncryptedPlayerPrefs.GetInt ("removeadverts") != 1)
			{
				if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.8f, buttonWidth, buttonHeight), "REMOVE ADS", customButtonStyle))
				{
					displayRemoveAds = true;
					displayMainMenu = false;
				}
			}
		}
		
		
		//EXTRA LIVES
		if (displayExtraLives)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			
			if (EncryptedPlayerPrefs.GetInt("xtralives") != 1)
			{
				GUI.Label(new Rect(sW*0.25f,sH*0.3f,sW*0.5f, sH*0.1f), "BUY EXTRA LIVES", customButtonStyle);	
				GUI.Label(new Rect(sW*0.125f,sH*0.4f,sW*0.75f, sH*0.3f), "Survive longer each game by buying the extra lives option. This gives you 3 extra lives each go so you can grab those high scores!", customButtonStyle);
				
				if (GUI.Button(new Rect(sW*0.25f, sH*0.8f, buttonWidth*0.4f, buttonHeight*0.4f), "BUY 3 EXTRA LIVES", customButtonStyle))
				{
					BuyExtraLives ();
				}
				
				if (GUI.Button(new Rect(sW*0.75f, sH*0.8f, buttonWidth*0.4f, buttonHeight*0.4f), "BUY 10 EXTRA LIVES", customButtonStyle))
				{
					Buy10ExtraLives ();
				}
				
				if (GUI.Button(new Rect(sW*0.25f, sH*0.9f, buttonWidth*0.4f, buttonHeight*0.4f), "RESTORE", customButtonStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(new Rect(sW*0.75f, sH*0.9f, buttonWidth*0.4f, buttonHeight*0.4f), "CANCEL", customButtonStyle))
				{
					CancelButtonPressed ();
				}
			}
			else
			{
				GUI.Label(new Rect(sW*0.25f,sH*0.4f,sW*0.5f, sH*0.2f), "You have already bought the extra lives option!");
				
				if (GUI.Button(new Rect((sW-buttonWidth*0.6f)/2, sH*0.5f, buttonWidth*0.6f, buttonHeight*0.6f), "BACK", customButtonStyle))
				{
					displayExtraLives = false;
					displayMainMenu = true;
				}
			}
		}
		
		
		//GAME OVER
		if (displayGameOver)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
		
			if (GUI.Button(new Rect((sW-buttonWidth)/2, sH*0.5f, buttonWidth, buttonHeight), "BACK", customButtonStyle))
			{
				displayMainMenu = true;
				displayGameOver = false;
			}
		}
		
		
		//REMOVE ADS PURCHASE
		if (displayRemoveAds)
		{
			GUI.DrawTextureWithTexCoords(new Rect(0f,0f,sW,sH),mainMenuBackground,new Rect(0f,0f,5f,5f));
			GUI.DrawTexture (new Rect(sW*0.2f,sH*0.1f, sW*0.6f, sH*0.1f), gamelogo);
			
			
			if (EncryptedPlayerPrefs.GetInt("removeadverts") != 1)
			{
				GUI.Label(new Rect(0.25f,0.3f,sW*0.5f, sH*0.1f), "REMOVE ADVERTS");	
				GUI.Label(new Rect(0.25f,0.4f,sW*0.5f, sH*0.4f), "Have and advert-free experience by purchasing this option");
				
				
				if (GUI.Button(new Rect(sW*0.25f, sH*0.8f, buttonWidth*0.4f, buttonHeight*0.4f), "BUY REMOVE ADS", customButtonStyle))
				{
					BuyRemoveAds();
				}
				
				if (GUI.Button(new Rect(sW*0.5f, sH*0.8f, buttonWidth*0.4f, buttonHeight*0.4f), "RESTORE", customButtonStyle))
				{
					RestoreButtonPressed ();
				}
				
				if (GUI.Button(new Rect(sW*0.75f, sH*0.8f, buttonWidth*0.4f, buttonHeight*0.4f), "CANCEL", customButtonStyle))
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
