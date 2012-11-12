//
//  Element.h
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface Element : NSManagedObject

@property (nonatomic, retain) NSDate * date;
@property (nonatomic, retain) NSNumber * dbId;
@property (nonatomic, retain) NSNumber * isFavorite;
@property (nonatomic, retain) NSString * title;
@property (nonatomic, retain) NSNumber * voteCount;
@property (nonatomic, retain) NSString * author;
@property (nonatomic, retain) NSString * authorInfo;

@end
