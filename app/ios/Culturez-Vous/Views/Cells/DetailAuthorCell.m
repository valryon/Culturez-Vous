//
//  DetailAuthorCell.m
//  Culturez-Vous
//
//  Created by Dam on 11/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "DetailAuthorCell.h"

@implementation DetailAuthorCell

@synthesize authorInfo;

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:style reuseIdentifier:reuseIdentifier];
    if (self) {
        // Initialization code
    }
    return self;
}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated
{
    [super setSelected:selected animated:animated];

    // Configure the view for the selected state
}

- (IBAction)showAuthor:(id)sender
{
    // Affichage des informations de l'auteur
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString: authorInfo]];
}
@end
