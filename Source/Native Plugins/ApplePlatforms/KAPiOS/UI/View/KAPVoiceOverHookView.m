//
//  KAPVoiceOverHookView.m
//  KAPiOS
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPVoiceOverHookView.h"

NS_ASSUME_NONNULL_BEGIN

@interface KAPVoiceOverHookView ()

@property (nonatomic, strong) UIView *backgroundView;

@property (nonatomic, strong) NSMutableArray<UIView *> *voiceOverHooks;

@end

@implementation KAPVoiceOverHookView

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
    // Init array
    
    NSMutableArray <UIView*> *voiceOverHooks = [[NSMutableArray alloc] init];
    _voiceOverHooks = voiceOverHooks;
    
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

// MARK: -

- (void)addCustomViewWithFrame:(CGRect)rect label:(NSString *)label
{
    UIView *custom = [[UIView alloc] initWithFrame:rect];
    
    [custom setIsAccessibilityElement:YES];
    [custom setAccessibilityLabel:label];
    
    [self addSubview:custom];
    [custom setBackgroundColor:[UIColor colorWithWhite:1.0 alpha:0.25]];
}

- (void)clearAllElements
{
    [[self voiceOverHooks] makeObjectsPerformSelector:@selector(removeFromSuperview)];
    [[self voiceOverHooks] removeAllObjects];
}

@end

NS_ASSUME_NONNULL_END
