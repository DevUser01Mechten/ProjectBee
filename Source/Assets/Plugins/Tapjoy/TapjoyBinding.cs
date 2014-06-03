using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;



#if UNITY_IPHONE
public enum TapjoyAdPosition
{
	TopLeft,
	TopCenter,
	TopRight,
	Centered,
	BottomLeft,
	BottomCenter,
	BottomRight
}

public enum TapjoyAdContentSize
{
	Size320x50,
	Size640x100,
	Size768x90
}


public class TapjoyBinding
{
	[DllImport("__Internal")]
	private static extern void _tapjoyInit( string applicationId, string secretKey, bool showDefaultEarnedCurrencyAlert );

	// Initializes the Tapjoy Plugin. showDefaultEarnedCurrencyAlert controls whether the default UIAlertView will be shown or not when tap points are earned.
	public static void init( string applicationId, string secretKey, bool showDefaultEarnedCurrencyAlert = true )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyInit( applicationId, secretKey, showDefaultEarnedCurrencyAlert );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyGetFullScreenAd( string currencyId );

	// Kicks off a request for a full screen ad
	public static void getFullScreenAd()
	{
		getFullScreenAd( null );
	}


	// Kicks off a request for a full screen ad
	public static void getFullScreenAd( string currencyId )
	{
		if( Application.platform != RuntimePlatform.IPhonePlayer )
			return;

		_tapjoyGetFullScreenAd( currencyId );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyShowFullScreenAd();

	// If a full screen ad is ready to be shown, this will present it full screen
	public static void showFullScreenAd()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyShowFullScreenAd();
	}


	[DllImport("__Internal")]
	private static extern void _tapjoySetFullScreenAdDelayCount( int delayCount );

	// Sets the full screen ad delay count
	public static void setFullScreenAdDelayCount( int delayCount )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoySetFullScreenAdDelayCount( delayCount );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoySetAdContentSize( string adContentSize );

	// Sets the size of the ad banner. Must be called before createBanner, if you don't call it an appropriately sized
	// banner will be used based on whether the app is being run on a tablet or phone/touch.
	public static void setAdContentSize( TapjoyAdContentSize adContentSize )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoySetAdContentSize( adContentSize.ToString().Substring( 4 ) );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyCreateBanner( int position, string currencyId, bool shouldRefreshAd );

	// Creates a banner docked to the TapjoyAdPosition with optional specified currencyId
	public static void createBanner( TapjoyAdPosition position )
	{
		createBanner( position, null, true );
	}

	public static void createBanner( TapjoyAdPosition position, bool shouldRefreshAd )
	{
		createBanner( position, null, shouldRefreshAd );
	}

	public static void createBanner( TapjoyAdPosition position, string currencyId, bool shouldRefreshAd )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyCreateBanner( (int)position, currencyId, shouldRefreshAd );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyDestroyBanner();

	// Destroys the banner and removes it from view
	public static void destroyBanner()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyDestroyBanner();
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyRefreshBanner();

	// Refreshes the banner ad
	public static void refreshBanner()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyRefreshBanner();
	}


	[DllImport("__Internal")]
	private static extern void _tapjoySetUserId( string userId );

	// Sets the userId for all Tapjoy methods that utilize it
	public static void setUserId( string userId )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoySetUserId( userId );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyShowOffers();

	// Shows the offers screen
	public static void showOffers()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyShowOffers();
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyShowOffersWithOptions( string currencyId, bool showCurrencySelector );

	// Shows the offers screen with additional options
	public static void showOffers( string currencyId, bool showCurrencySelector )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyShowOffersWithOptions( currencyId, showCurrencySelector );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyGetTapPoints();

	// Gets the available tap points for the current user
	public static void getTapPoints()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyGetTapPoints();
	}


	[DllImport("__Internal")]
	private static extern void _tapjoySpendTapPoints( int points );

	// Updates the virtual currency for the user with the given spent amount of currency.
	public static void spendTapPoints( int points )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoySpendTapPoints( points );
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyAwardTapPoints( int points );

	// Awards the user with the given amount of currency.
	public static void awardTapPoints( int points )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyAwardTapPoints( points );
	}


	[DllImport("__Internal")]
	private static extern bool _tapjoyIsFullScreenAdReady();

	// Checks to see if a full screen ad is ready to be displayed
	public static bool isFullscreenAdReady()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _tapjoyIsFullScreenAdReady();

		return false;
	}


	[DllImport("__Internal")]
	private static extern void _tapjoyActionComplete( string action );

	// Call this when a user completes an in-game action
	public static void actionComplete( string action )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_tapjoyActionComplete( action );
	}

}
#endif