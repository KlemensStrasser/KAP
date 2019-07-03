//
//  KAPVoiceOverHookOverlayUIView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookOverlayUIView.h"
#import "KAPVoiceOverHookUIView.h"
#import "KAPInternalAccessibilityHook.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlayUIView ()  <KAPVoiceOverHookUIViewDelegate>

@property (nonatomic, strong) UIView *backgroundView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, KAPVoiceOverHookUIView *> *hookViews;

@end

@implementation KAPVoiceOverHookOverlayUIView

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
    NSMutableDictionary *hookViews = [[NSMutableDictionary alloc] init];
    _hookViews = hookViews;
    
    // Add background
    UIView *backgroundView = [[UIView alloc] initWithFrame:CGRectZero];
    [backgroundView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [backgroundView setBackgroundColor:[UIColor colorWithWhite:0.0 alpha:0.2]];
    
    [self addSubview:backgroundView];
    _backgroundView = backgroundView;
    
    [[[backgroundView leadingAnchor] constraintEqualToAnchor:[self leadingAnchor]] setActive:YES];
    [[[backgroundView trailingAnchor] constraintEqualToAnchor:[self trailingAnchor]] setActive:YES];
    [[[backgroundView topAnchor] constraintEqualToAnchor:[self topAnchor]] setActive:YES];
    [[[backgroundView bottomAnchor] constraintEqualToAnchor:[self bottomAnchor]] setActive:YES];
}

# pragma mark - KAPVoiceOverHookOverlayViewDelegate

- (void)makeHidden:(bool)hidden
{
    [self setHidden:hidden];
}

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook
{
    KAPVoiceOverHookUIView *hookView = [[self hookViews] objectForKey:[hook instanceID]];
    
    if(hookView == nil){
        hookView = [[KAPVoiceOverHookUIView alloc] initWithFrame:[hook frame] instanceID:[hook instanceID]];
        [hookView setDelegate:self];
        
        [self addSubview:hookView];
        [[self hookViews] setObject:hookView forKey:[hookView instanceID]];
    } else {
        [hookView setFrame:[hook frame]];
    }
    
    [hookView setAccessibilityLabel:[hook label]];
    [hookView setAccessibilityValue:[hook value]];
    [hookView setAccessibilityHint:[hook hint]];
    [hookView setAccessibilityTraits:[hook trait]];
}

- (void)removeHookWithID:(NSNumber *)instanceID
{
    KAPVoiceOverHookUIView *invalidHookView = [[self hookViews] objectForKey:instanceID];
    [invalidHookView removeFromSuperview];
    [[self hookViews] removeObjectForKey:instanceID];
}

- (void)clear
{
    [[[self hookViews] allValues] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self hookViews] removeAllObjects];
}

# pragma mark - KAPVoiceOverHookViewDelegate

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerActivateCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerActivateCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookWasAccessibilityIncremented:(KAPVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerIncrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerIncrementCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookWasAccessibilityDecremented:(KAPVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerDecrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDecrementCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookDidBecomeAccessibilityFocused:(KAPVoiceOverHookUIView *)hook {
    if([[self viewDelegate] respondsToSelector:@selector(triggerDidBecomeFocusedCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDidBecomeFocusedCallbackOfHookWithID:[hook instanceID]];
    }
}

@end

NS_ASSUME_NONNULL_END
