//
//  UA11YAppDelegate.h
//  Unity-iPhone
//
//  Created by Klemens Strasser on 08.05.19.
//

#import "UnityAppController.h"
@class UA11YVoiceOverBridgeViewController;

NS_ASSUME_NONNULL_BEGIN

@interface UA11YAppDelegate : UnityAppController

@property (nonatomic, strong) UA11YVoiceOverBridgeViewController *bridgeViewController;

@end

#define IMPL_APP_CONTROLLER_SUBCLASS(UA11YAppDelegate)
@interface UA11YAppDelegate(OverrideAppDelegate)

+(void)load;

@end

NS_ASSUME_NONNULL_END
