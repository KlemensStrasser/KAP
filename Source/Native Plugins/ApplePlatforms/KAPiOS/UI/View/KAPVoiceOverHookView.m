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
    }
    return self;
}

# pragma mark -

- (BOOL)accessibilityActivate
{
    self.invokeSelectionCallback((int)[[self instanceID] integerValue]);
    return YES;
}

@end
