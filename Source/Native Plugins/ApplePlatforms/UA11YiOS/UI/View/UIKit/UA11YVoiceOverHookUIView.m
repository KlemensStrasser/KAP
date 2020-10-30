//
//  UA11YVoiceOverHookUIView.m
//  UA11YiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "UA11YVoiceOverHookUIView.h"

@implementation UA11YVoiceOverHookUIView

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID
{
    self = [super initWithFrame:frame];
    if (self) {
        _instanceID = instanceID;
        [self setup];
    }
    return self;
}

- (void)setup
{
    [self setIsAccessibilityElement:YES];
    [self setBackgroundColor:[UIColor colorWithWhite:1.0 alpha:0.25]];
}

# pragma mark - Accessibility Instance Methods

- (BOOL)accessibilityActivate
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityActivated:)]) {
        [[self delegate] voiceOverHookWasAccessibilityActivated:self];
    }
    
    return YES;
}
    
- (void)accessibilityIncrement
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityIncremented:)]) {
        [[self delegate] voiceOverHookWasAccessibilityIncremented:self];
    }
}

- (void)accessibilityDecrement
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityDecremented:)]) {
        [[self delegate] voiceOverHookWasAccessibilityDecremented:self];
    }
}

- (void)accessibilityElementDidBecomeFocused
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookDidBecomeAccessibilityFocused:)]) {
        [[self delegate] voiceOverHookDidBecomeAccessibilityFocused:self];
    }
}
    
@end
