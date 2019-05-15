//
//  KAPVoiceOverPipe.mm
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "KAPVoiceOverBridgeViewController.h"
#import "KAPInternalAccessibilityHook.h"

#include "KAPVoiceOverPipe.h"
#include "Utils/CHelpers/KAPStringConversion.mm"

bool KAPIsScreenReaderRunning()
{
    return UIAccessibilityIsVoiceOverRunning();
}

KAPVoiceOverBridgeViewController *getBridgeViewController() {
    
    KAPVoiceOverBridgeViewController *bridgeViewController = nil;
    
    id<UIApplicationDelegate> appDelegate = [[UIApplication sharedApplication] delegate];
    
    // This is super hacky.
    if([appDelegate isKindOfClass:[NSObject class]]) {
        NSObject *appDelegateObject = (NSObject *)appDelegate;
        
        // We've set our bridgeViewController as the rootController.
        UIViewController *rootViewController = [appDelegateObject valueForKey:@"bridgeViewController"];
        
        if([rootViewController isKindOfClass:[KAPVoiceOverBridgeViewController class]]) {
            bridgeViewController = (KAPVoiceOverBridgeViewController *)rootViewController;
        }
    }
    
    return bridgeViewController;
}

// TODO: Maybe remove that. We shouldn't really add hooks one by one.
void KAPAddHook(KAPExternalAccessibilityHook hook)
{
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    if (bridgeViewController != nil) {
        
//        NSString *label = NSStringFromCString(hook.label);
//
//        // Unity hands over the correct screen coordinates, but on iOS we need the coordinates without the scale applied
//        CGFloat scale = [[UIScreen mainScreen] scale];
//        CGRect frame = CGRectMake(hook.x / scale, hook.y / scale, hook.width / scale, hook.height / scale);
//
//        [bridgeViewController addCustomViewWithFrame:frame label:label];
    }
}

void KAPUpdateHooks(KAPExternalAccessibilityHook *externalHooks, int size)
{
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if(bridgeViewController != nil) {
        NSMutableArray <KAPInternalAccessibilityHook *> *internalHooks = [[NSMutableArray alloc] initWithCapacity:size];
        
        for(int i = 0; i < size; i++) {
            KAPExternalAccessibilityHook externalHook = externalHooks[i];
            KAPInternalAccessibilityHook *internalHook = [[KAPInternalAccessibilityHook alloc] initWithExternalHook:externalHook];
            
            [internalHooks addObject:internalHook];
        }
        
        [bridgeViewController updateHookViewsForHooks:internalHooks];
    }
}

void KAPClearAllHooks()
{
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if (bridgeViewController != nil) {
        [bridgeViewController clearAllHooks];
    }
}