//
//  KAPVoiceOverHookOverlaySKView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookOverlaySKView.h"
#import "KAPVoiceOverHookOverlaySKScene.h"
#import "KAPInternalAccessibilityHook.h"
#import "KAPVoiceOverHookSKNode.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlaySKView () <KAPVoiceOverHookOverlaySKSceneDelegate>

@end

@implementation KAPVoiceOverHookOverlaySKView

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
    
    KAPVoiceOverHookOverlaySKScene *kapScene = [[KAPVoiceOverHookOverlaySKScene alloc] init];
    [kapScene setBackgroundColor:[UIColor clearColor]];
    [kapScene setSceneOverlayDelegate:self];
    [kapScene setScaleMode:SKSceneScaleModeResizeFill];

    [self presentScene:kapScene];
}

# pragma mark - KAPVoiceOverHookOverlayViewDelegate

- (void)makeHidden:(bool)hidden
{
    [self setHidden:hidden];
}

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook
{
    if([[self scene] isKindOfClass:[KAPVoiceOverHookOverlaySKScene class]]) {
        KAPVoiceOverHookOverlaySKScene *kapScene = (KAPVoiceOverHookOverlaySKScene *)[self scene];
        
        [kapScene updateHookViewForAccessibilityHook:hook];
    }
}

- (void)removeHookWithID:(NSNumber *)instanceID
{
    if([[self scene] isKindOfClass:[KAPVoiceOverHookOverlaySKScene class]]) {
        KAPVoiceOverHookOverlaySKScene *kapScene = (KAPVoiceOverHookOverlaySKScene *)[self scene];
        [kapScene removeHookWithID:instanceID];
    }
}

- (void)clear
{
    if([[self scene] isKindOfClass:[KAPVoiceOverHookOverlaySKScene class]]) {
        KAPVoiceOverHookOverlaySKScene *kapScene = (KAPVoiceOverHookOverlaySKScene *)[self scene];
        [kapScene clear];
    }
}

# pragma mark - KAPVoiceOverHookOverlaySKSceneDelegate

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
