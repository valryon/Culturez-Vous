//
//  ElementCache.m
//  Culturez-Vous
//
//  Created by Dam on 02/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementCache.h"

@implementation ElementCache

+ (Word*) createNewWordNoContext
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSEntityDescription *entity = [[app.managedObjectModel entitiesByName] objectForKey:@"Word"];
    
    Word *word = [[Word alloc] initWithEntity:entity
               insertIntoManagedObjectContext:nil];
    
    return word;
}
+ (Definition*) createNewDefinitionNoContext
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSEntityDescription *entity = [[app.managedObjectModel entitiesByName] objectForKey:@"Definition"];
    
    Definition *def = [[Definition alloc] initWithEntity:entity
                          insertIntoManagedObjectContext:nil];
    
    return def;
}
+ (Contrepeterie*) createNewContreperieNoContext
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSEntityDescription *entity = [[app.managedObjectModel entitiesByName] objectForKey:@"Contrepeterie"];
    
    Contrepeterie *ctp = [[Contrepeterie alloc] initWithEntity:entity
                                insertIntoManagedObjectContext:nil];
    
    return ctp;
}


//- (id)createNewWord:(NSString *)title withDate:(NSDate *)date
//{
//    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
//
//    // Création d'un nouveau mot à partir du cache
//    Word *word = (Word *)[NSEntityDescription insertNewObjectForEntityForName:@"Word" inManagedObjectContext:app.managedObjectContext];
//
//    if(word != NULL) {
//        // Mise à jour des données
//        [word setTitle:title];
//        [word setDate:date];
//    }
//    return word;
//}

#pragma Insertion objets

- (void) insertElement:(Element*)element
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Pour les mots, on transforme les définitions temporaire en définitions pour le cache
    if([element isKindOfClass:[Word class]])
    {
        Word *w = (Word*)element;
        
        // Puis remplir le champ definitions
        [w addDefinitions:w.tempDefinitions];
        
        // Et vider les définitions temporaires
        [w removeTempDefinitions:w.tempDefinitions];
    }
    
    [app.managedObjectContext insertObject:element];
}

#pragma Sauvegarde du cache

-(BOOL)saveCache
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Sauvegarde du cache
    NSError *error = nil;
    if (![app.managedObjectContext save:&error]) {
        NSLog(@"%@", [error localizedDescription]);
        return false;
    }
    
    return true;
}

#pragma Recherche dans le cache

#pragma Préparation des requêtes

- (NSFetchRequest*) prepareFetchRequest:(NSString*) elementType
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Récupération d'éléments
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:elementType inManagedObjectContext:app.managedObjectContext];
    
    // Création d'une requête
    NSFetchRequest *request = [[NSFetchRequest alloc] init];
    [request setEntity:entityDescription];
    
    // Tri par date
    NSSortDescriptor *sortDescriptor = [[NSSortDescriptor alloc]
                                        initWithKey:@"date" ascending:NO];
    [request setSortDescriptors:@[sortDescriptor]];
    
    return request;
}

- (NSFetchRequest*) prepareFetchRequestPaginated:(NSString*) elementType fromPage:(int)pageFrom toPage:(int) pageTo
{
    // Requête standard
    NSFetchRequest *request = [self prepareFetchRequest:elementType];
    
    // Pagination
    request.fetchOffset = pageFrom * ELEMENTS_PER_PAGE; // Index de début
    request.fetchLimit = ((pageTo - pageFrom) + 1) * ELEMENTS_PER_PAGE; // Taille de la page
    
    return request;
}

- (NSFetchRequest*) prepareFetchRequestTitleSearch:(NSString*) title
{
    // Requête standard
    NSFetchRequest *request = [self prepareFetchRequest:@"Element"];
    
    // Recherche fulltext
    NSPredicate *predicate = [NSPredicate predicateWithFormat:@"title == %@", title];
    
    [request setPredicate:predicate];
    
    return request;
}

- (NSFetchRequest*) prepareFetchRequestIdSearch:(NSNumber*) dbId
{
    // Requête standard
    NSFetchRequest *request = [self prepareFetchRequest:@"Element"];
    
    // Recherche fulltext
    NSPredicate *predicate = [NSPredicate predicateWithFormat:@"dbId == %@", dbId];
    
    [request setPredicate:predicate];
    
    return request;
}

#pragma Fonctions de recherche

- (NSArray*) getElements:(NSString*)type fromPage:(int)pageFrom toPage:(int) pageTo

{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSFetchRequest *request = [self prepareFetchRequestPaginated:type fromPage:pageFrom toPage:pageTo];
    
    NSError *error;
    NSArray *array = [app.managedObjectContext executeFetchRequest:request error:&error];
    if (array == nil)
    {
        NSLog(@"%@", [error localizedDescription]);
        return NULL;
    }
    
    return array;
}

- (NSArray *)getAllElements:(NSString*)type
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSFetchRequest *request = [self prepareFetchRequest:type];
    
    NSError *error;
    NSArray *array = [app.managedObjectContext executeFetchRequest:request error:&error];
    if (array == nil)
    {
        NSLog(@"%@", [error localizedDescription]);
        return NULL;
    }
    
    return array;
}

- (BOOL)existsWithId:(NSNumber*)dbId
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSFetchRequest *request = [self prepareFetchRequestIdSearch:dbId];
    
    NSError *error;
    NSArray *array = [app.managedObjectContext executeFetchRequest:request error:&error];
    if (array == nil)
    {
        NSLog(@"%@", [error localizedDescription]);
        return false;
    }
    
    return array.count > 0;
}

@end
