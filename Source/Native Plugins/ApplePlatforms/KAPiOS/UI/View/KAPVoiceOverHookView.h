//
//  KAPVoiceOverHookView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookView : UIView

- (void)addCustomViewWithFrame:(CGRect)rect label:(NSString *)label;

- (void)clearAllElements;

@end

NS_ASSUME_NONNULL_END
