//
//  ElementManager.h
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "AppDelegate.h"
#import "Element.h"
#import "Word.h"
#import "Definition.h"

@interface ElementManager : NSObject

+ (Word*) createNewWord:(NSString*) title withDate:(NSDate*) date;

@end
