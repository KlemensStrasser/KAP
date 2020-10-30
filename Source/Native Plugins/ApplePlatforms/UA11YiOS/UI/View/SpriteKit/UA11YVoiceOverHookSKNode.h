//
//  UA11YVoiceOverHookSKNode.h
//  UA11YiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <SpriteKit/SpriteKit.h>

NS_ASSUME_NONNULL_BEGIN

@class UA11YVoiceOverHookSKNode;

@protocol UA11YVoiceOverHookSKNodeDelegate <NSObject>

- (void)voiceOverHookWasAccessibilityActivated:(UA11YVoiceOverHookSKNode *)hook;
- (void)voiceOverHookWasAccessibilityIncremented:(UA11YVoiceOverHookSKNode *)hook;
- (void)voiceOverHookWasAccessibilityDecremented:(UA11YVoiceOverHookSKNode *)hook;
- (void)voiceOverHookDidBecomeAccessibilityFocused:(UA11YVoiceOverHookSKNode *)hook;

@end

@interface UA11YVoiceOverHookSKNode : SKNode

@property (nonatomic, weak) id<UA11YVoiceOverHookSKNodeDelegate> delegate;
@property (nonatomic, readonly) NSNumber *instanceID;
@property (nonatomic) CGRect frame;

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable));
- (instancetype)initWithFrame:(CGRect)frame __attribute((unavailable));
- (instancetype)initWithCoder:(NSCoder *)coder __attribute((unavailable));

@end

NS_ASSUME_NONNULL_END
