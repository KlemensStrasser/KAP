// #pragma once

#include "pch.h"

#define DllExport __declspec (dllexport)

extern "C" {
    DllExport void VoiceSetup();

    DllExport void VoiceStartSpeaking(const char*);
    DllExport void VoicePauseSpeaking();
    DllExport void VoiceContinueSpeaking();
    DllExport void VoiceStopSpeaking();

    DllExport  bool VoiceIsSpeaking();
    DllExport bool VoiceIsPaused();
}