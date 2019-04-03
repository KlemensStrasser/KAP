//
//  KAPVoice.m
//  KAP
//
//  Created by Klemens Strasser on 08.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#include "KAPVoice.h"
#ifdef TARGET_MAC
#include "KAPMacSpeechSynthesizer.h"
#else
#include "KAPiOSSpeechSynthesizer.h"
#endif

// TODO: Probably put that somewhere else
NSString *stringFromCString(const char *cString)
{
    if(cString) {
        return [NSString stringWithUTF8String:cString];
    } else {
        return @"";
    }
}

id<KAPSpeechSynthesizer> speechSynthesizer;

void VoiceSetup()
{
        // TODO: Add Preferences
#ifdef TARGET_MAC
    speechSynthesizer = [[KAPMacSpeechSynthesizer alloc] init];
#else
    speechSynthesizer = [[KAPiOSSpeechSynthesizer alloc] init];
#endif
}

void VoiceStartSpeaking(const char *cString)
{
    NSString *text = stringFromCString(cString);
    
    if(!speechSynthesizer) {
        VoiceSetup();
    }
    
    [speechSynthesizer startSpeakingString:text];
}

void VoicePauseSpeaking()
{
    [speechSynthesizer pauseSpeaking];
}

void VoiceContinueSpeaking()
{
    [speechSynthesizer continueSpeaking];
}

void VoiceStopSpeaking()
{
    [speechSynthesizer stopSpeaking];
}

bool VoiceIsSpeaking()
{
    return [speechSynthesizer isSpeaking];
}

bool VoiceIsPaused()
{
    return [speechSynthesizer isPaused];
}
