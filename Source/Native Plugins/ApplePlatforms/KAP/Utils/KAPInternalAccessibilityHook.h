//
//  KAPInternalAccessibilityHook.h
//  KAPMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#include "CHelpers/KAPExternalAccessiblityHook.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPInternalAccessibilityHook : NSObject

@property (readonly, nonatomic) NSNumber *instanceID;
@property (readonly, nonatomic) CGRect frame;
@property (readonly, nonatomic) NSString *label;
@property (readonly, nonatomic) NSString *value;
@property (readonly, nonatomic) NSString *hint;
@property (readonly, nonatomic) InvokeSelectionCallback selectionCallback;

- (instancetype)initWithExternalHook:(KAPExternalAccessibilityHook)externalHook NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable)); 

@end

NS_ASSUME_NONNULL_END
