//
//  HomeTableViewController.h
//  Culturez-Vous
//
//  Created by Dam on 20/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface HomeTableViewController : UITableViewController {
    
    NSMutableArray *cvElementsArray;
    NSManagedObjectContext *managedObjectContext;
    
    UIBarButtonItem *addButton;
}

// Elements affichés
@property (nonatomic, retain) NSMutableArray *cvElementsArray;
// Cache
@property (nonatomic, retain) NSManagedObjectContext *managedObjectContext;

@end
