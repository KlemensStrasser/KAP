//
//  KAPVoiceOverHookView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookView.h"

@implementation KAPVoiceOverHookView

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

# pragma mark -

- (BOOL)accessibilityActivate
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityActivated:)]) {
        [[self delegate] voiceOverHookWasAccessibilityActivated:self];
    }
    
    return YES;
}

@end