//
//  TapjoyBinding.m
//  TapjoyTest
//
//  Created by Mike on 1/28/11.
//  Copyright 2011 Prime31 Studios. All rights reserved.
//

#import "TapjoyManager.h"


// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL


// Initializes the Tapjoy Plugin
void _tapjoyInit( const char * applicationId, const char * secretKey, bool showDefaultEarnedCurrencyAlert )
{
	[TapjoyManager sharedManager].showDefaultEarnedCurrencyAlert = showDefaultEarnedCurrencyAlert;
	[TapjoyManager sharedManager].secretKey = GetStringParam( secretKey );
	[[TapjoyManager sharedManager] startWithAppId:GetStringParam( applicationId )];
}


void _tapjoyGetFullScreenAd( const char * currencyId )
{
	[[TapjoyManager sharedManager] getFullScreenAdWithCurrencyId:GetStringParamOrNil( currencyId )];
}


void _tapjoyShowFullScreenAd()
{
	[[TapjoyManager sharedManager] showFullScreenAd];
}


void _tapjoySetFullScreenAdDelayCount( int delayCount )
{
	[[TapjoyManager sharedManager] setFullScreenAdDelayCount:delayCount];
}


void _tapjoySetAdContentSize( const char * adContentSize )
{
	[TapjoyManager sharedManager].adContentSize = GetStringParam( adContentSize );
}


void _tapjoyCreateBanner( int position, const char * currencyId, BOOL shouldRefreshAd )
{
	TapjoyAdPosition bannerPosition = (TapjoyAdPosition)position;
	[[TapjoyManager sharedManager] createBannerWithPosition:bannerPosition currencyId:GetStringParamOrNil( currencyId )];
	[TapjoyManager sharedManager]._shouldRefreshAd = shouldRefreshAd;
}


// Destroys the banner and removes it from view
void _tapjoyDestroyBanner()
{
	[[TapjoyManager sharedManager] destroyBanner];
}


// Refreshes the banner ad
void _tapjoyRefreshBanner()
{
	[[TapjoyManager sharedManager] refreshBanner];
}


void _tapjoySetUserId( const char * userId )
{
	[Tapjoy setUserID:GetStringParam( userId )];
}


// Shows the offers screen
void _tapjoyShowOffers()
{
	[[TapjoyManager sharedManager] showOffers];
}


void _tapjoyShowOffersWithOptions( const char * currencyId, BOOL showCurrencySelector )
{
	[[TapjoyManager sharedManager] showOffersWithCurrencyID:GetStringParam( currencyId ) withCurrencySelector:showCurrencySelector];
}


// Gets the available tap points for the current user
void _tapjoyGetTapPoints()
{
	[[TapjoyManager sharedManager] getTapPoints];
}


// Updates the virtual currency for the user with the given spent amount of currency.
void _tapjoySpendTapPoints( int points )
{
	[[TapjoyManager sharedManager] spendTapPoints:points];
}


void _tapjoyAwardTapPoints( int points )
{
	[[TapjoyManager sharedManager] awardTapPoints:points];
}



// Checks to see if the featured app is ready to be displayed
bool _tapjoyIsFullScreenAdReady()
{
	return [[TapjoyManager sharedManager] isFeaturedAppReady];
}


void _tapjoyActionComplete( const char * action )
{
	[[TapjoyManager sharedManager] actionComplete:GetStringParam( action )];
}


