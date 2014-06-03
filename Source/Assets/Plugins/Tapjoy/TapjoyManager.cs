using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


// Any methods that Obj-C calls back using UnitySendMessage should be present here
public class TapjoyManager : AbstractManager
{
#if UNITY_IPHONE
	// Fired when the featured app finishes loading.  Includes information about the featured app.
	public static event Action fullscreenAdDidLoadEvent;
	
	// Fired when the users tap points are retrieved from the server. Includes the amount, currencyId and currencyName in that order.
	public static event Action<int,string,string> tapPointsReceivedEvent;
	
	// Fired when the users tap points fail to be retrieved from the server
	public static event Action receiveTapPointsFailedEvent;
	
	// Fired after spendTapPoints has been called, and indicates that the user has successfully spent currency.
	public static event Action<int> tapPointsSpentEvent;
	
	// Fired when spendTapPoints fails
	public static event Action spendTapPointsFailedEvent;
	
	// Fired when a video ad begins playing
	public static event Action videoAdBeganEvent;
	
	// Fired when a video ad has completed playing
	public static event Action videoAdClosedEvent;
	
	// Fired when a view is closed
	public static event Action viewClosedEvent;
	
	// Fired when tappoints are earned and includes how many
	public static event Action<int> tappointsEarnedEvent;
	
	// Fired when an ad is received
	public static event Action didReceiveAdEvent;
	
	// Fired when an ad fails to be received with the error message
	public static event Action<string> didFailToReceiveAdEvent;

	

    static TapjoyManager()
    {
		AbstractManager.initialize( typeof( TapjoyManager ) );
    }
	
	
	public void fullScreenAdDidLoad( string empty )
	{
		if( fullscreenAdDidLoadEvent != null )
		{
			fullscreenAdDidLoadEvent();
		}
	}
	
	
	public void tapPointsReceived( string json )
	{
		if( tapPointsReceivedEvent != null )
		{
			var dict = json.dictionaryFromJson();
			tapPointsReceivedEvent( int.Parse( dict["amount"].ToString() ), dict["currencyID"].ToString(), dict["currencyName"].ToString() );
		}
	}
	
	
	public void receivedTapPointsFailed( string empty )
	{
		if( receiveTapPointsFailedEvent != null )
			receiveTapPointsFailedEvent();
	}
	
	
	public void spentTapPoints( string remainingTapPoints )
	{
		if( tapPointsSpentEvent != null )
			tapPointsSpentEvent( int.Parse( remainingTapPoints ) );
	}
	
	
	public void spentTapPointsFailed( string empty )
	{
		if( spendTapPointsFailedEvent != null )
			spendTapPointsFailedEvent();
	}
	

	public void videoAdBegan( string empty )
	{
		if( videoAdBeganEvent != null )
			videoAdBeganEvent();
	}


	public void videoAdClosed( string empty )
	{
		if( videoAdClosedEvent != null )
			videoAdClosedEvent();
	}
	
	
	public void viewClosed( string empty )
	{
		if( viewClosedEvent != null )
			viewClosedEvent();
	}
	
	
	public void tappointsEarned( string tappoints )
	{
		if( tappointsEarnedEvent != null )
			tappointsEarnedEvent( int.Parse( tappoints ) );
	}
	
	
	public void didReceiveAd( string empty )
	{
		if( didReceiveAdEvent != null )
			didReceiveAdEvent();
	}
	
	
	public void didFailToReceiveAd( string error )
	{
		if( didFailToReceiveAdEvent != null )
			didFailToReceiveAdEvent( error );
	}
	
#endif
}