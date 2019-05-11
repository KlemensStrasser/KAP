//
//  KAPVoiceOverHookOverlayView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPVoiceOverHookView;

@interface KAPVoiceOverHookOverlayView : UIView

- (KAPVoiceOverHookView *)addHookViewWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID;

@end

NS_ASSUME_NONNULL_END
