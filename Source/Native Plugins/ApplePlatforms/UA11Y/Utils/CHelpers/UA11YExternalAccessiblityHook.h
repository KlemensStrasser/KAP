//
//  UA11YExternalAccessiblityHook.h
//  UA11YMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#ifndef UA11YExternalAccessiblityHook_h
#define UA11YExternalAccessiblityHook_h

typedef void (*InvokeSelectionCallback)(int);
typedef void (*InvokeFocusCallback)(int);
typedef void (*InvokeValueChangeCallback)(int, int);

typedef struct UA11YExternalAccessibilityHook {
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
    InvokeFocusCallback focusCallback;
    InvokeValueChangeCallback valueChangeCallback;
} UA11YExternalAccessibilityHook;

#endif /* UA11YExternalAccessiblityHook_h */
