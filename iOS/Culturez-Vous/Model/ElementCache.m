//
//  ElementCache.m
//  Culturez-Vous
//
//  Created by Dam on 02/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementCache.h"

@implementation ElementCache

#pragma Instanciation des objets en cache

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

+(NSArray *)getAllElements
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    NSEntityDescription *entityDescription = [NSEntityDescription
                                              entityForName:@"Word" inManagedObjectContext:app.managedObjectContext];
    NSFetchRequest *request = [[NSFetchRequest alloc] init];
    [request setEntity:entityDescription];
    
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
