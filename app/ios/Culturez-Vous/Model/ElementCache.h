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
#import "Common.h"
#import "ElementContextHelper.h"

@interface ElementCache : NSOperation

//
// Création d'éléments
//
+ (Word*) createNewWord:(NSManagedObjectContext *) context;
+ (Definition*) createNewDefinition:(NSManagedObjectContext *) context;
+ (Contrepeterie*) createNewContreperie:(NSManagedObjectContext *) context;

//
// Insertion d'un élément dans le cache
//
- (void) insertElement:(Element*)element;

//
// Récupération d'éléments avec pagination
//
- (NSArray*) getElements:(NSString*)type forPage:(int)page;

//
// Récupération du nombre d'éléments pour la page indiquée
//
- (int) getElementsCount:(NSString*)type forPage:(int)page;

//
// Lecture entière du cache
//
- (NSArray *)getAllElements:(NSString*)type;

//
// Teste l'existence d'un élément dans le cache par son titre
//
- (BOOL)existsWithId:(NSNumber*)dbId;

@end
