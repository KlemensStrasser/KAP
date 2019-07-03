//
//  KAPInternalAccessibilityHook.m
//  KAPMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPInternalAccessibilityHook.h"
#include "CHelpers/KAPStringConversion.mm"
#import <UIKit/UIKit.h>

@implementation KAPInternalAccessibilityHook

- (instancetype)initWithExternalHook:(KAPExternalAccessibilityHook)externalHook
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
