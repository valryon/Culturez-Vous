//
//  Definition.h
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Definition : NSManagedObject

@property (nonatomic, retain) NSString * content;
@property (nonatomic, retain) NSString * details;

@end
