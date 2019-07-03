//
//  KAPVoiceOverHookUIView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookUIView.h"

@implementation KAPVoiceOverHookUIView

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
    
@end
