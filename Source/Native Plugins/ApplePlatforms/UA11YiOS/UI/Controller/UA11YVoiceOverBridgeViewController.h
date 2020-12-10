//
//  UA11YVoiceOverBridgeViewController.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
@class UA11YInternalAccessibilityHook;

NS_ASSUME_NONNULL_BEGIN

@interface UA11YVoiceOverBridgeViewController : UIViewController

- (void)updateHookViewForHook:(UA11YInternalAccessibilityHook *)hook;
- (void)updateHookViewsForHooks:(NSArray<UA11YInternalAccessibilityHook *> *)hooks;

- (void)clearAllHooks;

@end

NS_ASSUME_NONNULL_END
