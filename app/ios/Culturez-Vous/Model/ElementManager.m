//
//  ElementManager.m
//  Culturez-Vous
//
//  Created by Dam on 10/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementManager.h"

@implementation ElementManager

@synthesize cache;
@synthesize downloader;

- (id) init
{
    self = [super init];
    
    // On instancie le composant de téléchargement
    downloader = [[ElementDownloader alloc] init];
    
    // Et celui de stockage
    cache = [[ElementCache alloc] init];
    
    return self;
}

// Fonction récursive de mise à jour
- (void) updatePage:(int)page forType:(NSString*)type withCallback:(UpdatePageCompleted) callback withFailureCallback:(FailureCallback)failureCallback
{
    [downloader downloadElementsWithPage:page
                                 forType:type
                            withCallback:^(NSManagedObjectContext *context){
                                
                                BOOL newElementsFound = false;
                                
                                // On regarde s'il y a de nouveaux éléments pour cette page
                                for (Element *element in [context insertedObjects])
                                {
                                    if( [element isKindOfClass:[Element class]] == false) continue;
                                    
                                    // -- On regarde si des mots sont dans le cache
                                    if([cache existsWithId:element.dbId])
                                    {
                                        // On le supprime du contexte temporaire
                                        [context deleteObject:element];
                                    }
                                    else
                                    {
                                        // Si on ne le trouve pas on considère qu'il doit être ajouté
                                        newElementsFound = true;
                                    }
                                }
                                
                                if(newElementsFound)
                                {
                                    // Ajout en sauvant dans le cache
                                    NSError *error;
                                    [context save:&error];
                                    
                                    if(error)
                                    {
                                        NSLog(@"ERROR: %@",[error localizedDescription]);
                                    }
                                }
                                
                                if(callback)
                                {
                                    NSLog(@"INFO : Mise à jour page %d OK",page);
                                    callback(newElementsFound);
                                }
                            }
                       withErrorCallback:^(NSError *error) {
                           if(failureCallback)
                           {
                               failureCallback(error);
                           }
                       }
     ];
}

- (void) updateElements: (NSString*)elementType withCallback:(UpdateCompleted) callback withFailureCallback:(FailureCallback)failureCallback
{
    // On télécharge la première page du service, à la recherche de nouveauté
    NSLog(@"INFO : Update...");
    
    //En asynchrone
    dispatch_queue_t queuePage = dispatch_queue_create("com.culturezvous.updatpage", NULL);
    
    dispatch_async(queuePage, ^{
        [self updatePage:0 forType:elementType withCallback:^(BOOL newElementsFound)
         {
             NSLog(@"INFO : Update OK !");
             dispatch_async(dispatch_get_main_queue(), ^{
                 callback();
             });
         } withFailureCallback:^(NSError *error)
         {
             dispatch_async(dispatch_get_main_queue(), ^{
                 failureCallback(error);
             });
         }];
    });
}

- (void) getElements: (NSString*)elementType forPage: (int)page withCallback:(ElementsRetrieved) callback withFailureCallback:(FailureCallback)failureCallback
{
    // On regarde combien d'éléments on a en cache pour ces pages
    int elementCacheCount = [cache getElementsCount:elementType forPage:page];
    
    // S'il n'y a pas assez d'éléments on essaie d'en télécharger pour compléter
    if(elementCacheCount < ELEMENTS_PER_PAGE)
    {
        [self updatePage:page forType:elementType withCallback:^(BOOL newElementsFound) {
            NSArray* elements = [cache getElements:elementType forPage:page];
    
            if(callback)
            {
                callback(elements);
            }
        } withFailureCallback:^(NSError *error) {
            if(failureCallback)
            {
                failureCallback(error);
            }
        }];
    }
    else{
    
        // Enfin on récupère les éléments de cette page du cache
        NSArray* elements = [cache getElements:elementType forPage:page];
    
        if(callback)
        {
            callback(elements);
        }
    }
}

- (void) getAllElements: (NSString*)elementType withCallback:(ElementsRetrieved) callback withFailureCallback:(FailureCallback)failureCallback
{
    NSArray* elements = [cache getAllElements:elementType];
    
    if(callback)
    {
        callback(elements);
    }
}

- (void) markElementAsRead: (Element*)element
{
    element.isRead = [NSNumber numberWithBool:true];
    
    //    [cache saveCache];
}

- (void) markElementAsFavorite: (Element*)element
{
    element.isFavorite = [NSNumber numberWithBool:true];
    
    //    [cache saveCache];
}

@end
