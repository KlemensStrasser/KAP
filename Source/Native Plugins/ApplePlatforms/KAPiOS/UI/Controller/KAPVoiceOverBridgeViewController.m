//
//  KAPVoiceOverBridgeViewController.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverBridgeViewController.h"
#import "KAPVoiceOverHookView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverBridgeViewController ()

@property (nonatomic, strong) KAPVoiceOverHookView *hookView;

@end

@implementation KAPVoiceOverBridgeViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    KAPVoiceOverHookView *hookView = [[KAPVoiceOverHookView alloc] initWithFrame:CGRectZero];
    [hookView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [hookView setHidden:!UIAccessibilityIsVoiceOverRunning()];
    [[self view] addSubview:hookView];
    
    _hookView = hookView;
    
    // Constraints for hook view
    [[[hookView leadingAnchor] constraintEqualToAnchor:[[self view] leadingAnchor]] setActive:YES];
    [[[hookView trailingAnchor] constraintEqualToAnchor:[[self view] trailingAnchor]] setActive:YES];
    [[[hookView topAnchor] constraintEqualToAnchor:[[self view] topAnchor]] setActive:YES];
    [[[hookView bottomAnchor] constraintEqualToAnchor:[[self view] bottomAnchor]] setActive:YES];
    
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
    [[self hookView] setHidden:!UIAccessibilityIsVoiceOverRunning()];
}

// MARK - Notifications

- (void)voiceOverStatusDidChange:(NSNotification *)notification
{
    [[self hookView] setHidden:!UIAccessibilityIsVoiceOverRunning()];
}

// MARK -

- (void)addCustomViewWithFrame:(CGRect)rect label:(NSString *)label
{
    [[self hookView] addCustomViewWithFrame:rect label:label];
}

- (void)clearAllElements
{
    [[self hookView] clearAllElements];
}

@end

NS_ASSUME_NONNULL_END
