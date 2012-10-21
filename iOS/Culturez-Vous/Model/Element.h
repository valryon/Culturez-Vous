//
//  Element.h
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Element : NSManagedObject

@property (nonatomic, retain) NSString * title;
@property (nonatomic, retain) NSDate * date;
@property (nonatomic, retain) NSNumber * voteCount;
@property (nonatomic, retain) NSNumber * isFavorite;

@end
