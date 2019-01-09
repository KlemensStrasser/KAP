//
//  NSSpeechSynthesizer+KAPAdditions.h
//  
//
//  Created by Klemens Strasser on 03.01.19.
//  Copyright © 2019 KlemensStrasser. All rights reserved.
//

@import AppKit;

NS_ASSUME_NONNULL_BEGIN

@interface NSSpeechSynthesizer (KAPAdditions)

+ (NSArray <NSDictionary<NSVoiceAttributeKey, id> *> *)attributesForVoices:(NSArray<NSSpeechSynthesizerVoiceName> *)voices;

@end

NS_ASSUME_NONNULL_END
