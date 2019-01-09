//
//  KAPSpeechSynthesizer.h
//  KAPMac
//
//  Created by Klemens Strasser on 08.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/// Key for language code (ISO 639-1) as NSString value
/// This value has the highest priority
extern NSString *const kKAPSpeechSynthesizerPreferenceLanguageCode;

/// Key for gender preference KAPSpeechSynthesizerGenderPreference value
extern  NSString *const kKAPSpeechSynthesizerPreferenceGenderPreference;


typedef NS_ENUM(NSInteger, KAPSpeechSynthesizerGenderPreference) {
    KAPSpeechSynthesizerGenderPreferenceNone,
    KAPSpeechSynthesizerGenderPreferenceFemale,
    KAPSpeechSynthesizerGenderPreferenceMale,
    KAPSpeechSynthesizerGenderPreferenceNeutral
};

@protocol KAPSpeechSynthesizer

// TODO: Mabye expand this to return also the country code and gender
/// Languages available on the plattform
+(NSArray <NSString *> *)availableLanguages;

/// Convenience Initializer to configure the voice.  For available keys see kKAPSpeechSynthesizerPreference in this file
-(instancetype)initWithPreferences:(NSDictionary<NSString*,id> *)preferences;

/// Indicates if synthesis is currently running
-(BOOL)isSpeaking;

/// Indicates if synthesis is currently paused
-(BOOL)isPaused;

/// Starts syynthesis of given string
-(void)startSpeakingString:(NSString *)string;

/// Pauses synthesis immediately
-(void)pauseSpeaking;

/// Continues synthesis if if was paused before
-(void)continueSpeaking;

/// Stops synthesis immediately
-(void)stopSpeaking;

// TODO: Maybe add stopSpeakingAfterWord, stopSpeakingAfterSentence and the same for pause, if available for other systems
// TODO: Maybe expose didFinishSpeaking

/// A volume value between 0.0 and 1.0
// Based on: https://developer.apple.com/documentation/appkit/nsspeechsynthesizer/1448501-volume?language=objc
@property(nonatomic) float volume; // TODO: Find other spcifiers

/// The synthesizer’s speaking rate in words per minute.
/// A Normal human speech rate is between 180 and 220.
// Based on: https://developer.apple.com/documentation/appkit/nsspeechsynthesizer/1448450-rate?language=objc
@property(nonatomic) float rate;

// TODO: Expose language/gender. Maybe enable setting it outside of the preferences

@end

NS_ASSUME_NONNULL_END
