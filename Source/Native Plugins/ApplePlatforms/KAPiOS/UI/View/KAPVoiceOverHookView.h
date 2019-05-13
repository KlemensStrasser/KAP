//
//  KAPVoiceOverHookView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPVoiceOverHookView;

@protocol KAPVoiceOverHookViewDelegate <NSObject>

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookView *)hook;

@end

@interface KAPVoiceOverHookView : UIView

@property (nonatomic, weak) id<KAPVoiceOverHookViewDelegate> delegate;
@property (nonatomic, readonly) NSNumber *instanceID;

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID  NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable));
- (instancetype)initWithFrame:(CGRect)frame __attribute((unavailable));
- (instancetype)initWithCoder:(NSCoder *)coder __attribute((unavailable));

@end

NS_ASSUME_NONNULL_END
