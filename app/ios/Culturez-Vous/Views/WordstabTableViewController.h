//
//  HomeTableViewController.h
//  Culturez-Vous
//
//  Created by Dam on 20/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ElementManager.h"
#import "ElementTableViewCell.h"
#import "DetailTableViewController.h"
#import "DateFormatter.h"
#import "Element.h"
#import "Word.h"
#import "Contrepeterie.h"
#import "Definition.h"

@interface WordstabTableViewController : UITableViewController<UITableViewDelegate, UITableViewDataSource>
{
    NSMutableArray *cvElementsArray;
    NSManagedObjectContext *managedObjectContext;
    
    UIBarButtonItem *addButton;
}

// Elements affichés
@property (nonatomic, retain) NSMutableArray *cvElementsArray;
// Cache
@property (nonatomic, retain) NSManagedObjectContext *managedObjectContext;
// Table view
@property(nonatomic, strong) IBOutlet UITableView *tableView;

@property (nonatomic, retain) ElementManager *elementManager;

@end
