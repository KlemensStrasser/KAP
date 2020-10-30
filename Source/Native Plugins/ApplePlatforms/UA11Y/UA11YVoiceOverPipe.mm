//
//  UA11YVoiceOverPipe.mm
//  UA11YiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "UA11YVoiceOverBridgeViewController.h"
#import "UA11YInternalAccessibilityHook.h"

#include "UA11YVoiceOverPipe.h"
#include "Utils/CHelpers/UA11YStringConversion.mm"

bool UA11YIsScreenReaderRunning()
{
    return UIAccessibilityIsVoiceOverRunning();
}

UA11YVoiceOverBridgeViewController *getBridgeViewController() {
    
    UA11YVoiceOverBridgeViewController *bridgeViewController = nil;
    
    id<UIApplicationDelegate> appDelegate = [[UIApplication sharedApplication] delegate];
    
    // This is super hacky.
    if([appDelegate isKindOfClass:[NSObject class]]) {
        NSObject *appDelegateObject = (NSObject *)appDelegate;
        
        // We've set our bridgeViewController as the rootController.
        UIViewController *rootViewController = [appDelegateObject valueForKey:@"bridgeViewController"];
        
        if([rootViewController isKindOfClass:[UA11YVoiceOverBridgeViewController class]]) {
            bridgeViewController = (UA11YVoiceOverBridgeViewController *)rootViewController;
        }
    }
    
    return bridgeViewController;
}

void UA11YUpdateHook(UA11YExternalAccessibilityHook externalHook)
{
    UA11YVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    // TODO: Implement again
    if(bridgeViewController != nil) {
        UA11YInternalAccessibilityHook *internalHook = [[UA11YInternalAccessibilityHook alloc] initWithExternalHook:externalHook];
    }
}

void UA11YUpdateHooks(UA11YExternalAccessibilityHook *externalHooks, int size)
{
    UA11YVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if(bridgeViewController != nil) {
        NSMutableArray <UA11YInternalAccessibilityHook *> *internalHooks = [[NSMutableArray alloc] initWithCapacity:size];
        
        for(int i = 0; i < size; i++) {
            UA11YExternalAccessibilityHook externalHook = externalHooks[i];
            UA11YInternalAccessibilityHook *internalHook = [[UA11YInternalAccessibilityHook alloc] initWithExternalHook:externalHook];
            
            [internalHooks addObject:internalHook];
        }
        
        [bridgeViewController updateHookViewsForHooks:internalHooks];
    }
}

void UA11YClearAllHooks()
{
    UA11YVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if (bridgeViewController != nil) {
        [bridgeViewController clearAllHooks];
    }
}

void UA11YAnnoucnceVoiceOverMessage(const char *cString)
{
    NSString *text = NSStringFromCString(cString);
    
    if(text){
        UIAccessibilityPostNotification(UIAccessibilityScreenChangedNotification, text);
    }
}
