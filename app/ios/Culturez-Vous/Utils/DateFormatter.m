//
//  NSDateFormatter.m
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "DateFormatter.h"

@implementation DateFormatter

+ (NSString*) getStringForDate: (NSDate*) date withFormat:(NSString*) format
{
    NSDateFormatter *dateFormatter = nil;
    dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:format];
    
    return [dateFormatter stringFromDate:date];
}

+ (NSDate*) getDateFromString: (NSString*) dateString withFormat:(NSString*) format
{
    NSDateFormatter *dateFormat;
    dateFormat = [[NSDateFormatter alloc] init];
    [dateFormat setDateFormat:format];
    
    return [dateFormat dateFromString:dateString];
}

@end
