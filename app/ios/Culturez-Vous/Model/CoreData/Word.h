//
//  Word.h
//  Culturez-Vous
//
//  Created by Dam on 01/12/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "Element.h"

@class Definition;

@interface Word : Element

@property (nonatomic, retain) NSSet *definitions;
@end

@interface Word (CoreDataGeneratedAccessors)

- (void)addDefinitionsObject:(Definition *)value;
- (void)removeDefinitionsObject:(Definition *)value;
- (void)addDefinitions:(NSSet *)values;
- (void)removeDefinitions:(NSSet *)values;

@end
