//
//  ElementManager.m
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementManager.h"

@implementation ElementManager

+ (id)createNewWord:(NSString *)title withDate:(NSDate *)date
{
    AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
    
    // Création d'un nouveau mot à partir du cache
    Word *word = (Word *)[NSEntityDescription insertNewObjectForEntityForName:@"Word" inManagedObjectContext:app.managedObjectContext];
    
    if(word != NULL) {
        // Mise à jour des données
        [word setTitle:title];
        [word setDate:date];
    
        // Sauvegarde dans le cache
        NSError *error = nil;
        if (![app.managedObjectContext save:&error]) {
            // Handle the error.
            NSLog(@"%@", [error localizedDescription]);
        }
    }
    return word;
}

@end
