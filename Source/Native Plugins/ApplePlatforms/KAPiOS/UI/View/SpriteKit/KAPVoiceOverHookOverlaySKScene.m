//
//  KAPVoiceOverHookOverlaySKScene.m
//  KAPiOS
//
//  Created by Klemens Strasser on 03.07.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookOverlaySKScene.h"
#import "KAPInternalAccessibilityHook.h"
#import "KAPVoiceOverHookSKNode.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlaySKScene () <KAPVoiceOverHookSKNodeDelegate> {
    BOOL didAlreadyMove;
}

@property (nonatomic, strong) UIView *backgroundView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, KAPVoiceOverHookSKNode *> *hookNodes;
@property (nonatomic, strong) NSMutableArray<KAPInternalAccessibilityHook *> *temporaryHooks;

@end

@implementation KAPVoiceOverHookOverlaySKScene

- (instancetype)init
{
    self = [super init];
    if (self) {
        [self setup];
    }
    return self;
}

- (instancetype)initWithSize:(CGSize)size
{
    self = [super initWithSize:size];
    if (self) {
        [self setup];
    }
    return self;
}

- (void)setup
{
    NSMutableDictionary *hookNodes = [[NSMutableDictionary alloc] init];
    _hookNodes = hookNodes;
    
    NSMutableArray *temporaryHooks = [[NSMutableArray alloc] init];
    _temporaryHooks = temporaryHooks;
    
    didAlreadyMove = false;
}

# pragma mark -
- (void)didMoveToView:(SKView *)view
{
    [super didMoveToView:view];
    didAlreadyMove = true;
    
    // We might have saved some hooks to _temporaryHooks if updateHookViewForAccessibilityHook was called before the scene was ready
    // Now is the time to add them
    if([[self temporaryHooks] count] > 0) {
        for(KAPInternalAccessibilityHook *hook in [self temporaryHooks])
        {
            [self updateHookViewForAccessibilityHook:hook];
        }
        [[self temporaryHooks] removeAllObjects];
        
        UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, nil);
    }
}

- (void)updateHookViewForAccessibilityHook:(KAPInternalAccessibilityHook *)hook
{
    if(didAlreadyMove) {
        
        KAPVoiceOverHookSKNode *hookNode = [[self hookNodes] objectForKey:[hook instanceID]];
        
        CGPoint convertedOrigin = [[self view] convertPoint:[hook frame].origin toScene:self];
        CGRect convertedFrame = CGRectMake(convertedOrigin.x, convertedOrigin.y, CGRectGetWidth([hook frame]), CGRectGetHeight([hook frame]));
        
        if(hookNode == nil) {
            hookNode = [[KAPVoiceOverHookSKNode alloc] initWithFrame:convertedFrame instanceID:[hook instanceID]];
            [hookNode setDelegate:self];
            
            [self addChild:hookNode];
            [[self hookNodes] setObject:hookNode forKey:[hookNode instanceID]];
        } else {
            [hookNode setFrame:convertedFrame];
        }
        
        [hookNode setAccessibilityLabel:[hook label]];
        [hookNode setAccessibilityValue:[hook value]];
        [hookNode setAccessibilityHint:[hook hint]];
        [hookNode setAccessibilityTraits:[hook trait]];
    } else {
        // Wait until didMove was called and then add everything
        [[self temporaryHooks] addObject:hook];
    }
}

- (void)removeHookWithID:(NSNumber *)instanceID
{
    KAPVoiceOverHookSKNode *invalidHookNode = [[self hookNodes] objectForKey:instanceID];
    [invalidHookNode removeFromParent];
    [[self hookNodes] removeObjectForKey:instanceID];
}

- (void)clear
{
    [[[self hookNodes] allValues] makeObjectsPerformSelector:@selector(removeFromParent)];
    [[self hookNodes] removeAllObjects];
}


# pragma mark - KAPVoiceOverHookSKNodeDelegate

- (void)voiceOverHookWasAccessibilityActivated:(KAPVoiceOverHookSKNode *)hook
{
    if([[self sceneOverlayDelegate] respondsToSelector:@selector(voiceOverHookWithIDWasAccessibilityActivated:)]){
        [[self sceneOverlayDelegate] voiceOverHookWithIDWasAccessibilityActivated:[hook instanceID]];
    }
}
- (void)voiceOverHookWasAccessibilityIncremented:(KAPVoiceOverHookSKNode *)hook
{
    if([[self sceneOverlayDelegate] respondsToSelector:@selector(voiceOverHookWithIDWasAccessibilityIncremented:)]){
        [[self sceneOverlayDelegate] voiceOverHookWithIDWasAccessibilityIncremented:[hook instanceID]];
    }
}
- (void)voiceOverHookWasAccessibilityDecremented:(KAPVoiceOverHookSKNode *)hook
{
    if([[self sceneOverlayDelegate] respondsToSelector:@selector(voiceOverHookWithIDWasAccessibilityDecremented:)]){
        [[self sceneOverlayDelegate] voiceOverHookWithIDWasAccessibilityDecremented:[hook instanceID]];
    }
}

- (void)voiceOverHookDidBecomeAccessibilityFocused:(KAPVoiceOverHookSKNode *)hook
{
    if([[self sceneOverlayDelegate] respondsToSelector:@selector(voiceOverHookWithIDDidBecomeAccessibilityFocused:)]){
        [[self sceneOverlayDelegate] voiceOverHookWithIDDidBecomeAccessibilityFocused:[hook instanceID]];
    }
}

# pragma mark - Accessibility

- (NSInteger)accessibilityElementCount
{
    return [[[self hookNodes] allValues] count];
}

/// Index needs to be invert, because Spritekit behaves weirdly
- (NSInteger)indexOfAccessibilityElement:(id)element
{
    NSInteger index = [super indexOfAccessibilityElement:element];
    return [self accessibilityElementCount] - index - 1;
}

/// Index needs to be invert, because Spritekit behaves weirdly
- (nullable id)accessibilityElementAtIndex:(NSInteger)index
{
    return [super accessibilityElementAtIndex:[self accessibilityElementCount] - index - 1];
}

@end

NS_ASSUME_NONNULL_END
