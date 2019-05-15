//
//  KAPVoiceOverHookView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookOverlayView.h"
#import "KAPVoiceOverHookView.h"
#import "KAPInternalAccessibilityHook.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlayView ()  <KAPVoiceOverHookViewDelegate>

@property (nonatomic, strong) UIView *backgroundView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, KAPVoiceOverHookView *> *hookViews;

@end

@implementation KAPVoiceOverHookOverlayView

- (instancetype)init
{
    self = [super init];
    if (self) {
        [self setupView];
    }
    return self;
}

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        [self setupView];
    }
    return self;
}

- (void)setupView
{   
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

# pragma mark -

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook
{
    KAPVoiceOverHookView *hookView = [[self hookViews] objectForKey:[hook instanceID]];
    
    if(hookView == nil){
        hookView = [[KAPVoiceOverHookView alloc] initWithFrame:[hook frame] instanceID:[hook instanceID]];
    } else {
        [hookView setFrame:[hookView frame]];
    }
    
    [hookView setAccessibilityLabel:[hook label]];
    [hookView setAccessibilityValue:[hook value]];
    [hookView setAccessibilityHint:[hook hint]];
    [hookView setAccessibilityTraits:[hook trait]];
    
    [hookView setDelegate:self];
    
    [self addSubview:hookView];
    
    [[self hookViews] setObject:hookView forKey:[hookView instanceID]];
}

- (void)removeHookWithID:(NSNumber *)instanceID
{
    KAPVoiceOverHookView *invalidHookView = [[self hookViews] objectForKey:instanceID];
    [invalidHookView removeFromSuperview];
    [[self hookViews] removeObjectForKey:instanceID];
}

- (void)clear
{
    [[[self hookViews] allValues] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self hookViews] removeAllObjects];
}

# pragma mark - KAPVoiceOverHookViewDelegate

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookView *)hook
{
    if([[self delegate] respondsToSelector:@selector(triggerCallbackOfHookWithID:)]) {
        [[self delegate] triggerCallbackOfHookWithID:[hook instanceID]];
    }
}

@end

NS_ASSUME_NONNULL_END
