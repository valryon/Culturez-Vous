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
#import "Constants.h"

@interface ElementCache : NSObject

+ (Word*) createNewWordNoContext;
+ (Definition*) createNewDefinitionNoContext;
+ (Contrepeterie*) createNewContreperieNoContext;

- (BOOL) saveCache;

- (NSArray*) getElements:(NSString*)type withPage:(int) page;

- (NSArray *)getAllElements:(NSString*)type;

- (NSFetchRequest*) prepareFetchRequest:(NSString*) elementType;

- (NSFetchRequest*) prepareFetchRequestPaginated:(NSString*) elementType forPage:(int)page;

@end
