//
//  UA11YInternalAccessibilityHook.h
//  UA11YMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#include "CHelpers/UA11YExternalAccessiblityHook.h"
@class UA11YExternalAccessiblityHook;

NS_ASSUME_NONNULL_BEGIN

@interface UA11YInternalAccessibilityHook : NSObject

@property (readonly, nonatomic) NSNumber *instanceID;
@property (readonly, nonatomic) CGRect frame;
@property (readonly, nonatomic) NSString *label;
@property (readonly, nonatomic) NSString *value;
@property (readonly, nonatomic) NSString *hint;
@property (readonly, nonatomic) uint64_t trait;
@property (readonly, nonatomic) InvokeSelectionCallback selectionCallback;
@property (readonly, nonatomic) InvokeFocusCallback focusCallback;
@property (readonly, nonatomic) InvokeValueChangeCallback valueChangeCallback;

- (instancetype)initWithExternalHook:(UA11YExternalAccessibilityHook)externalHook NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable)); 

@end

NS_ASSUME_NONNULL_END
