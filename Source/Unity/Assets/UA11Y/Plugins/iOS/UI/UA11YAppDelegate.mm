//
//  UA11YAppDelegate.m
//
//
//  Created by Klemens Strasser on 07.05.19.
//

#import "UA11YAppDelegate.h"
#import "UA11YVoiceOverBridgeViewController.h"

NS_ASSUME_NONNULL_BEGIN

@implementation UA11YAppDelegate

@end

@implementation UA11YAppDelegate(OverrideAppDelegate)

extern const char* AppControllerClassName;

+(void)load
{
    AppControllerClassName = [@"UA11YAppDelegate" UTF8String];
}

- (void)willStartWithViewController:(UIViewController*)controller
{
    UA11YVoiceOverBridgeViewController *bridgeViewController = [[UA11YVoiceOverBridgeViewController alloc] init];
    _bridgeViewController = bridgeViewController;
    
    // Set root
    _rootController = bridgeViewController;
    _rootView = _rootController.view;
    
    // Initialize Unity view
    UIView *unityView = UnityGetGLView();
    [unityView setIsAccessibilityElement:NO];
    [unityView setTranslatesAutoresizingMaskIntoConstraints:NO];

    // Add Unity view to the back
    [_rootController.view addSubview:unityView];
    [_rootController.view sendSubviewToBack:unityView];
    
    // Set constraints
    [[[unityView leadingAnchor] constraintEqualToAnchor:[[bridgeViewController view] leadingAnchor]] setActive:YES];
    [[[unityView trailingAnchor] constraintEqualToAnchor:[[bridgeViewController view] trailingAnchor]] setActive:YES];
    [[[unityView topAnchor] constraintEqualToAnchor:[[bridgeViewController view] topAnchor]] setActive:YES];
    [[[unityView bottomAnchor] constraintEqualToAnchor:[[bridgeViewController view] bottomAnchor]] setActive:YES];
}
@end

NS_ASSUME_NONNULL_END
