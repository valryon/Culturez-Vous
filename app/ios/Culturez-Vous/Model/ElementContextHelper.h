//
//  ElementContextHelper.h
//  Culturez-Vous
//
//  Created by Dam on 02/12/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AppDelegate.h"

@interface ElementContextHelper : NSObject

+ (NSManagedObjectContext *)defaultContext;
+ (NSManagedObjectContext *)context;
+ (NSPersistentStoreCoordinator *)persistentStoreCoordinator;
+ (NSManagedObjectModel *)managedObjectModel;

+ (void)saveDataInContext:(void(^)(NSManagedObjectContext *localContext))saveBlock;
+ (void)saveDataInBackgroundWithContext:(void(^)(NSManagedObjectContext *context))saveBlock completion:(void(^)(void))completion;

@end
