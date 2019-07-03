//
//  KAPVoiceOverHookOverlaySKView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <SpriteKit/SpriteKit.h>
#import "KAPVoiceOverHookOverlayView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlaySKView : SKView <KAPVoiceOverHookOverlayView>

@property (nonatomic, weak) id<KAPVoiceOverHookOverlayViewDelegate> viewDelegate;

@end

NS_ASSUME_NONNULL_END
