//
//  KAPVoiceOverBridgeViewController.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverBridgeViewController.h"
#import "KAPVoiceOverHookOverlayView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverBridgeViewController ()

@property (nonatomic, strong) KAPVoiceOverHookOverlayView *hookOverlayView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, UIView *> *hookViews;

@end

@implementation KAPVoiceOverBridgeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSMutableDictionary<NSNumber*, UIView *> *hookViews = [[NSMutableDictionary alloc] init];
    _hookViews = hookViews;
    
    // View init
    KAPVoiceOverHookOverlayView *hookOverlayView = [[KAPVoiceOverHookOverlayView alloc] initWithFrame:CGRectZero];
    [hookOverlayView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [hookOverlayView setHidden:!UIAccessibilityIsVoiceOverRunning()];
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
    [[self hookOverlayView] setHidden:!UIAccessibilityIsVoiceOverRunning()];
}

// MARK - Notifications

- (void)voiceOverStatusDidChange:(NSNotification *)notification
{
    [[self hookOverlayView] setHidden:!UIAccessibilityIsVoiceOverRunning()];
}

// MARK -

- (void)updateHookViewsForHooks:(NSArray<KAPInternalAccessibilityHook *> *)hooks
{
    // First remove all views that are not available anymore
    NSSet<NSNumber *> *validKeySet = [NSSet setWithArray:[hooks valueForKey:@"instanceID"]];
    NSArray *availableKeys = [[self hookViews] allKeys];
    
    // TODO: Remove debug stuff
    
    for(NSNumber *hookKey in availableKeys) {
        if(![validKeySet containsObject:hookKey]) {
            UIView *invalidHookView = [[self hookViews] objectForKey:hookKey];
            [invalidHookView removeFromSuperview];
            [[self hookViews] removeObjectForKey:hookKey];
            NSLog(@"Removed a view");
        }
    }
    
    // Then update/create all other hooks
    for(KAPInternalAccessibilityHook *hook in hooks) {
        UIView *hookView = [[self hookViews] objectForKey:[hook instanceID]];
        
        // Only create if needed
        if(hookView == nil) {
            hookView = [[self hookOverlayView] addHookViewWithFrame:[hook frame]];
            NSLog(@"Create a new view");
        } else {
            [hookView setFrame:[hook frame]];
            NSLog(@"Updated a view");
        }
        
        // Update values
        hookView.accessibilityLabel = [hook label];
        
        [[self hookViews] setObject:hookView forKey:[hook instanceID]];
    }
    
    [[NSNotificationCenter defaultCenter] postNotificationName:UIAccessibilityVoiceOverStatusDidChangeNotification object:nil];
}

- (void)clearAllHooks
{
    [[[self hookViews] allValues] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self hookViews] removeAllObjects];
}

@end

NS_ASSUME_NONNULL_END
