//
//  UA11YVoiceOverHookOverlaySKView.m
//  UA11YiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "UA11YVoiceOverHookOverlaySKView.h"
#import "UA11YVoiceOverHookOverlaySKScene.h"
#import "UA11YInternalAccessibilityHook.h"
#import "UA11YVoiceOverHookSKNode.h"

NS_ASSUME_NONNULL_BEGIN

@interface UA11YVoiceOverHookOverlaySKView () <UA11YVoiceOverHookOverlaySKSceneDelegate>

@end

@implementation UA11YVoiceOverHookOverlaySKView

- (instancetype)init
{
    self = [super init];
    if (self) {
        [self setup];
    }
    return self;
}

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        [self setup];
    }
    return self;
}

- (void)setup
{
    [self setIgnoresSiblingOrder:YES];
    [self setAllowsTransparency:YES];
    [self setBackgroundColor:[UIColor clearColor]];
    
    UA11YVoiceOverHookOverlaySKScene *UA11YScene = [[UA11YVoiceOverHookOverlaySKScene alloc] init];
    [UA11YScene setBackgroundColor:[UIColor clearColor]];
    [UA11YScene setSceneOverlayDelegate:self];
    [UA11YScene setScaleMode:SKSceneScaleModeResizeFill];

    [self presentScene:UA11YScene];
}

# pragma mark - UA11YVoiceOverHookOverlayViewDelegate

- (void)makeHidden:(bool)hidden
{
    [self setHidden:hidden];
}

- (void)updateHookViewForAccessibilityHook:(UA11YInternalAccessibilityHook *)hook
{
    if([[self scene] isKindOfClass:[UA11YVoiceOverHookOverlaySKScene class]]) {
        UA11YVoiceOverHookOverlaySKScene *UA11YScene = (UA11YVoiceOverHookOverlaySKScene *)[self scene];
        
        [UA11YScene updateHookViewForAccessibilityHook:hook];
    }
}

- (void)removeHookWithID:(NSNumber *)instanceID
{
    if([[self scene] isKindOfClass:[UA11YVoiceOverHookOverlaySKScene class]]) {
        UA11YVoiceOverHookOverlaySKScene *UA11YScene = (UA11YVoiceOverHookOverlaySKScene *)[self scene];
        [UA11YScene removeHookWithID:instanceID];
    }
}

- (void)clear
{
    if([[self scene] isKindOfClass:[UA11YVoiceOverHookOverlaySKScene class]]) {
        UA11YVoiceOverHookOverlaySKScene *UA11YScene = (UA11YVoiceOverHookOverlaySKScene *)[self scene];
        [UA11YScene clear];
    }
}

# pragma mark - UA11YVoiceOverHookOverlaySKSceneDelegate

- (void)voiceOverHookWithIDWasAccessibilityActivated:(NSNumber *)instanceID
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerActivateCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerActivateCallbackOfHookWithID:instanceID];
    }
}

- (void)voiceOverHookWithIDWasAccessibilityIncremented:(NSNumber *)instanceID
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerIncrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerIncrementCallbackOfHookWithID:instanceID];
    }
}

- (void)voiceOverHookWithIDWasAccessibilityDecremented:(NSNumber *)instanceID
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerDecrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDecrementCallbackOfHookWithID:instanceID];
    }
}

- (void)voiceOverHookWithIDDidBecomeAccessibilityFocused:(NSNumber *)instanceID
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerDidBecomeFocusedCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDidBecomeFocusedCallbackOfHookWithID:instanceID];
    }
}

@end

NS_ASSUME_NONNULL_END
