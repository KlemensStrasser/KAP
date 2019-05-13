//
//  KAPVoiceOverHookOverlayView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPVoiceOverHookView;
@class KAPInternalAccessibilityHook;

@protocol KAPVoiceOverHookOverlayViewDelegate <NSObject>

- (void)triggerCallbackOfHookWithID:(NSNumber *)instanceID;

@end

@interface KAPVoiceOverHookOverlayView : UIView

@property (nonatomic, weak) id<KAPVoiceOverHookOverlayViewDelegate> delegate;

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook;
- (void)removeHookWithID:(NSNumber *)instanceID;
- (void)clear;

@end

NS_ASSUME_NONNULL_END
