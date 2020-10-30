//
//  UA11YMacSpeechSynthesizer.m
//  
//
//  Created by Klemens Strasser on 03.01.19.
//

#import "UA11YMacSpeechSynthesizer.h"
@import AppKit;

NS_ASSUME_NONNULL_BEGIN

@interface UA11YMacSpeechSynthesizer () {
    BOOL _paused;
}

@property (nonatomic, readonly) NSSpeechSynthesizer *speechSynthesizer;

@end

@implementation UA11YMacSpeechSynthesizer

#pragma mark - Class methods

+ (NSArray<NSString *> *)availableLanguages
{
    NSMutableSet<NSString *> *languageIdentifiers = [[NSMutableSet alloc] init];
    
    for(NSSpeechSynthesizerVoiceName voice in [NSSpeechSynthesizer availableVoices]) {
        NSDictionary<NSVoiceAttributeKey, id> *attribues = [NSSpeechSynthesizer attributesForVoice:voice];
        NSString *localeIdentifier = [attribues valueForKey:NSVoiceLocaleIdentifier];
        
        NSLocale *locale = [NSLocale localeWithLocaleIdentifier:localeIdentifier];
        
        // TODO: Maybe expose the countryCode too - But that might not possible for all platforms
        [languageIdentifiers addObject:[locale languageCode]];
    }
    
    return [languageIdentifiers allObjects];
}

#pragma mark - Initializer

- (instancetype)init
{
    self = [super init];
    if (self) {
        // nil = defaultVoice
        _speechSynthesizer = [[NSSpeechSynthesizer alloc] initWithVoice:nil];
    }
    return self;
}

- (instancetype)initWithPreferences:(NSDictionary<NSString *,id> *)preferences {
    self = [super init];
    if (self) {
        NSSpeechSynthesizerVoiceName voice = [self bestMatchingVoiceForPreferences:preferences];
        _speechSynthesizer = [[NSSpeechSynthesizer alloc] initWithVoice:voice];
    }
    return self;
}

        
#pragma mark - Properties

- (void)setVolume:(float)volume
{
    [[self speechSynthesizer] setVolume:(volume < 0.0 || volume > 1.0) ? 0.5 : volume];
}

- (float)volume
{
    return  [self speechSynthesizer] ? [[self speechSynthesizer] volume] : 0.0;
}

- (void)setRate:(float)rate
{
    [[self speechSynthesizer] setRate:rate];
}

- (float)rate
{
    return [self speechSynthesizer] ? [[self speechSynthesizer] rate] : 0.0;
}

#pragma mark - Speaking

-(BOOL)isSpeaking
{
    return [[self speechSynthesizer] isSpeaking];
}

- (BOOL)isPaused {
    return _paused;
}

-(void)startSpeakingString:(NSString *)string
{
    [[self speechSynthesizer] startSpeakingString:string];
    _paused = NO;
}

-(void)pauseSpeaking
{
    if([self isSpeaking]) {
        [[self speechSynthesizer] pauseSpeakingAtBoundary:NSSpeechImmediateBoundary];
        _paused = YES;
    }
}

-(void)continueSpeaking
{
    [[self speechSynthesizer] continueSpeaking];
    _paused = NO;
}

-(void)stopSpeaking
{
    [[self speechSynthesizer] stopSpeaking];
    _paused = NO;
}

#pragma mark - Helpers
-(NSSpeechSynthesizerVoiceName)bestMatchingVoiceForPreferences:(NSDictionary<NSString *,id> *)preferences
{
    NSSpeechSynthesizerVoiceName selectedVoice;
    
    if([preferences count] > 0) {
        NSArray<NSSpeechSynthesizerVoiceName> *availableVoices = [NSSpeechSynthesizer availableVoices];
        
        NSString *preferredLanguageCode = [preferences objectForKey:kUA11YSpeechSynthesizerPreferenceLanguageCode];
        
        if(preferredLanguageCode != nil) {
            NSArray<NSSpeechSynthesizerVoiceName> *languageFilteredVoices = [availableVoices filteredArrayUsingPredicate:[NSPredicate predicateWithBlock:^BOOL(NSSpeechSynthesizerVoiceName voice, NSDictionary *bindings) {
                NSDictionary<NSVoiceAttributeKey, id> *attribues = [NSSpeechSynthesizer attributesForVoice:voice];
                NSString *localeIdentifier = [attribues valueForKey:NSVoiceLocaleIdentifier];
                
                NSLocale *locale = [NSLocale localeWithLocaleIdentifier:localeIdentifier];
                
                return [preferredLanguageCode isEqualToString:[locale languageCode]];
            }]];
            
            if([languageFilteredVoices count] > 0) {
                availableVoices = languageFilteredVoices;
            }
        }
        
        UA11YSpeechSynthesizerGenderPreference preferredGender = [[preferences objectForKey:kUA11YSpeechSynthesizerPreferenceGenderPreference] integerValue];
        
        if(preferredGender > UA11YSpeechSynthesizerGenderPreferenceNone && preferredGender <= UA11YSpeechSynthesizerGenderPreferenceNeutral) {
            NSArray<NSSpeechSynthesizerVoiceName> *genderFilteredVoices = [availableVoices filteredArrayUsingPredicate:[NSPredicate predicateWithBlock:^BOOL(NSSpeechSynthesizerVoiceName voice, NSDictionary *bindings) {
                NSDictionary<NSVoiceAttributeKey, id> *attribues = [NSSpeechSynthesizer attributesForVoice:voice];
                NSString *gender = [attribues valueForKey:NSVoiceGender];
                
                return ([gender isEqualToString: NSVoiceGenderFemale] && preferredGender == UA11YSpeechSynthesizerGenderPreferenceFemale) || ([gender isEqualToString: NSVoiceGenderMale] && preferredGender == UA11YSpeechSynthesizerGenderPreferenceMale) || ([gender isEqualToString: NSVoiceGenderNeuter] && preferredGender == UA11YSpeechSynthesizerGenderPreferenceNeutral);
            }]];
            
            if([genderFilteredVoices count] > 0) {
                availableVoices = genderFilteredVoices;
            }
        }
        
        selectedVoice = [availableVoices count] > 0 ? [availableVoices firstObject] : [NSSpeechSynthesizer defaultVoice];
    } else {
        selectedVoice = [NSSpeechSynthesizer defaultVoice];
    }

    return selectedVoice;
}

@end

NS_ASSUME_NONNULL_END
