//
//  UA11YVoiceOverPipe.h
//  UA11YMac
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

extern "C" {
    
    #include "Utils/CHelpers/UA11YExternalAccessiblityHook.h"
    
    bool UA11YIsScreenReaderRunning();
    
    void UA11YUpdateHook(UA11YExternalAccessibilityHook);
    void UA11YUpdateHooks(UA11YExternalAccessibilityHook*, int);
    void UA11YClearAllHooks();
    
    void UA11YAnnoucnceVoiceOverMessage(const char *);
}
