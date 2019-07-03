//
//  KAPVoiceOverHookSKNode.h
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <SpriteKit/SpriteKit.h>

NS_ASSUME_NONNULL_BEGIN

@class KAPVoiceOverHookSKNode;

@protocol KAPVoiceOverHookSKNodeDelegate <NSObject>

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookSKNode *)hook;
- (void)voiceOverHookWasAccessibilityIncremented:(KAPVoiceOverHookSKNode *)hook;
- (void)voiceOverHookWasAccessibilityDecremented:(KAPVoiceOverHookSKNode *)hook;

@end

@interface KAPVoiceOverHookSKNode : SKNode

@property (nonatomic, weak) id<KAPVoiceOverHookSKNodeDelegate> delegate;
@property (nonatomic, readonly) NSNumber *instanceID;
@property (nonatomic) CGRect frame;

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID NS_DESIGNATED_INITIALIZER;

- (instancetype)init __attribute((unavailable));
- (instancetype)initWithFrame:(CGRect)frame __attribute((unavailable));
- (instancetype)initWithCoder:(NSCoder *)coder __attribute((unavailable));

@end

NS_ASSUME_NONNULL_END
