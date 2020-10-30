//
//  UA11YVoiceOverHookOverlayUIView.m
//  UA11YiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "UA11YVoiceOverHookOverlayUIView.h"
#import "UA11YVoiceOverHookUIView.h"
#import "UA11YInternalAccessibilityHook.h"

NS_ASSUME_NONNULL_BEGIN

@interface UA11YVoiceOverHookOverlayUIView ()  <UA11YVoiceOverHookUIViewDelegate>

@property (nonatomic, strong) UIView *backgroundView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, UA11YVoiceOverHookUIView *> *hookViews;

@end

@implementation UA11YVoiceOverHookOverlayUIView

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

# pragma mark - UA11YVoiceOverHookOverlayViewDelegate

- (void)makeHidden:(bool)hidden
{
    [self setHidden:hidden];
}

- (void)updateHookViewForAccessibilityHook:(UA11YInternalAccessibilityHook *)hook
{
    UA11YVoiceOverHookUIView *hookView = [[self hookViews] objectForKey:[hook instanceID]];
    
    if(hookView == nil){
        hookView = [[UA11YVoiceOverHookUIView alloc] initWithFrame:[hook frame] instanceID:[hook instanceID]];
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
    UA11YVoiceOverHookUIView *invalidHookView = [[self hookViews] objectForKey:instanceID];
    [invalidHookView removeFromSuperview];
    [[self hookViews] removeObjectForKey:instanceID];
}

- (void)clear
{
    [[[self hookViews] allValues] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self hookViews] removeAllObjects];
}

# pragma mark - UA11YVoiceOverHookViewDelegate

- (void)voiceOverHookWasAccessibilityActivated:(UA11YVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerActivateCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerActivateCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookWasAccessibilityIncremented:(UA11YVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerIncrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerIncrementCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookWasAccessibilityDecremented:(UA11YVoiceOverHookUIView *)hook
{
    if([[self viewDelegate] respondsToSelector:@selector(triggerDecrementCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDecrementCallbackOfHookWithID:[hook instanceID]];
    }
}

- (void)voiceOverHookDidBecomeAccessibilityFocused:(UA11YVoiceOverHookUIView *)hook {
    if([[self viewDelegate] respondsToSelector:@selector(triggerDidBecomeFocusedCallbackOfHookWithID:)]) {
        [[self viewDelegate] triggerDidBecomeFocusedCallbackOfHookWithID:[hook instanceID]];
    }
}

@end

NS_ASSUME_NONNULL_END
