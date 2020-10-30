//
//  UA11YVoiceOverBridgeViewController.m
//  UA11YiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "UA11YVoiceOverBridgeViewController.h"
#import "UA11YInternalAccessibilityHook.h"
#import "UA11YVoiceOverHookOverlayUIView.h"
#import "UA11YVoiceOverHookUIView.h"
#import "UA11YVoiceOverHookOverlaySKView.h"

NS_ASSUME_NONNULL_BEGIN

@interface UA11YVoiceOverBridgeViewController () <UA11YVoiceOverHookOverlayViewDelegate>

@property (nonatomic, strong) id<UA11YVoiceOverHookOverlayView> hookOverlayView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, UA11YInternalAccessibilityHook *> *hookDictionary;

@end

@implementation UA11YVoiceOverBridgeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSMutableDictionary<NSNumber*, UA11YInternalAccessibilityHook *> *hookDictionary = [[NSMutableDictionary alloc] init];
    _hookDictionary = hookDictionary;
    
    // View init
    // UIKit
//    UA11YVoiceOverHookOverlayUIView *hookOverlayView = [[UA11YVoiceOverHookOverlayUIView alloc] initWithFrame:CGRectZero];
    
    // SpriteKit
    UA11YVoiceOverHookOverlaySKView *hookOverlayView = [[UA11YVoiceOverHookOverlaySKView alloc] initWithFrame:CGRectZero];
    
    [hookOverlayView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [hookOverlayView setHidden:!UIAccessibilityIsVoiceOverRunning()];
    [hookOverlayView setViewDelegate:self];
    [[self view] addSubview:hookOverlayView];
    
    _hookOverlayView = hookOverlayView;
    
    // Constraints for hook view
    [[[hookOverlayView leadingAnchor] constraintEqualToAnchor:[[self view] leadingAnchor]] setActive:YES];
    [[[hookOverlayView trailingAnchor] constraintEqualToAnchor:[[self view] trailingAnchor]] setActive:YES];
    [[[hookOverlayView topAnchor] constraintEqualToAnchor:[[self view] topAnchor]] setActive:YES];
    [[[hookOverlayView bottomAnchor] constraintEqualToAnchor:[[self view] bottomAnchor]] setActive:YES];
    
    [self setupNotifications];
}

- (void)setupNotifications
{
    NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
    
    [notificationCenter addObserver:self selector:@selector(voiceOverStatusDidChange:) name:UIAccessibilityVoiceOverStatusDidChangeNotification object:nil];
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [[self hookOverlayView] makeHidden:!UIAccessibilityIsVoiceOverRunning()];
}

# pragma mark - Notifications

- (void)voiceOverStatusDidChange:(NSNotification *)notification
{
    [[self hookOverlayView] makeHidden:!UIAccessibilityIsVoiceOverRunning()];
}

# pragma mark -

- (void)updateHookViewForHook:(UA11YInternalAccessibilityHook *)hook
{
    [[self hookOverlayView] updateHookViewForAccessibilityHook:hook];
}

- (void)updateHookViewsForHooks:(NSArray<UA11YInternalAccessibilityHook *> *)hooks
{
    // First remove all views that are not available anymore
    NSSet<NSNumber *> *validKeySet = [NSSet setWithArray:[hooks valueForKey:@"instanceID"]];
    NSArray *availableKeys = [[self hookDictionary] allKeys];
    
    for(NSNumber *hookKey in availableKeys) {
        if(![validKeySet containsObject:hookKey]) {
            [[self hookOverlayView] removeHookWithID:hookKey];
        }
    }
    
    // Then update/create all other hooks
    for(UA11YInternalAccessibilityHook *hook in hooks) {
        [[self hookOverlayView] updateHookViewForAccessibilityHook:hook];
        [[self hookDictionary] setObject:hook forKey:[hook instanceID]];
    }
    
    UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, nil);
}

- (void)clearAllHooks
{
    [[[self hookDictionary] allValues] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self hookDictionary] removeAllObjects];
    [[self hookOverlayView] clear];
}

#pragma mark - UA11YVoiceOverHookOverlayViewDelegate

- (void)triggerActivateCallbackOfHookWithID:(NSNumber *)instanceID
{
    UA11YInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook selectionCallback]((int)[instanceID integerValue]);
    }
}
    
- (void)triggerIncrementCallbackOfHookWithID:(NSNumber *)instanceID
{
    UA11YInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook valueChangeCallback]((int)[instanceID integerValue], 1);
    }
}
    
- (void)triggerDecrementCallbackOfHookWithID:(NSNumber *)instanceID
{
    UA11YInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook valueChangeCallback]((int)[instanceID integerValue], -1);
    }
}

- (void)triggerDidBecomeFocusedCallbackOfHookWithID:(NSNumber *)instanceID
{
    UA11YInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook focusCallback]((int)[instanceID integerValue]);
    }
}

@end

NS_ASSUME_NONNULL_END
