//
//  UA11YVoiceOverHookOverlaySKView.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <SpriteKit/SpriteKit.h>
#import "UA11YVoiceOverHookOverlayView.h"

NS_ASSUME_NONNULL_BEGIN

@interface UA11YVoiceOverHookOverlaySKView : SKView <UA11YVoiceOverHookOverlayView>

@property (nonatomic, weak) id<UA11YVoiceOverHookOverlayViewDelegate> viewDelegate;

@end

NS_ASSUME_NONNULL_END
