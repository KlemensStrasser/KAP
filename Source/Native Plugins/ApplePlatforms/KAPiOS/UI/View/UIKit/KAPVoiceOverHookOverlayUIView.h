//
//  KAPVoiceOverHookOverlayUIView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "KAPVoiceOverHookOverlayView.h"

NS_ASSUME_NONNULL_BEGIN

@class KAPInternalAccessibilityHook;

@interface KAPVoiceOverHookOverlayUIView : UIView <KAPVoiceOverHookOverlayView>

@property (nonatomic, weak) id<KAPVoiceOverHookOverlayViewDelegate> viewDelegate;

@end

NS_ASSUME_NONNULL_END
