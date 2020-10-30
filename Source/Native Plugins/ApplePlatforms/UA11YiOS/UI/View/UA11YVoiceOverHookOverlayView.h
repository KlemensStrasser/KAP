//
//  UA11YVoiceOverHookOverlayView.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

NS_ASSUME_NONNULL_BEGIN

@class UA11YInternalAccessibilityHook;

@protocol UA11YVoiceOverHookOverlayView

- (void)makeHidden:(bool)hidden;
- (void)updateHookViewForAccessibilityHook:(UA11YInternalAccessibilityHook *)hook;
- (void)removeHookWithID:(NSNumber *)instanceID;
- (void)clear;

@end

@protocol UA11YVoiceOverHookOverlayViewDelegate <NSObject>

- (void)triggerActivateCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerIncrementCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerDecrementCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerDidBecomeFocusedCallbackOfHookWithID:(NSNumber *)instanceID;

@end

NS_ASSUME_NONNULL_END
