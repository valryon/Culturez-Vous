//
//  DetailViewController.h
//  Culturez-Vous
//
//  Created by Dam on 30/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "Element.h"

@interface DetailViewController : UITableViewController

@property (strong, nonatomic) Element *element;
@property (weak, nonatomic) IBOutlet UILabel *elementTitleLabel;
@property (weak, nonatomic) IBOutlet UILabel *elementDateLabel;

@end
