//
//  KAPExternalAccessiblityHook.h
//  KAPMac
//
//  Created by Klemens Strasser on 10.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#ifndef KAPExternalAccessiblityHook_h
#define KAPExternalAccessiblityHook_h

typedef struct KAPExternalAccessibilityHook {
    int instanceID;
    float x;
    float y;
    float width;
    float height;
    const char *label;
} KAPExternalAccessibilityHook;

#endif /* KAPExternalAccessiblityHook_h */
