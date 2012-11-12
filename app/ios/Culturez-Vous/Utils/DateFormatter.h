//
//  NSDateFormatter.h
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface DateFormatter : NSObject

+ (NSString*) getStringForDate: (NSDate*) date withFormat:(NSString*) format;

+ (NSDate*) getDateFromString: (NSString*) dateString withFormat:(NSString*) format;

@end
