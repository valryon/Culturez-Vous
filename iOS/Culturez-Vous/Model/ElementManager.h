//
//  ElementManager.h
//  Culturez-Vous
//
//  Created by Dam on 10/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Element.h"
#import "ElementDownloader.h"
#import "ElementCache.h"
#import "DateFormatter.h"

typedef void(^UpdateCompleted)(void);
typedef void(^ElementsRetrieved)(int page, NSArray* elements);

@interface ElementManager : NSObject

@property (nonatomic, retain) ElementDownloader *downloader;
@property (nonatomic, retain) ElementCache *cache;

//
// Mise à jour du cache si nécessaire
//
- (void) updateElementsWithCallback:(UpdateCompleted) callback;

//
// Récupération des éléments à partir du cache et téléchargement si nécessaire
//
- (void) getElementsWithPage: (int)page withCallback:(ElementsRetrieved) callback;

@end
