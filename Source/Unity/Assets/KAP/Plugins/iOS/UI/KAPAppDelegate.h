//
//  KAPAppDelegate.h
//  Unity-iPhone
//
//  Created by Klemens Strasser on 08.05.19.
//

#import "UnityAppController.h"
@class KAPVoiceOverBridgeViewController;

NS_ASSUME_NONNULL_BEGIN

@interface KAPAppDelegate : UnityAppController

@property (nonatomic, strong) KAPVoiceOverBridgeViewController *bridgeViewController;

@end

#define IMPL_APP_CONTROLLER_SUBCLASS(KAPAppDelegate)
@interface KAPAppDelegate(OverrideAppDelegate)

+(void)load;

@end

NS_ASSUME_NONNULL_END
