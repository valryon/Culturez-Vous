//
//  DetailViewController.h
//  Culturez-Vous
//
//  Created by Dam on 30/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ElementManager.h"
#import "DateFormatter.h"
#import "DetailAuthorCell.h"
#import "DetailDateCell.h"
#import "DetailWordContentCell.h"
#import "DetailWordDetailsCell.h"
#import "DetailCtpContentCell.h"
#import "DetailCtpSolutionCell.h"

@interface DetailTableViewController : UITableViewController<UITableViewDelegate,UITableViewDataSource>


@property (strong, nonatomic) Element *element;

@end
