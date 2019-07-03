//
//  KAPVoiceOverHookUIView.h
//  KAPiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPVoiceOverHookUIView;

@protocol KAPVoiceOverHookUIViewDelegate <NSObject>

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookUIView *)hook;
- (void)voiceOverHookWasAccessibilityIncremented:(KAPVoiceOverHookUIView *)hook;
- (void)voiceOverHookWasAccessibilityDecremented:(KAPVoiceOverHookUIView *)hook;

@end

@interface KAPVoiceOverHookUIView : UIView

@property (nonatomic, weak) id<KAPVoiceOverHookUIViewDelegate> delegate;
@property (nonatomic, readonly) NSNumber *instanceID;

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable));
- (instancetype)initWithFrame:(CGRect)frame __attribute((unavailable));
- (instancetype)initWithCoder:(NSCoder *)coder __attribute((unavailable));

@end

NS_ASSUME_NONNULL_END
