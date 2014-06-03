using UnityEngine;
using System.Collections;


public class TapjoyEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void Start()
	{
		// Listens to all the Tapjoy events.  All event listeners MUST be removed before this object is disposed!
		TapjoyManager.fullscreenAdDidLoadEvent += fullscreenAdDidLoadEvent;
		TapjoyManager.tapPointsReceivedEvent += tapPointsReceived;
		TapjoyManager.receiveTapPointsFailedEvent += receiveTapPointsFailed;
		TapjoyManager.tapPointsSpentEvent += tapPointsSpent;
		TapjoyManager.spendTapPointsFailedEvent += spendTapPointsFailed;
		TapjoyManager.videoAdBeganEvent += videoAdBeganEvent;
		TapjoyManager.videoAdClosedEvent += videoAdClosedEvent;
		TapjoyManager.viewClosedEvent += viewClosedEvent;
		TapjoyManager.tappointsEarnedEvent += tappointsEarnedEvent;
		TapjoyManager.didReceiveAdEvent += didReceiveAdEvent;
		TapjoyManager.didFailToReceiveAdEvent += didFailToReceiveAdEvent;
	}

		
	void OnDisable()
	{
		// Remove all the event handlers
		TapjoyManager.fullscreenAdDidLoadEvent -= fullscreenAdDidLoadEvent;
		TapjoyManager.tapPointsReceivedEvent -= tapPointsReceived;
		TapjoyManager.receiveTapPointsFailedEvent -= receiveTapPointsFailed;
		TapjoyManager.tapPointsSpentEvent -= tapPointsSpent;
		TapjoyManager.spendTapPointsFailedEvent -= spendTapPointsFailed;
		TapjoyManager.videoAdBeganEvent -= videoAdBeganEvent;
		TapjoyManager.videoAdClosedEvent -= videoAdClosedEvent;
		TapjoyManager.viewClosedEvent -= viewClosedEvent;
		TapjoyManager.tappointsEarnedEvent -= tappointsEarnedEvent;
		TapjoyManager.didReceiveAdEvent -= didReceiveAdEvent;
		TapjoyManager.didFailToReceiveAdEvent -= didFailToReceiveAdEvent;
	}
	
	
	

	private void fullscreenAdDidLoadEvent()
	{
		Debug.Log( "fullscreenAdDidLoadEvent" );
	}
	
		
	private void tapPointsReceived( int totalPoints, string currencyId, string currencyName )
	{
		Debug.Log( string.Format( "tapPointsReceived: {0}, currencyId: {1}, currencyName: {2}", totalPoints, currencyId, currencyName ) );
	}
	
	
	private void receiveTapPointsFailed()
	{
		Debug.Log( "receiveTapPointsFailed" );
	}
	
	
	private void tapPointsSpent( int totalPoints )
	{
		Debug.Log( "tapPointsSpent.  total remaining: " + totalPoints );
	}
	
	
	private void spendTapPointsFailed()
	{
		Debug.Log( "spendTapPointsFailed" );
	}

	
	private void videoAdBeganEvent()
	{
		Debug.Log( "videoAdBeganEvent" );
	}
	
	
	private void videoAdClosedEvent()
	{
		Debug.Log( "videoAdClosed" );
	}
	
	
	private void viewClosedEvent()
	{
		Debug.Log( "viewClosedEvent" );
	}
	
	
	private void tappointsEarnedEvent( int tappoints )
	{
		Debug.Log( "tappointsEarnedEvent: " + tappoints );
	}
	
	
	private void didReceiveAdEvent()
	{
		Debug.Log( "didReceiveAdEvent" );
		
	}
	
	
	private void didFailToReceiveAdEvent( string error )
	{
		Debug.Log( "didFailToReceiveAdEvent: " + error );
	}
	
#endif
}
