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



#pragma TODO Supprimer : Instanciation des objets en cache

- (id)createNewDefinition:(NSString *)details withContent:(NSString *)content
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Création d'une nouvelle définition
    Definition *def = (Definition *)[NSEntityDescription insertNewObjectForEntityForName:@"Definition" inManagedObjectContext:app.managedObjectContext];
    
    if(def != NULL) {
        // Mise à jour des données
        [def setDetails:details];
        [def setContent:content];
    }
    
    return def;
}

- (id)createNewWord:(NSString *)title withDate:(NSDate *)date
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Création d'un nouveau mot à partir du cache
    Word *word = (Word *)[NSEntityDescription insertNewObjectForEntityForName:@"Word" inManagedObjectContext:app.managedObjectContext];
    
    if(word != NULL) {
        // Mise à jour des données
        [word setTitle:title];
        [word setDate:date];
    }
    return word;
}

- (id)createNewContrepeterie:(NSString *)title withDate:(NSDate *)date withContent:(NSString*) content withSolution:(NSString*) solution
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Création d'un nouveau mot à partir du cache
    Contrepeterie *ctp = (Contrepeterie *)[NSEntityDescription insertNewObjectForEntityForName:@"Contrepeterie" inManagedObjectContext:app.managedObjectContext];
    
    if(ctp != NULL) {
        // Mise à jour des données
        [ctp setTitle:title];
        [ctp setDate:date];
    }
    return ctp;
}

#pragma Sauvegarde du cache

-(BOOL)saveCache
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Sauvegarde dans le cache
    NSError *error = nil;
    if (![app.managedObjectContext save:&error]) {
        NSLog(@"%@", [error localizedDescription]);
        return false;
    }
    
    return true;
}

#pragma Recherche dans le cache

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

- (NSFetchRequest*) prepareFetchRequestPaginated:(NSString*) elementType forPage:(int)page
{
    if(page < 0) page = 0;
    
    // Requête standard
    NSFetchRequest *request = [self prepareFetchRequest:elementType];
    
    // Pagination
    request.fetchOffset = page * ELEMENTS_PER_PAGE; // Index de début
    request.fetchLimit = ELEMENTS_PER_PAGE; // Taille de la page
    
    return request;
}

- (NSArray*) getElements:(NSString*)type withPage:(int) page
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSFetchRequest *request = [self prepareFetchRequestPaginated:type forPage:page];
    
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

@end
