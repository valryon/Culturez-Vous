//
//  ElementCache.h
//  Culturez-Vous
//
//  Created by Dam on 02/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "AppDelegate.h"
#import "Element.h"
#import "Word.h"
#import "Definition.h"
#import "Contrepeterie.h"

@interface ElementCache : NSObject

- (Definition*)createNewDefinition:(NSString *)details withContent:(NSString *)content;

- (Word*) createNewWord:(NSString*) title withDate:(NSDate*) date;

- (Contrepeterie*)createNewContrepeterie:(NSString *)title withDate:(NSDate *)date withContent:(NSString*) content withSolution:(NSString*) solution;

- (BOOL) saveCache;

@end
