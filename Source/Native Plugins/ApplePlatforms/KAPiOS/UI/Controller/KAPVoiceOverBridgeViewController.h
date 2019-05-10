//
//  KAPVoiceOverBridgeViewController.h
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "KAPInternalAccessibilityHook.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverBridgeViewController : UIViewController

- (void)updateHookViewsForHooks:(NSArray<KAPInternalAccessibilityHook *> *)hooks;

- (void)clearAllHooks;

@end

NS_ASSUME_NONNULL_END
