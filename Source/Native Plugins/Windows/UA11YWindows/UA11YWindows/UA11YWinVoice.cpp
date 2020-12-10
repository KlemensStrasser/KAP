#include "pch.h"
#include "UA11YWinVoice.h"
#include <sapi.h>
#include <atlstr.h>

ISpVoice* pVoice = NULL;

// TODO: This is never getting released, which makes me nervous
DllExport void VoiceSetup()
{
    HRESULT hr = CoCreateInstance(CLSID_SpVoice, NULL, CLSCTX_ALL, IID_ISpVoice, (void**)&pVoice);
    if (SUCCEEDED(hr))
    {
        // success!
    }
}

DllExport void VoiceStartSpeaking(const char* cString)
{
    if (pVoice != NULL)
    {
        CString str = CString(cString);
        USES_CONVERSION;
        LPCWSTR lString = A2CW(W2A(str));
        str.ReleaseBuffer();
        pVoice->Speak(lString, SPEAKFLAGS::SPF_ASYNC | SPEAKFLAGS::SPF_PURGEBEFORESPEAK, NULL);
    }
}

DllExport void VoicePauseSpeaking()
{
    if (pVoice != NULL)
    {
        pVoice->Pause();
    }
}

DllExport void VoiceContinueSpeaking()
{
    if (pVoice != NULL)
    {
        pVoice->Resume();
    }
}

DllExport void VoiceStopSpeaking()
{
    if (pVoice != NULL)
    {
        // TODO: Implement
    }
}

DllExport bool VoiceIsSpeaking()
{
    return false; // TODO: Implement
}

DllExport bool VoiceIsPaused()
{
    return false; // TODO: Implement
}
