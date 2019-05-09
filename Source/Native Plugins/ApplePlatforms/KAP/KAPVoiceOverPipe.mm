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

#include "KAPVoiceOverPipe.h"
#include "Helpers/KAPStringConversion.mm"

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

void KAPAddHook(KAPAccessibilityHook hook)
{
    NSString *label = NSStringFromCString(hook.label);
    
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    if (bridgeViewController != nil) {
        
        // Unity hands over the correct screen coordinates, but on iOS we need the coordinates without the scale applied
        CGFloat scale = [[UIScreen mainScreen] scale];
        CGRect frame = CGRectMake(hook.x / scale, hook.y / scale, hook.width / scale, hook.height / scale);
        
        [bridgeViewController addCustomViewWithFrame:frame label:label];
    }
}

void KAPClearAllHooks()
{
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if (bridgeViewController != nil) {
        [bridgeViewController clearAllElements];
    }
}
