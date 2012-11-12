//
//  ElementTableViewCell.h
//  Culturez-Vous
//
//  Created by Dam on 30/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ElementTableViewCell : UITableViewCell

@property (nonatomic, weak) IBOutlet UILabel *titleLabel;
@property (nonatomic, weak) IBOutlet UILabel *dateLabel;
@property (nonatomic, weak) IBOutlet UILabel *unreadLabel;

@end
