//
//  KAPStringConversion.m
//  KAPMac
//
//  Created by Klemens Strasser on 08.05.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#ifndef KAPStringConversion_h
#define KAPStringConversion_h

#include <Foundation/Foundation.h>

NSString *NSStringFromCString(const char *cString)
{
    if(cString) {
        return [NSString stringWithUTF8String:cString];
    } else {
        return @"";
    }
}

#endif
