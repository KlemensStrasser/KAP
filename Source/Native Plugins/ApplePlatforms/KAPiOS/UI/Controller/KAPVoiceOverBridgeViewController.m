//
//  KAPVoiceOverBridgeViewController.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverBridgeViewController.h"
#import "KAPVoiceOverHookOverlayUIView.h"
#import "KAPVoiceOverHookUIView.h"
#import "KAPVoiceOverHookOverlaySKView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverBridgeViewController () <KAPVoiceOverHookOverlayViewDelegate>

@property (nonatomic, strong) id<KAPVoiceOverHookOverlayView> hookOverlayView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, KAPInternalAccessibilityHook *> *hookDictionary;

@end

@implementation KAPVoiceOverBridgeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSMutableDictionary<NSNumber*, KAPInternalAccessibilityHook *> *hookDictionary = [[NSMutableDictionary alloc] init];
    _hookDictionary = hookDictionary;
    
    // View init
    // UIKit
//    KAPVoiceOverHookOverlayUIView *hookOverlayView = [[KAPVoiceOverHookOverlayUIView alloc] initWithFrame:CGRectZero];
    
    // SpriteKit
    KAPVoiceOverHookOverlaySKView *hookOverlayView = [[KAPVoiceOverHookOverlaySKView alloc] initWithFrame:CGRectZero];
    
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

- (void)updateHookViewForHook:(KAPInternalAccessibilityHook *)hook
{
    [[self hookOverlayView] updateHookViewForAccessibilityHook:hook];
}

- (void)updateHookViewsForHooks:(NSArray<KAPInternalAccessibilityHook *> *)hooks
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
    for(KAPInternalAccessibilityHook *hook in hooks) {
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

#pragma mark - KAPVoiceOverHookOverlayViewDelegate

- (void)triggerActivateCallbackOfHookWithID:(NSNumber *)instanceID
{
    KAPInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook selectionCallback]((int)[instanceID integerValue]);
    }
}
    
- (void)triggerIncrementCallbackOfHookWithID:(NSNumber *)instanceID
{
    KAPInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook valueChangeCallback]((int)[instanceID integerValue], 1);
    }
}
    
- (void)triggerDecrementCallbackOfHookWithID:(NSNumber *)instanceID
{
    KAPInternalAccessibilityHook *hook = [[self hookDictionary] objectForKey:instanceID];
    
    if(hook) {
        [hook valueChangeCallback]((int)[instanceID integerValue], -1);
    }
}

@end

NS_ASSUME_NONNULL_END
