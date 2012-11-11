//
//  DetailAuthorCell.h
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DetailAuthorCell : UITableViewCell

@property (weak, nonatomic) IBOutlet UILabel *authorCell;

@property (weak, nonatomic) NSString *authorInfo;

- (IBAction)showAuthor:(id)sender;

@end
