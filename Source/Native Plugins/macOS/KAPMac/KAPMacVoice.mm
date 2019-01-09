//
//  KAPMacVoice.m
//  KAPMac
//
//  Created by Klemens Strasser on 08.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#include "KAPMacVoice.h"
#include "KAPMacSpeechSynthesizer.h"

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
    speechSynthesizer = [[KAPMacSpeechSynthesizer alloc] init];
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
