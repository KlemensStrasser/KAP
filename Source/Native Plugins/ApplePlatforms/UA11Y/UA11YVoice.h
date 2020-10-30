//
//  UA11YVoice.h
//  UA11Y
//
//  Created by Klemens Strasser on 08.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

extern "C" {
    void VoiceSetup();
    
    void VoiceStartSpeaking(const char *);
    void VoicePauseSpeaking();
    void VoiceContinueSpeaking();
    void VoiceStopSpeaking();
    
    bool VoiceIsSpeaking();
    bool VoiceIsPaused();
}
