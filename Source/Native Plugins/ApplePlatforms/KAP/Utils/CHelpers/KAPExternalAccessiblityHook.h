//
//  KAPExternalAccessiblityHook.h
//  KAPMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#ifndef KAPExternalAccessiblityHook_h
#define KAPExternalAccessiblityHook_h

typedef void (*InvokeSelectionCallback)(int);
typedef void (*InvokeValueChangeCallback)(int, int);

typedef struct KAPExternalAccessibilityHook {
    int instanceID;
    float x;
    float y;
    float width;
    float height;
    const char *label;
    const char *value;
    const char *hint;
    const uint64_t trait;
    InvokeSelectionCallback selectionCallback;
    InvokeValueChangeCallback valueChangeCallback;
} KAPExternalAccessibilityHook;

#endif /* KAPExternalAccessiblityHook_h */
