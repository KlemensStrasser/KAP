//
//  UA11YInternalAccessibilityHook.m
//  UA11YMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "UA11YInternalAccessibilityHook.h"
#include "CHelpers/UA11YStringConversion.mm"
#import <UIKit/UIKit.h>

@implementation UA11YInternalAccessibilityHook

- (instancetype)initWithExternalHook:(UA11YExternalAccessibilityHook)externalHook
{
    self = [super init];
    if (self) {
        _instanceID = [NSNumber numberWithInt:externalHook.instanceID];
        
        CGFloat scale = [[UIScreen mainScreen] scale];
        CGRect frame = CGRectMake(externalHook.x / scale, externalHook.y / scale, externalHook.width / scale, externalHook.height / scale);
        _frame = frame;
        
        _label = NSStringFromCString(externalHook.label);
        _value = NSStringFromCString(externalHook.value);
        _hint = NSStringFromCString(externalHook.hint);
        
        _trait = externalHook.trait;
        
        _selectionCallback = externalHook.selectionCallback;
        _focusCallback = externalHook.focusCallback;
        _valueChangeCallback = externalHook.valueChangeCallback;
    }
    return self;
}

@end
