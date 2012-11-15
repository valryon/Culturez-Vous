//
//  Word.h
//  Culturez-Vous
//
//  Created by Dam on 15/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "Element.h"

@class Definition;

@interface Word : Element

@property (nonatomic, retain) NSSet *definitions;
@property (nonatomic, retain) NSSet *tempDefinitions;
@end

@interface Word (CoreDataGeneratedAccessors)

- (void)addDefinitionsObject:(Definition *)value;
- (void)removeDefinitionsObject:(Definition *)value;
- (void)addDefinitions:(NSSet *)values;
- (void)removeDefinitions:(NSSet *)values;

- (void)addTempDefinitionsObject:(Definition *)value;
- (void)removeTempDefinitionsObject:(Definition *)value;
- (void)addTempDefinitions:(NSSet *)values;
- (void)removeTempDefinitions:(NSSet *)values;

@end
