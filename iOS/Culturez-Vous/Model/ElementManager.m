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
                                
                                // On regarde s'il y a de nouveaux éléments
                                // -- Pour cela on récupère tout d'abord la première page du cache
                                NSArray* firstCachePage = [cache getElements:@"Element" withPage:1];
                                
                                for (Element *element in elements) {
                                    
                                }
                                
                                
                                if(callback) {
                                    NSLog(@"INFO : Mise à jour OK");
                                    callback();
                                }
                            }];
}

- (void)getElementsWithPage:(int)page withCallback:(ElementsRetrieved) callback
{
    // On récupère les éléments de cette page du cache
    
}

- (NSArray *)getElementsFromCacheWithPage:(int)page
{
    // On récupère les éléments de cette page du cache
    return NULL;
}


@end
