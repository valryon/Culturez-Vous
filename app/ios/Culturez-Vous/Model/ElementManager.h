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

typedef void(^UpdatePageCompleted)(BOOL newElementsFound);
typedef void(^UpdateCompleted)(void);
typedef void(^ElementsRetrieved)(NSArray* elements);

@interface ElementManager : NSObject

@property (nonatomic, retain) ElementDownloader *downloader;
@property (nonatomic, retain) ElementCache *cache;

//
// Mise à jour du cache si nécessaire
//
- (void) updateElementsWithCallback:(UpdateCompleted) callback withFailureCallback:(FailureCallback)failureCallback;

//
// Récupération des éléments à partir du cache et téléchargement si nécessaire
//
- (void) getElements: (NSString*)elementType fromPage: (int)pageFrom toPage:(int)pageTo withCallback:(ElementsRetrieved) callback withFailureCallback:(FailureCallback)failureCallback;

//
// Récupérer tous les éléments
//
- (void) getAllElements: (NSString*)elementType withCallback:(ElementsRetrieved) callback withFailureCallback:(FailureCallback)failureCallback;

//
// Marquer un élément comme lu
//
- (void) markElementAsRead: (Element*)element;

//
// Marquer un élément comme favoris
//
- (void) markElementAsFavorite: (Element*)element;

@end
