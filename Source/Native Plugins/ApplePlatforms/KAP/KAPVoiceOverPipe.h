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
    
    struct KAPAccessibilityHook {
        int instanceID;
        float x;
        float y;
        float width;
        float height;
        const char *label;
    };
    
    bool KAPIsScreenReaderRunning();
    
    void KAPAddHook(KAPAccessibilityHook);
    void KAPAddHooks(KAPAccessibilityHook*, int);
    
    void KAPClearAllHooks();
}
