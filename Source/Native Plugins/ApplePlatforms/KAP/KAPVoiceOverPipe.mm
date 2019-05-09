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


void KAPAddHookAtPosition(float x, float y, float width, float height, const char *cLabel)
{
    NSString *label = NSStringFromCString(cLabel);
    
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    if (bridgeViewController != nil) {
        
        // Unity hands over the correct screen coordinates, but on iOS we need the coordinates without the scale applied
        CGFloat scale = [[UIScreen mainScreen] scale];
        CGRect frame = CGRectMake(x / scale, y / scale, width / scale, height / scale);
        
        NSLog(@"Frame: %@", NSStringFromCGRect(frame));
        
        [bridgeViewController addCustomViewWithFrame:frame label:label];
    }
    
//    id<UIApplicationDelegate> appDelegate = [[UIApplication sharedApplication] delegate];
//
//    // This is super hacky.
//    if([appDelegate isKindOfClass:[NSObject class]]) {
//        NSObject *appDelegateObject = (NSObject *)appDelegate;
//
//        // We've set our bridgeViewController as the rootController.
//        UIViewController *rootViewController = [appDelegateObject valueForKey:@"bridgeViewController"];
//
//        if([rootViewController isKindOfClass:[KAPVoiceOverBridgeViewController class]]) {
//            KAPVoiceOverBridgeViewController *bridgeViewController = (KAPVoiceOverBridgeViewController *)rootViewController;
//
//            CGRect frame = CGRectMake(x, y, width, height);
//
//            [bridgeViewController addCustomViewWithFrame:frame label:label];
//        }
//    }
}

void KAPClearAllHooks()
{
    KAPVoiceOverBridgeViewController *bridgeViewController = getBridgeViewController();
    
    if (bridgeViewController != nil) {
        [bridgeViewController clearAllElements];
    }
}
