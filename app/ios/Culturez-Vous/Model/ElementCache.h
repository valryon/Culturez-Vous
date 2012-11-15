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

@interface ElementCache : NSObject

//
// Création d'éléments non attachés à un contexte
//
+ (Word*) createNewWordNoContext;
+ (Definition*) createNewDefinitionNoContext;
+ (Contrepeterie*) createNewContreperieNoContext;

//
// Insertion d'un élément dans le cache
//
- (void) insertElement:(Element*)element;

//
// Sauvegarde globale du cache
//
- (BOOL) saveCache;

//
// Récupération d'éléments avec pagination
//
- (NSArray*) getElements:(NSString*)type fromPage:(int)pageFrom toPage:(int) pageTo;

//
// Lecture entière du cache
//
- (NSArray *)getAllElements:(NSString*)type;

//
// Teste l'existence d'un élément dans le cache par son titre
//
- (BOOL)existsWithId:(NSNumber*)dbId;

@end
