//
//  KAPVoiceOverHookSKNode.m
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookSKNode.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookSKNode()

@property (nonatomic, assign) SKShapeNode *shapeNode;

@end

@implementation KAPVoiceOverHookSKNode

- (instancetype)initWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID
{
    self = [super init];
    if (self) {
        _instanceID = instanceID;
        [self setupWithFrame:frame];
    }
    return self;
}

- (void)setupWithFrame:(CGRect)frame
{
    // TODO: Don't forget that SpriteKit 0,0 is on a different location! But might not be relevant here
    
    [self setPosition:frame.origin];
    SKShapeNode *shapeNode = [SKShapeNode shapeNodeWithRect:CGRectMake(0, 0, frame.size.width, frame.size.height)];
    [shapeNode setFillColor:[UIColor colorWithWhite:1.0 alpha:0.25]];
    
    [self addChild:shapeNode];
    
    [shapeNode runAction:[SKAction moveToY:-frame.size.height duration:0.0]];
    
    // TODO: Self accessibility element or the ShapeNode?
    [shapeNode setIsAccessibilityElement:NO];
    [self setIsAccessibilityElement:YES];
    
    _shapeNode = shapeNode;
}

- (void)setFrame:(CGRect)frame
{
    [self setPosition:frame.origin];
    [[self shapeNode] runAction:[SKAction group:@[[SKAction resizeToWidth:frame.size.width duration:0.0],
                                                  [SKAction resizeToHeight:frame.size.height duration:0.0]]]];
    // ToDO: Missing: -frame.size.height
    [[self shapeNode] runAction:[SKAction moveToY:-frame.size.height duration:0.0]];
}

- (CGRect)frame
{
    return [[self shapeNode] frame];
}

# pragma mark - Accessibility Instance Methods

- (BOOL)accessibilityActivate
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityActivated:)]) {
        [[self delegate] voiceOverHookWasAccessibilityActivated:self];
    }
    
    return YES;
}

- (void)accessibilityIncrement
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityIncremented:)]) {
        [[self delegate] voiceOverHookWasAccessibilityIncremented:self];
    }
}

- (void)accessibilityDecrement
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookWasAccessibilityDecremented:)]) {
        [[self delegate] voiceOverHookWasAccessibilityDecremented:self];
    }
}

- (void)accessibilityElementDidBecomeFocused
{
    if([[self delegate] respondsToSelector:@selector(voiceOverHookDidBecomeAccessibilityFocused:)]) {
        [[self delegate] voiceOverHookDidBecomeAccessibilityFocused:self];
    }
}

- (CGRect)accessibilityFrame
{
    return [[self shapeNode] accessibilityFrame];
}

@end

NS_ASSUME_NONNULL_END
