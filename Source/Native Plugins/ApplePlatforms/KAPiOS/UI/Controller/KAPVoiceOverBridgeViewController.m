//
//  KAPVoiceOverBridgeViewController.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverBridgeViewController.h"
#import "KAPVoiceOverHookOverlayView.h"
#import "KAPVoiceOverHookView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverBridgeViewController () <KAPVoiceOverHookViewDelegate>

@property (nonatomic, strong) KAPVoiceOverHookOverlayView *hookOverlayView;
@property (nonatomic, strong) NSMutableDictionary<NSNumber*, KAPVoiceOverHookView *> *hookViews;

@end

@implementation KAPVoiceOverBridgeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSMutableDictionary<NSNumber*, KAPVoiceOverHookView *> *hookViews = [[NSMutableDictionary alloc] init];
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

# pragma mark - Notifications

- (void)voiceOverStatusDidChange:(NSNotification *)notification
{
    [[self hookOverlayView] setHidden:!UIAccessibilityIsVoiceOverRunning()];
}

# pragma mark -

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
        KAPVoiceOverHookView *hookView = [[self hookViews] objectForKey:[hook instanceID]];
        
        // Only create if needed
        if(hookView == nil) {
            hookView = [[self hookOverlayView] addHookViewWithFrame:[hook frame] instanceID:[hook instanceID]];
            NSLog(@"Create a new view");
        } else {
            [hookView setFrame:[hook frame]];
            NSLog(@"Updated a view");
        }
        
        [hookView setDelegate:self];
        
        // Update values
        [hookView setAccessibilityLabel:[hook label]];
        [hookView setAccessibilityValue:[hook value]];
        [hookView setAccessibilityHint:[hook hint]];
        [hookView setInvokeSelectionCallback:[hook selectionCallback]];
        
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
