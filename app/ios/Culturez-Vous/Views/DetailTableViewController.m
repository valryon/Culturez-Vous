//
//  DetailViewController.m
//  Culturez-Vous
//
//  Created by Dam on 30/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "DetailTableViewController.h"

@implementation DetailTableViewController

@synthesize element;

- (void)viewDidLoad
{
    [self configureView];
}

// Préparation de la vue pour un élément
- (void)configureView
{
    //self.dateLabel.text = [DateFormatter getStringForDate:element.date withFormat:@"dd MMMM yyyy"];
    
    //self.authorLabel.text = element.author;
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // 1 section pour les infos
    int sectionCount = 1;
    
    // + 1 section par définition
    if([element isKindOfClass:[Word class]])
    {
        Word *word = (Word*)element;
        sectionCount += word.definitions.count;
        
        sectionCount += 1; // DEBUG
    }
    // + 1 section pour les contrepétries
    else if([element isKindOfClass:[Contrepeterie class]])
    {
        sectionCount += 1;
    }
    
    
    
    return sectionCount;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if(section == 0)
    {
        return 2;
    }
    
    return 2;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSString* cellIdentifier = NULL;
    UITableViewCell *cell = NULL;
    
    if(indexPath.section == 0 && indexPath.row == 0)
    {
        cellIdentifier = @"dateCell";
        
        DetailDateCell* dc = (DetailDateCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
        
        dc.dateLabel.text = [DateFormatter getStringForDate:element.date withFormat:@"dd MMMM yyyy"];
        
        cell = dc;
    }
    else if(indexPath.section == 0 && indexPath.row == 1)
    {
        cellIdentifier = @"auteurCell";
        
        DetailAuthorCell* dc = (DetailAuthorCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
        
        dc.authorCell.text = element.author;
        dc.authorInfo = element.authorInfo;
        
        cell = dc;
    }
    else if(indexPath.section == 1)
    {
        if([element isKindOfClass:[Word class]])
        {
            if(indexPath.row % 2 == 0)
            {
                cellIdentifier = @"wordDetailsCell";
                
                DetailWordDetailsCell* dc = (DetailWordDetailsCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
                
                
                cell = dc;
            }
            else
            {
                cellIdentifier = @"wordContentCell";
                
                DetailWordContentCell* dc = (DetailWordContentCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
                
                
                cell = dc;
            }
        }
        else if([element isKindOfClass:[Contrepeterie class]])
        {
            if(indexPath.row % 2 == 0)
            {
                cellIdentifier = @"ctpContentCell";
                
                DetailCtpContentCell* dc = (DetailCtpContentCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
                
                
                cell = dc;
            }
            else
            {
                cellIdentifier = @"ctpSolutionCell";
                
                DetailCtpSolutionCell* dc = (DetailCtpSolutionCell*)[tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
                
                
                cell = dc;
            }
        }
    }
    
    if(cell == NULL)
    {
       cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
    }
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
}

@end
