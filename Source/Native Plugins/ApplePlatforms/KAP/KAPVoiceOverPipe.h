//
//  KAPVoiceOverPipe.h
//  KAPMac
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

// TODO: Callback for Activation
//typedef void (*INT_CALLBACK)(int);

extern "C" {
    
    #include "Utils/CHelpers/KAPExternalAccessiblityHook.h"
    
    bool KAPIsScreenReaderRunning();
    
    void KAPAddHook(KAPExternalAccessibilityHook);
    void KAPUpdateHooks(KAPExternalAccessibilityHook*, int);
    
    void KAPClearAllHooks();
}
