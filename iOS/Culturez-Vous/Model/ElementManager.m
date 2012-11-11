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

- (void) updateElementsWithCallback:(UpdateCompleted) callback {
    
    NSLog(@"INFO : Mise à jour demandée...");
    
    // On télécharge la première page (les nouveautés) du service
    [downloader downloadElementsWithPage:1
                            withCallback:^(NSArray* elements){
                                
                                BOOL saveCache = false;
                                
                                // Cache debug
                                //NSArray* f = [cache getElements:@"Element" withPage:1];
                                NSArray* f = [cache getAllElements:@"Element"];
                                
                                for (Element *element in f) {
                                    NSLog(@"INFO : element in cache %@", element.title);
                                }
                                
                                // On regarde s'il y a de nouveaux éléments
                                for (Element *element in elements) {
                                    
                                    // -- On regarde si des mots ne sont pas dans le cache
                                    if([cache existsWithId:element.dbId] == false)
                                    {
                                        NSLog(@"INFO : element to add %@", element.title);
                                        saveCache = true;
                                        
                                        // On l'ajoute au cache
                                        [cache insertElement:element];
                                    }
                                    else
                                    {
                                        //-- Si on le trouve, on considère que les mots suivants sont déjà connus (tri par date)
                                        break;
                                    }
                                }
                                
                                if(saveCache)
                                {
                                    // Sauvegarde du cache
                                    [cache saveCache];
                                }
                                
                                if(callback)
                                {
                                    NSLog(@"INFO : Mise à jour OK");
                                    callback();
                                }
                            }];
}

- (void)getElementsWithPage:(int)page withCallback:(ElementsRetrieved) callback
{
    // On récupère les éléments de cette page du cache
    //NSArray* f = [cache getElements:@"Element" withPage:page];
    NSArray* f = [cache getAllElements:@"Element"];
    
    if(callback)
    {
        callback(page,f);
    }
}


@end
