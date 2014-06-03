//
//  TapjoyManager.m
//  TapjoyTest
//
//  Created by Mike on 1/28/11.
//  Copyright 2011 Prime31 Studios. All rights reserved.
//

#import "TapjoyManager.h"


void UnitySendMessage( const char * className, const char * methodName, const char * param );

void UnityPause( bool shouldPause );

UIViewController *UnityGetGLViewController();



@implementation TapjoyManager

@synthesize adView, bannerPosition, adContentSize, publisherId, secretKey, showDefaultEarnedCurrencyAlert;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (TapjoyManager*)sharedManager
{
	static TapjoyManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[TapjoyManager alloc] init];
	
	return sharedSingleton;
}


- (id)init
{
	if( (self = [super init]) )
	{
		// set the ad size
		// Must return one of TJC_AD_BANNERSIZE_320X50 or TJC_AD_BANNERSIZE_480X32 or TJC_AD_BANNERSIZE_768X90
		if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
			self.adContentSize = TJC_DISPLAY_AD_SIZE_768X90;
		else
			self.adContentSize = TJC_DISPLAY_AD_SIZE_320X50;
		
		self._shouldRefreshAd = YES;
	}
	return self;
}


+ (id)objectFromJson:(NSString*)json
{
	NSError *error = nil;
	NSData *data = [NSData dataWithBytes:json.UTF8String length:json.length];
    NSObject *object = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingAllowFragments error:&error];
	
	if( error )
		NSLog( @"failed to deserialize JSON: %@ with error: %@", json, [error localizedDescription] );

    return object;
}


+ (NSString*)jsonFromObject:(id)object
{
	NSError *error = nil;
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object options:0 error:&error];
	
	if( jsonData && !error )
		return [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
	else
		NSLog( @"jsonData was null, error: %@", [error localizedDescription] );

    return @"{}";
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)adjustAdViewFrameToShowAdView
{
	// fetch screen dimensions and useful values
	CGRect origFrame = self.adView.frame;
	
	CGFloat screenHeight = [UIScreen mainScreen].bounds.size.height;
	CGFloat screenWidth = [UIScreen mainScreen].bounds.size.width;
	
	if( UIInterfaceOrientationIsLandscape( UnityGetGLViewController().interfaceOrientation ) )
	{
		screenWidth = screenHeight;
		screenHeight = [UIScreen mainScreen].bounds.size.width;
	}
	
	
	switch( bannerPosition )
	{
		case TapjoyAdPositionTopLeft:
			origFrame.origin.x = 0;
			origFrame.origin.y = 0;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin );
			break;
		case TapjoyAdPositionTopCenter:
			origFrame.origin.x = ( screenWidth / 2 ) - ( origFrame.size.width / 2 );
			origFrame.origin.y = 0;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin );
			break;
		case TapjoyAdPositionTopRight:
			origFrame.origin.x = screenWidth - origFrame.size.width;
			origFrame.origin.y = 0;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleBottomMargin );
			break;
		case TapjoyAdPositionCentered:
			origFrame.origin.x = ( screenWidth / 2 ) - ( origFrame.size.width / 2 );
			origFrame.origin.y = ( screenHeight / 2 ) - ( origFrame.size.height / 2 );
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleBottomMargin );
			break;
		case TapjoyAdPositionBottomLeft:
			origFrame.origin.x = 0;
			origFrame.origin.y = screenHeight - origFrame.size.height;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin );
			break;
		case TapjoyAdPositionBottomCenter:
			origFrame.origin.x = ( screenWidth / 2 ) - ( origFrame.size.width / 2 );
			origFrame.origin.y = screenHeight - origFrame.size.height;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin );
			break;
		case TapjoyAdPositionBottomRight:
			origFrame.origin.x = screenWidth - self.adView.frame.size.width;
			origFrame.origin.y = screenHeight - origFrame.size.height;
			self.adView.autoresizingMask = ( UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin );
			break;
	}
	
	self.adView.frame = origFrame;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)startWithAppId:(NSString*)appId
{
	// default notifications
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(tapjoyConnectionSuccessful:) name:TJC_CONNECT_SUCCESS object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(tapjoyConnectionFailed:) name:TJC_CONNECT_FAILED object:nil];
	
	// TapPoint spending notifications
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(spentTapPoints:) name:TJC_SPEND_TAP_POINTS_RESPONSE_NOTIFICATION object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(spentTapPointsFailed:) name:TJC_SPEND_TAP_POINTS_RESPONSE_NOTIFICATION_ERROR object:nil];

	// TJC_VIEW_CLOSED_NOTIFICATION and TJC_TAPPOINTS_EARNED_NOTIFICATION
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(viewClosedNotification:) name:TJC_VIEW_CLOSED_NOTIFICATION object:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(tappointsEarnedNotification:) name:TJC_TAPPOINTS_EARNED_NOTIFICATION object:nil];
	
	// connect to Tapjoy
	[Tapjoy requestTapjoyConnect:appId
					   secretKey:self.secretKey
						 options:@{ TJC_OPTION_ENABLE_LOGGING : @(YES) }];
	
	// featured app
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(receivedFullScreenAd:) name:TJC_FULL_SCREEN_AD_RESPONSE_NOTIFICATION object:nil];
	[Tapjoy getFullScreenAd];
	
	[Tapjoy setViewDelegate:self];
}


- (void)getFullScreenAdWithCurrencyId:(NSString*)currencyId
{
	if( currencyId )
		[Tapjoy getFullScreenAdWithCurrencyID:currencyId];
	else
		[Tapjoy getFullScreenAd];
}


- (void)showFullScreenAd
{
	if( !_isFeaturedAppReady )
	{
		NSLog( @"full screen ad is not yet ready" );
		return;
	}
	
	UnityPause( true );
	[Tapjoy showFullScreenAdWithViewController:UnityGetGLViewController()];
}


- (void)setFullScreenAdDelayCount:(int)delayCount
{
	[Tapjoy setFullScreenAdDelayCount:delayCount];
}


- (void)createBannerWithPosition:(TapjoyAdPosition)position currencyId:(NSString*)currencyID
{
	// kill the current adView if we have one
	if( self.adView )
		[self destroyBanner];
	
	bannerPosition = position;
	
	if( currencyID )
		[Tapjoy getDisplayAdWithDelegate:self currencyID:currencyID];
	else
		[Tapjoy getDisplayAdWithDelegate:self];
}


- (void)destroyBanner
{
	[Tapjoy getDisplayAdWithDelegate:nil];
	
	if( self.adView )
	{
		[self.adView removeFromSuperview];
		self.adView = nil;
	}
}


- (void)refreshBanner
{
	[Tapjoy getDisplayAdWithDelegate:self];
	[self adjustAdViewFrameToShowAdView];
}


- (void)showOffers
{
	UnityPause( true );
	[Tapjoy showOffersWithViewController:UnityGetGLViewController()];
}


- (void)showOffersWithCurrencyID:(NSString*)currencyId withCurrencySelector:(BOOL)isSelectorVisible
{
	UnityPause( true );
	[Tapjoy showOffersWithCurrencyID:currencyId withViewController:UnityGetGLViewController() withCurrencySelector:isSelectorVisible];
}


- (void)getTapPoints
{
	[Tapjoy getTapPointsWithCompletion:^( NSDictionary *parameters, NSError *error )
	{
		if( error )
		{
			UnitySendMessage( "TapjoyManager", "receivedTapPointsFailed", error.localizedDescription.UTF8String );
		}
		else
		{
			UnitySendMessage( "TapjoyManager", "tapPointsReceived", [TapjoyManager jsonFromObject:parameters].UTF8String );
		}
	}];
}


- (void)spendTapPoints:(int)points
{
	[Tapjoy spendTapPoints:points];
}


- (void)awardTapPoints:(int)points
{
	[Tapjoy awardTapPoints:points];
}


- (BOOL)isFeaturedAppReady
{
	return _isFeaturedAppReady;
}


- (void)actionComplete:(NSString*)action
{
	[Tapjoy actionComplete:action];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSNotifications

- (void)tapjoyConnectionSuccessful:(NSNotification*)note
{
	NSLog( @"tapjoyConnectionSuccessful" );
}


- (void)tapjoyConnectionFailed:(NSNotification*)note
{
	NSLog( @"tapjoyConnectionFailed" );
}


// This notification is fired after spendTapPoints has been called, and indicates that the user has successfully spent currency.
- (void)spentTapPoints:(NSNotification*)note
{
	NSNumber *tapPoints = note.object;
	UnitySendMessage( "TapjoyManager", "spentTapPoints", [[NSString stringWithFormat:@"%i", [tapPoints intValue]] UTF8String] );
}


// Error notification for spendTapPoints
- (void)spentTapPointsFailed:(NSNotification*)note
{
	UnitySendMessage( "TapjoyManager", "spentTapPointsFailed", "" );
}


- (void)viewClosedNotification:(NSNotification*)note
{
	UnityPause( false );
	UnitySendMessage( "TapjoyManager", "viewClosed", "" );
}


- (void)tappointsEarnedNotification:(NSNotification*)note
{
	NSNumber *tappointsEarned = note.object;
	NSString *tappointsString = [NSString stringWithFormat:@"%i", [tappointsEarned intValue]];
	
	// Pops up a UIAlert notifying the user that they have successfully earned some currency.
	if( showDefaultEarnedCurrencyAlert )
		[Tapjoy showDefaultEarnedCurrencyAlert];
	
	UnitySendMessage( "TapjoyManager", "tappointsEarned", tappointsString.UTF8String );
}


- (void)receivedFullScreenAd:(NSNotification*)note
{
	UnitySendMessage( "TapjoyManager", "fullScreenAdDidLoad", "" );
	
	// Show the custom Tapjoy full screen full screen ad view.
	_isFeaturedAppReady = YES;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark TJCAdDelegate

- (void)didReceiveAd:(TJCAdView*)adView
{
	if( !self.adView )
	{
		self.adView = (UIView*)[Tapjoy getDisplayAdView];
		
		if( [adContentSize isEqual:TJC_DISPLAY_AD_SIZE_320X50] )
			self.adView.frame = CGRectMake( 0, 0, 320, 50 );
		else if( [adContentSize isEqual:TJC_DISPLAY_AD_SIZE_640X100] )
			self.adView.frame = CGRectMake( 0, 0, 640, 100 );
		else if( [adContentSize isEqual:TJC_DISPLAY_AD_SIZE_768X90] )
			self.adView.frame = CGRectMake( 0, 0, 768, 90 );
		else
			self.adView.frame = CGRectMake( 0, 0, 320, 50 );
		
		
		[self adjustAdViewFrameToShowAdView];
		[UnityGetGLViewController().view addSubview:self.adView];
	}
	
	[self adjustAdViewFrameToShowAdView];
	UnitySendMessage( "TapjoyManager", "didReceiveAd", "" );
}


- (void)didFailWithMessage:(NSString*)msg
{
	UnitySendMessage( "TapjoyManager", "didFailToReceiveAd", msg.UTF8String );
}


- (NSString*)adContentSize
{
	return adContentSize;
}


- (BOOL)shouldRefreshAd
{
	return self._shouldRefreshAd;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark TJCVideoAdDelegate

- (void)videoAdBegan
{
	UnityPause( true );
	UnitySendMessage( "TapjoyManager", "videoAdBegan", "" );
}


- (void)videoAdClosed
{
	UnitySendMessage( "TapjoyManager", "videoAdClosed", "" );
}


- (void)videoAdError:(NSString*)errorMsg
{
	NSLog( @"videoAdError: %@", errorMsg );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark - TJCViewDelegate

- (void)viewWillAppearWithType:(int)viewType
{
	//NSString *type = [NSString stringWithFormat:@"%i", viewType];
	//UnitySendMessage( "TapjoyManager", "viewWillAppearWithType", type.UTF8String );
}


- (void)viewWillDisappearWithType:(int)viewType
{
	//NSString *type = [NSString stringWithFormat:@"%i", viewType];
	//UnitySendMessage( "TapjoyManager", "viewWillDisappearWithType", type.UTF8String );
	[Tapjoy getTapPoints];
}


@end
