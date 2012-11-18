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
- (void) updatePage:(int)page withCallback:(UpdatePageCompleted) callback withFailureCallback:(FailureCallback)failureCallback
{
    [downloader downloadElementsWithPage:page
                            withCallback:^(NSArray* elements){
                                
                                BOOL newElementsFound = false;
                                
                                // On regarde s'il y a de nouveaux éléments pour cette page
                                for (Element *element in elements) {
                                    
                                    // -- On regarde si des mots ne sont pas dans le cache
                                    if([cache existsWithId:element.dbId] == false)
                                    {
                                        NSLog(@"INFO : element to add %@", element.title);
                                        newElementsFound = true;
                                        
                                        // On l'ajoute au cache
                                        [cache insertElement:element];
                                    }
                                    else
                                    {
                                        //-- Si on le trouve, on considère que les mots suivants sont déjà connus (tri par date)
                                        break;
                                    }
                                }
                                
                                if(newElementsFound)
                                {
                                    // Sauvegarde du cache
                                    [cache saveCache];
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

- (void) updateElementsWithCallback:(UpdateCompleted) callback withFailureCallback:(FailureCallback)failureCallback
{
    // On télécharge les premières page du service, à la recherche de nouveautés
    NSLog(@"INFO : Mise à jour demandée...");
    
    // ATTENTION LES YEUX
    // On charge autant de pages que nécessaire pour avoir un minimum d'éléments à afficher
    dispatch_queue_t queue = dispatch_queue_create("com.culturezvous.updatequeue", NULL);
    
    // On crée un thread qui va attendre que la mise à jour soit terminée
    dispatch_async(queue, ^{
        
        __block NSError *errorDownload = NULL;
        __block BOOL loadingComplete = false;
        int page = 1;
        
        // On retourne en mode synchrone donc
        while(loadingComplete == false && page < UPDATE_PAGES_COUNT)
        {
            // Le sémaphore permet de débloquer le thread synchrone quand la requête async est terminée
            dispatch_semaphore_t sema = dispatch_semaphore_create(0);
            
            // Cette requête async c'est notre récupération d'éléments pour une page
            dispatch_queue_t queuePage = dispatch_queue_create("com.culturezvous.updatpage", NULL);
            
            dispatch_async(queuePage, ^{
                
                [self updatePage:page
                    withCallback:^(BOOL newElementFound)
                 {
                     loadingComplete = (newElementFound == false);
                     
                     // Débloque le sémaphore
                     dispatch_semaphore_signal(sema);
                 }
             withFailureCallback:^(NSError *error) {
                 
                 errorDownload = error;
                 
                 // Débloque le sémaphore
                 dispatch_semaphore_signal(sema);
                 
             }
                 ];
            });
            
            // Attente d'un signal
            dispatch_semaphore_wait(sema, DISPATCH_TIME_FOREVER);
            sema = NULL;
            
            page++;
        }
        
        if(errorDownload != NULL)
        {
            NSLog(@"INFO : Erreur lors de la mise à jour de la page %d.", page);
            
            if(failureCallback)
            {
                dispatch_async(dispatch_get_main_queue(), ^{
                    failureCallback(errorDownload);
                });
            }
        }
        else if(callback)
        {
            NSLog(@"INFO : Mise à jour OK");
            
            dispatch_async(dispatch_get_main_queue(), ^{
                callback();
            });
        }
    });
    
}

- (void) getElements: (NSString*)elementType fromPage: (int)pageFrom toPage:(int)pageTo withCallback:(ElementsRetrieved) callback withFailureCallback:(FailureCallback)failureCallback
{
    // On récupère les éléments de cette page du cache
    NSArray* elements = [cache getElements:elementType fromPage:pageFrom toPage:pageTo];
    
    if(callback)
    {
        callback(elements);
    }
}

- (void) markElementAsRead: (Element*)element
{
    element.isRead = [NSNumber numberWithBool:true];
    
    [cache saveCache];
}

- (void) markElementAsFavorite: (Element*)element
{
    element.isFavorite = [NSNumber numberWithBool:true];
    
    [cache saveCache];
}

@end
