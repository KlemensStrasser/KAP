//
//  UA11YVoiceOverHookUIView.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class UA11YVoiceOverHookUIView;

@protocol UA11YVoiceOverHookUIViewDelegate <NSObject>

- (void)voiceOverHookWasAccessibilityActivated:(UA11YVoiceOverHookUIView *)hook;
- (void)voiceOverHookWasAccessibilityIncremented:(UA11YVoiceOverHookUIView *)hook;
- (void)voiceOverHookWasAccessibilityDecremented:(UA11YVoiceOverHookUIView *)hook;
- (void)voiceOverHookDidBecomeAccessibilityFocused:(UA11YVoiceOverHookUIView *)hook;

@end

@interface UA11YVoiceOverHookUIView : UIView

@property (nonatomic, weak) id<UA11YVoiceOverHookUIViewDelegate> delegate;
@property (nonatomic, readonly) NSNumber *instanceID;

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable));
- (instancetype)initWithFrame:(CGRect)frame __attribute((unavailable));
- (instancetype)initWithCoder:(NSCoder *)coder __attribute((unavailable));

@end

NS_ASSUME_NONNULL_END
