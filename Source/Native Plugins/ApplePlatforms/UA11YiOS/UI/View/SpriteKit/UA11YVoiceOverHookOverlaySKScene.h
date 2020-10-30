//
//  UA11YVoiceOverHookOverlaySKScene.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <SpriteKit/SpriteKit.h>

NS_ASSUME_NONNULL_BEGIN

@class UA11YInternalAccessibilityHook;
@class UA11YVoiceOverHookSKNode;

@protocol UA11YVoiceOverHookOverlaySKSceneDelegate <NSObject>

- (void)voiceOverHookWithIDWasAccessibilityActivated:(NSNumber *)instanceID;
- (void)voiceOverHookWithIDWasAccessibilityIncremented:(NSNumber *)instanceID;
- (void)voiceOverHookWithIDWasAccessibilityDecremented:(NSNumber *)instanceID;
- (void)voiceOverHookWithIDDidBecomeAccessibilityFocused:(NSNumber *)instanceID;

@end

@interface UA11YVoiceOverHookOverlaySKScene : SKScene

@property (nonatomic, weak) id<UA11YVoiceOverHookOverlaySKSceneDelegate> sceneOverlayDelegate;

- (void)updateHookViewForAccessibilityHook:(UA11YInternalAccessibilityHook *)hook;
- (void)removeHookWithID:(NSNumber *)instanceID;
- (void)clear;

@end

NS_ASSUME_NONNULL_END
