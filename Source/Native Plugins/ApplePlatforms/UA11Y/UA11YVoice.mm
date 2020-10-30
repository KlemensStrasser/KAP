//
//  UA11YVoice.m
//  UA11Y
//
//  Created by Klemens Strasser on 08.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#include "UA11YVoice.h"
#include "Utils/CHelpers/UA11YStringConversion.mm"
#ifdef TARGET_MAC
#include "UA11YMacSpeechSynthesizer.h"
#else
#include "UA11YiOSSpeechSynthesizer.h"
#endif

id<UA11YSpeechSynthesizer> speechSynthesizer;

void VoiceSetup()
{
        // TODO: Add Preferences
#ifdef TARGET_MAC
    speechSynthesizer = [[UA11YMacSpeechSynthesizer alloc] init];
#else
    speechSynthesizer = [[UA11YiOSSpeechSynthesizer alloc] init];
#endif
}

void VoiceStartSpeaking(const char *cString)
{
    NSString *text = NSStringFromCString(cString);
    
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
