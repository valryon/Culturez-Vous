//
//  ElementContextHelper.h
//  Culturez-Vous
//
//  Created by Dam on 02/12/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>

#if ENABLE_PONYDEBUGGER
#import <PonyDebugger/PonyDebugger.h>
#endif

@interface ElementContextHelper : NSObject

+(void) initialize:(NSURL *)documentUrl;
+(void) enableDebug;

+ (NSManagedObjectContext *)defaultContext;
+ (NSManagedObjectContext *)context;
+ (NSPersistentStoreCoordinator *)persistentStoreCoordinator;
+ (NSManagedObjectModel *)managedObjectModel;

+ (void)saveDataInContext:(void(^)(NSManagedObjectContext *localContext))saveBlock;
+ (void)saveDataInBackgroundWithContext:(void(^)(NSManagedObjectContext *context))saveBlock completion:(void(^)(void))completion;

@end
