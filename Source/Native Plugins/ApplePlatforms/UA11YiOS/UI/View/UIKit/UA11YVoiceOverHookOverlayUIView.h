//
//  UA11YVoiceOverHookOverlayUIView.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "UA11YVoiceOverHookOverlayView.h"

NS_ASSUME_NONNULL_BEGIN

@class UA11YInternalAccessibilityHook;

@interface UA11YVoiceOverHookOverlayUIView : UIView <UA11YVoiceOverHookOverlayView>

@property (nonatomic, weak) id<UA11YVoiceOverHookOverlayViewDelegate> viewDelegate;

@end

NS_ASSUME_NONNULL_END
