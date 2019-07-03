//
//  KAPVoiceOverHookOverlayView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

NS_ASSUME_NONNULL_BEGIN

@class KAPInternalAccessibilityHook;

@protocol KAPVoiceOverHookOverlayView

- (void)makeHidden:(bool)hidden;
- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook;
- (void)removeHookWithID:(NSNumber *)instanceID;
- (void)clear;

@end

@protocol KAPVoiceOverHookOverlayViewDelegate <NSObject>

- (void)triggerActivateCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerIncrementCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerDecrementCallbackOfHookWithID:(NSNumber *)instanceID;
- (void)triggerDidBecomeFocusedCallbackOfHookWithID:(NSNumber *)instanceID;

@end

NS_ASSUME_NONNULL_END
