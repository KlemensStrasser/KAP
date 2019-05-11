//
//  KAPVoiceOverHookView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookOverlayView.h"
#import "KAPVoiceOverHookView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookOverlayView ()

@property (nonatomic, strong) UIView *backgroundView;

@end

@implementation KAPVoiceOverHookOverlayView

- (instancetype)init
{
    self = [super init];
    if (self) {
        [self setupView];
    }
    return self;
}

- (instancetype)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        [self setupView];
    }
    return self;
}

- (void)setupView
{   
    // Add background
    UIView *backgroundView = [[UIView alloc] initWithFrame:CGRectZero];
    [backgroundView setTranslatesAutoresizingMaskIntoConstraints:NO];
    [backgroundView setBackgroundColor:[UIColor colorWithWhite:0.0 alpha:0.2]];
    
    [self addSubview:backgroundView];
    _backgroundView = backgroundView;
    
    [[[backgroundView leadingAnchor] constraintEqualToAnchor:[self leadingAnchor]] setActive:YES];
    [[[backgroundView trailingAnchor] constraintEqualToAnchor:[self trailingAnchor]] setActive:YES];
    [[[backgroundView topAnchor] constraintEqualToAnchor:[self topAnchor]] setActive:YES];
    [[[backgroundView bottomAnchor] constraintEqualToAnchor:[self bottomAnchor]] setActive:YES];
}

# pragma mark -

- (KAPVoiceOverHookView *)addHookViewWithFrame:(CGRect)frame instanceID:(NSNumber *)instanceID
{
    KAPVoiceOverHookView *hookView = [[KAPVoiceOverHookView alloc] initWithFrame:frame instanceID:instanceID];
    
    [hookView setIsAccessibilityElement:YES];
    [hookView setBackgroundColor:[UIColor colorWithWhite:1.0 alpha:0.25]];
    
    [self addSubview:hookView];

    return hookView;
}

@end

NS_ASSUME_NONNULL_END
