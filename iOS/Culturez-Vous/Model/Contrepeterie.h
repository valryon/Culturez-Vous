//
//  Contrepeterie.h
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>
#import "Element.h"


@interface Contrepeterie : Element

@property (nonatomic, retain) NSString * content;
@property (nonatomic, retain) NSString * solution;

@end
