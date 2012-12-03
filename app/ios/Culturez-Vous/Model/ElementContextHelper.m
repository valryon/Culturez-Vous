//
//  ElementContextHelper.m
//  Culturez-Vous
//
//  Created by Dam on 02/12/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementContextHelper.h"


@implementation NSManagedObjectContext (MagicalObserving)

#pragma mark - Context Observation Helpers

- (void) observeContext:(NSManagedObjectContext *)otherContext
{
    NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
	[notificationCenter addObserver:self
                           selector:@selector(mergeChangesFromNotification:)
                               name:NSManagedObjectContextDidSaveNotification
                             object:otherContext];
}

- (void) mergeChangesFromNotification:(NSNotification *)notification;
{
	NSLog(@"DEBUG: Merging changes to %@context%@",
          self == [ElementContextHelper defaultContext] ? @"*** DEFAULT *** " : @"",
          ([NSThread isMainThread] ? @" *** on Main Thread ***" : @""));
    
	[self mergeChangesFromContextDidSaveNotification:notification];
}

@end

@implementation ElementContextHelper

// Contexte principal
static NSManagedObjectContext *defaultManagedObjectContext = nil;
// Queue pour la sauvegarde asynchrone
static dispatch_queue_t coredata_background_save_queue;

static NSPersistentStoreCoordinator *store_coordinator;
static NSManagedObjectModel *object_model;


#pragma mark - Création des objets Core Data

//
// Récupération du contexte principal.
// Il sera créé s'il n'existe pas.
//
+ (NSManagedObjectContext *)defaultContext
{
    if (defaultManagedObjectContext == nil) {
        defaultManagedObjectContext = [[NSManagedObjectContext alloc] initWithConcurrencyType:NSMainQueueConcurrencyType];
        [defaultManagedObjectContext setPersistentStoreCoordinator:[self persistentStoreCoordinator]];
        [defaultManagedObjectContext setMergePolicy:NSMergeByPropertyStoreTrumpMergePolicy];
    }
    
    return defaultManagedObjectContext;
}

//
// Récupération d'un nouveau contexte qui aura pour parent le contexte principal
//
+ (NSManagedObjectContext *) context;
{
    NSManagedObjectContext *context = [[NSManagedObjectContext alloc] initWithConcurrencyType:NSPrivateQueueConcurrencyType];
    [context setParentContext:[self defaultContext]];
    
    // Contexte principal observateur du nouveau contexte
    [[self defaultContext] observeContext:context];
    
    [context setMergePolicy:NSMergeByPropertyObjectTrumpMergePolicy];
    
    return context;
}

//
// Récupération d'un StoreCoordinator
//
+ (NSPersistentStoreCoordinator *)persistentStoreCoordinator
{
    if(store_coordinator == nil)
    {
        
        AppDelegate* app = (AppDelegate*)[[UIApplication sharedApplication] delegate];
        NSURL *storeURL = [[app applicationDocumentsDirectory] URLByAppendingPathComponent:@"Culturez_Vous.sqlite"];
        
        NSError *error = nil;
        NSPersistentStoreCoordinator *persistentStoreCoordinator = [[NSPersistentStoreCoordinator alloc] initWithManagedObjectModel:[self managedObjectModel]];
        
        if (![persistentStoreCoordinator addPersistentStoreWithType:NSSQLiteStoreType configuration:nil URL:storeURL options:nil error:&error]) {
            /*
             Replace this implementation with code to handle the error appropriately.
             
             abort() causes the application to generate a crash log and terminate. You should not use this function in a shipping application, although it may be useful during development.
             
             Typical reasons for an error here include:
             * The persistent store is not accessible;
             * The schema for the persistent store is incompatible with current managed object model.
             Check the error message to determine what the actual problem was.
             
             
             If the persistent store is not accessible, there is typically something wrong with the file path. Often, a file URL is pointing into the application's resources directory instead of a writeable directory.
             
             If you encounter schema incompatibility errors during development, you can reduce their frequency by:
             * Simply deleting the existing store:
             [[NSFileManager defaultManager] removeItemAtURL:storeURL error:nil]
             
             * Performing automatic lightweight migration by passing the following dictionary as the options parameter:
             @{NSMigratePersistentStoresAutomaticallyOption:@YES, NSInferMappingModelAutomaticallyOption:@YES}
             
             Lightweight migration will only work for a limited set of schema changes; consult "Core Data Model Versioning and Data Migration Programming Guide" for details.
             
             */
            NSLog(@"Unresolved error %@, %@", error, [error userInfo]);
            abort();
        }
        
        store_coordinator = persistentStoreCoordinator;
    }
    
    return store_coordinator;
}

//
// Création d'un nouveau modèle
//
+ (NSManagedObjectModel *)managedObjectModel
{
    if(object_model == nil)
    {
        NSURL *modelURL = [[NSBundle mainBundle] URLForResource:@"Culturez_Vous" withExtension:@"momd"];
        NSManagedObjectModel *managedObjectModel = [[NSManagedObjectModel alloc] initWithContentsOfURL:modelURL];
        
        object_model = managedObjectModel;
    }
    
    return object_model;
}

#pragma mark - Opérations sur le contexte

+ (void)saveDataInContext:(void(^)(NSManagedObjectContext *localContext))saveBlock
{
    // Création d'un nouveau contexte
	NSManagedObjectContext *localContext = [self context];
    
    // Exécution de la requête personnalisée
	saveBlock(localContext);
    
    // Sauvegarde des modifications
	if ([localContext hasChanges])
	{
        NSError *error = nil;
        
        NSLog(@"INFO : saving context...");
		[localContext save:&error];
        NSLog(@"INFO : save complete!");

        if(error != nil)
        {
            NSLog(@"ERROR: ElementContextHelper.saveDataInContext %@",error.localizedDescription);
        }
	}
}

// http://www.duckrowing.com/2010/03/11/using-core-data-on-multiple-threads/
- (void)mergeChanges:(NSNotification *)notification
{
    NSManagedObjectContext *mainContext = [ElementContextHelper defaultContext];
    
    // Merge changes into the main context on the main thread
    [mainContext performSelectorOnMainThread:@selector(mergeChangesFromContextDidSaveNotification:)
                                  withObject:notification
                               waitUntilDone:YES];
}

//
// Sauvegarde asynchrone
//
+ (void)saveDataInBackgroundWithContext:(void(^)(NSManagedObjectContext *context))saveBlock completion:(void(^)(void))completion
{
	dispatch_async(background_save_queue(), ^{
		[self saveDataInContext:saveBlock];
        
		dispatch_sync(dispatch_get_main_queue(), ^{
			completion();
		});
	});
}

//
// Fonction pour créer une file pour le dispatcher pour les sauvegardes
//
dispatch_queue_t background_save_queue()
{
    if (coredata_background_save_queue == NULL)
    {
        coredata_background_save_queue = dispatch_queue_create("fr.culturezvous.coredata.backgroundsaves", 0);
    }
    return coredata_background_save_queue;
}

@end
