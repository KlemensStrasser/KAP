//
//  NSSpeechSynthesizer+UA11YAdditions.m
//  
//
//  Created by Klemens Strasser on 03.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

#import "NSSpeechSynthesizer+UA11YAdditions.h"

NS_ASSUME_NONNULL_BEGIN

@implementation NSSpeechSynthesizer (UA11YAdditions)

+ (NSArray <NSDictionary<NSVoiceAttributeKey, id> *> *)attributesForVoices:(NSArray<NSSpeechSynthesizerVoiceName> *)voices
{
    NSMutableArray <NSDictionary<NSVoiceAttributeKey, id> *> *attribues = [[NSMutableArray alloc] initWithCapacity:[voices count]];
    for(NSSpeechSynthesizerVoiceName voice in voices) {
        [attribues addObject:[NSSpeechSynthesizer attributesForVoice:voice]];
    }
    
    return attribues;
}

@end

NS_ASSUME_NONNULL_END
