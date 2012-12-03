//
//  ElementCache.m
//  Culturez-Vous
//
//  Created by Dam on 02/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementCache.h"

@implementation ElementCache

+ (Word*) createNewWord:(NSManagedObjectContext *) context
{
    NSEntityDescription *entity = [[[ElementContextHelper managedObjectModel] entitiesByName] objectForKey:@"Word"];
    
    Word *word = [[Word alloc] initWithEntity:entity
               insertIntoManagedObjectContext:context];
    
    return word;
}
+ (Definition*) createNewDefinition:(NSManagedObjectContext *) context
{
    NSEntityDescription *entity = [[[ElementContextHelper managedObjectModel]entitiesByName] objectForKey:@"Definition"];
    
    Definition *def = [[Definition alloc] initWithEntity:entity
                          insertIntoManagedObjectContext:context];
    
    return def;
}
+ (Contrepeterie*) createNewContreperie:(NSManagedObjectContext *) context
{
    NSEntityDescription *entity = [[[ElementContextHelper managedObjectModel] entitiesByName] objectForKey:@"Contrepeterie"];
    
    Contrepeterie *ctp = [[Contrepeterie alloc] initWithEntity:entity
                                insertIntoManagedObjectContext:context];
    
    return ctp;
}

#pragma Insertion objets

- (void) insertElement:(Element*)element
{
    NSManagedObjectID *objectID = [element objectID];
    
    [ElementContextHelper saveDataInContext:^(NSManagedObjectContext *localContext)
    {
        Element *localElement = (Element *)[localContext objectWithID:objectID];
        
        // Insertion dans le contexte
        [localContext insertObject:localElement];
        
    }];
}

- (void) insertElements:(NSArray*)elements
{
    NSMutableArray *objectsIDs = [[NSMutableArray alloc] init];
    
    for (Element *element in elements)
    {
        [objectsIDs addObject:[element objectID]];
    }
    
    [ElementContextHelper saveDataInContext:^(NSManagedObjectContext *localContext)
     {
         for (NSManagedObjectID *oid in objectsIDs)
         {
             Element *localElement = (Element *)[localContext objectWithID:oid];
         
            // Insertion dans le contexte
            //[localContext insertObject:localElement];
         }
         
         for (Element *element in elements)
         {
             [localContext insertObject:element];
         }
     }];
}

#pragma Recherche dans le cache

#pragma Préparation des requêtes

- (NSFetchRequest*) prepareFetchRequest:(NSString*) elementType
{
    // Récupération d'éléments
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:elementType inManagedObjectContext:[ElementContextHelper defaultContext]];
    
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
    request.fetchLimit = ELEMENTS_PER_PAGE; // Taille de la page
    
    NSLog(@"DEBUG : Paging from %d to %d", request.fetchOffset, (request.fetchOffset + request.fetchLimit));
    
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
    NSFetchRequest *request = [self prepareFetchRequestPaginated:type fromPage:pageFrom toPage:pageTo];
    
    NSError *error;
    NSArray *array = [[ElementContextHelper defaultContext] executeFetchRequest:request error:&error];
    
    if (error != nil)
    {
        NSLog(@"CacheManager.getElements - %@", [error localizedDescription]);
        return NULL;
    }
    
    for (Element* element in array)
    {
        NSLog(@"Found: %@", element.title);
    }
    
    return array;
}

- (NSArray *)getAllElements:(NSString*)type
{
    NSFetchRequest *request = [self prepareFetchRequest:type];
    
    NSError *error;
    NSArray *array = [[ElementContextHelper defaultContext] executeFetchRequest:request error:&error];
    if (array == nil)
    {
        NSLog(@"ERROR: CacheManager.getAllElements - %@", [error localizedDescription]);
        return NULL;
    }
    
    return array;
}

- (BOOL)existsWithId:(NSNumber*)dbId
{
//    NSFetchRequest *request = [self prepareFetchRequestIdSearch:dbId];
  
    NSFetchRequest *request = [self prepareFetchRequest:@"Element"];
    
    NSError *error;
    NSArray *array = [[ElementContextHelper defaultContext] executeFetchRequest:request error:&error];
    if (array == nil)
    {
        NSLog(@"ERROR: CacheManager.existsWithId - %@", [error localizedDescription]);
        return false;
    }
    
    return array.count > 0;
}

@end
