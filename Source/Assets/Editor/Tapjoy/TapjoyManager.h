//
//  TapjoyManager.h
//  TapjoyTest
//
//  Created by Mike on 1/28/11.
//  Copyright 2011 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Tapjoy.h"


typedef enum
{
	TapjoyAdPositionTopLeft,
	TapjoyAdPositionTopCenter,
	TapjoyAdPositionTopRight,
	TapjoyAdPositionCentered,
	TapjoyAdPositionBottomLeft,
	TapjoyAdPositionBottomCenter,
	TapjoyAdPositionBottomRight
} TapjoyAdPosition;


@interface TapjoyManager : NSObject <TJCAdDelegate, TJCVideoAdDelegate, TJCViewDelegate>
{
@private
	BOOL _isFeaturedAppReady;
}
@property (nonatomic, retain) UIView *adView;
@property (nonatomic) TapjoyAdPosition bannerPosition;
@property (nonatomic, copy) NSString *publisherId;
@property (nonatomic, copy) NSString *secretKey;
@property (nonatomic, copy) NSString *adContentSize;
@property (nonatomic, assign) BOOL _shouldRefreshAd;
@property (nonatomic, assign) BOOL showDefaultEarnedCurrencyAlert;


+ (TapjoyManager*)sharedManager;

+ (id)objectFromJson:(NSString*)json;

+ (NSString*)jsonFromObject:(id)object;



- (void)startWithAppId:(NSString*)appId;

- (void)getFullScreenAdWithCurrencyId:(NSString*)currencyId;

- (void)showFullScreenAd;

- (void)setFullScreenAdDelayCount:(int)delayCount;

- (void)createBannerWithPosition:(TapjoyAdPosition)position currencyId:(NSString*)currencyID;

- (void)destroyBanner;

- (void)refreshBanner;

- (void)showOffers;

- (void)showOffersWithCurrencyID:(NSString*)currencyId withCurrencySelector:(BOOL)isSelectorVisible;

- (void)getTapPoints;

- (void)spendTapPoints:(int)points;

- (void)awardTapPoints:(int)points;

- (BOOL)isFeaturedAppReady;

- (void)actionComplete:(NSString*)action;

@end
