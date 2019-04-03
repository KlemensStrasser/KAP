//
//  KAPiOSSpeechSynthesizer.m
//  KAPMac
//
//  Created by Klemens Strasser on 02.04.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "KAPiOSSpeechSynthesizer.h"
@import AVFoundation;

NS_ASSUME_NONNULL_BEGIN

@interface KAPiOSSpeechSynthesizer ()

@property (nonatomic, readonly) AVSpeechSynthesizer *speechSynthesizer;
@property (nonatomic, readonly, nullable) AVSpeechSynthesisVoice *voice;

@end

@implementation KAPiOSSpeechSynthesizer

@synthesize volume;
@synthesize rate;

#pragma mark - Class methods

+ (NSArray<NSString *> *)availableLanguages
{
    NSMutableSet<NSString *> *languageIdentifiers = [[NSMutableSet alloc] init];
    
    for(AVSpeechSynthesisVoice *voice in [AVSpeechSynthesisVoice speechVoices]) {
        // TODO: Check if this language code is the same as on mac.
        // https://en.wikipedia.org/wiki/IETF_language_tag
        [languageIdentifiers addObject:[voice language]];
    }
    
    return [languageIdentifiers allObjects];
}

#pragma mark - Initializer

- (instancetype)init
{
    self = [super init];
    if (self) {
        _speechSynthesizer = [[AVSpeechSynthesizer alloc] init];
        
        // Set default values here.
        volume = 1.0;
        rate = 0.5;
    }
    return self;
}

- (instancetype)initWithPreferences:(NSDictionary<NSString *,id> *)preferences {
    self = [super init];
    if (self) {
        AVSpeechSynthesisVoice *voice = [self bestMatchingVoiceForPreferences:preferences];
        AVSpeechSynthesizer *synthezier = [[AVSpeechSynthesizer alloc] init];
        
        _voice = voice;
        _speechSynthesizer = synthezier;
        
        // Set default values here.
        volume = 1.0;
        rate = 0.5;
    }
    return self;
}

#pragma mark - Speaking

-(BOOL)isSpeaking
{
    return [[self speechSynthesizer] isSpeaking];
}

- (BOOL)isPaused {
    return [[self speechSynthesizer] isPaused];
}

-(void)startSpeakingString:(NSString *)string
{
    [[self speechSynthesizer] stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
    
    if(string != nil) {
        AVSpeechUtterance *utterance = [[AVSpeechUtterance alloc] initWithString:string];
        [utterance setRate:[self rate]];
        [utterance setVolume:[self volume]];
        
        [[self speechSynthesizer] speakUtterance:utterance];
    }
}

-(void)pauseSpeaking
{
    if([self isSpeaking]) {
        [[self speechSynthesizer] pauseSpeakingAtBoundary:AVSpeechBoundaryImmediate];
    }
}

-(void)continueSpeaking
{
    [[self speechSynthesizer] continueSpeaking];
}

-(void)stopSpeaking
{
    [[self speechSynthesizer] stopSpeakingAtBoundary:AVSpeechBoundaryImmediate];
}

#pragma mark - Helpers
-(AVSpeechSynthesisVoice *)bestMatchingVoiceForPreferences:(NSDictionary<NSString *,id> *)preferences
{
    AVSpeechSynthesisVoice *selectedVoice;
    
    if([preferences count] > 0) {
        NSArray<AVSpeechSynthesisVoice *> *availableVoices = [AVSpeechSynthesisVoice speechVoices];
        
        NSString *preferredLanguageCode = [preferences objectForKey:kKAPSpeechSynthesizerPreferenceLanguageCode];
        
        if(preferredLanguageCode != nil) {
            NSArray<AVSpeechSynthesisVoice *> *languageFilteredVoices = [availableVoices filteredArrayUsingPredicate:[NSPredicate predicateWithBlock:^BOOL(AVSpeechSynthesisVoice *voice, NSDictionary *bindings) {
                return [[voice language] isEqualToString:preferredLanguageCode];
            }]];
            
            if([languageFilteredVoices count] > 0) {
                availableVoices = languageFilteredVoices;
            }
        }
        
        KAPSpeechSynthesizerGenderPreference preferredGender = [[preferences objectForKey:kKAPSpeechSynthesizerPreferenceGenderPreference] integerValue];
        
        if(preferredGender > KAPSpeechSynthesizerGenderPreferenceNone && preferredGender <= KAPSpeechSynthesizerGenderPreferenceNeutral) {
            // TODO: Check if we can filter here by gender or not
        }
        
        selectedVoice = [availableVoices count] > 0 ? [availableVoices firstObject] : [[AVSpeechSynthesisVoice speechVoices] firstObject];
    } else {
        selectedVoice = [[AVSpeechSynthesisVoice speechVoices] firstObject];
    }
    
    return selectedVoice;
}


@end

NS_ASSUME_NONNULL_END
