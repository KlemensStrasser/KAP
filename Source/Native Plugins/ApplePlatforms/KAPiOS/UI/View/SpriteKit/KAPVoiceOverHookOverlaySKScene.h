//
//  KAPVoiceOverHookOverlaySKScene.h
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <SpriteKit/SpriteKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPInternalAccessibilityHook;
@class KAPVoiceOverHookSKNode;

@protocol KAPVoiceOverHookOverlaySKSceneDelegate <NSObject>

- (void)voiceOverHookWithIDWasAccessibilityActivated:(NSNumber *)instanceID;
- (void)voiceOverHookWithIDWasAccessibilityIncremented:(NSNumber *)instanceID;
- (void)voiceOverHookWithIDWasAccessibilityDecremented:(NSNumber *)instanceID;

@end

@interface KAPVoiceOverHookOverlaySKScene : SKScene

@property (nonatomic, weak) id<KAPVoiceOverHookOverlaySKSceneDelegate> sceneOverlayDelegate;

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook;
- (void)removeHookWithID:(NSNumber *)instanceID;
- (void)clear;

@end

NS_ASSUME_NONNULL_END
