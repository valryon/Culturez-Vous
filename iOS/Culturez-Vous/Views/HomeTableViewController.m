//
//  HomeTableViewController.m
//  Culturez-Vous
//
//  Created by Dam on 20/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "HomeTableViewController.h"

@implementation HomeTableViewController

@synthesize cvElementsArray;
@synthesize managedObjectContext;
@synthesize elementDownloader;

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    cvElementsArray  = [NSMutableArray alloc];
    
    if(elementDownloader == NULL) {
        elementDownloader = [[ElementDownloader alloc] init];
        elementDownloader.downloadComplete = ^(NSArray* elements){
            // TODO Rafraîchir la vue ?
        };
    }
    
    // Chargement des données
    // TODO Vérifier ce qu'il y a en cache avant, tout ça
    [elementDownloader downloadElementsWithPage:1];
    
    cvElementsArray = [cvElementsArray initWithArray:[ElementCache getAllElements]];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [cvElementsArray count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    Element *element = (Element *)[cvElementsArray objectAtIndex:indexPath.row];
    
    NSString *cellIdentifier = nil;
    
    if([element isKindOfClass:[Word class]]) {
        cellIdentifier = @"Word";
    } else if([element isKindOfClass:[Contrepeterie class]]) {
        cellIdentifier = @"Contrepeterie";
    }
    
    ElementTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
    
    // Format pour la date
    static NSDateFormatter *dateFormatter = nil;
    if (dateFormatter == nil) {
        dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"dd MMM YYYY"];
    }
    
    cell.dateLabel.text = [dateFormatter stringFromDate:[element date]];
    cell.titleLabel.text = [element title];
    cell.unreadLabel.alpha = 0;
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    DetailViewController *controller = [self.storyboard instantiateViewControllerWithIdentifier:@"DetailsView"];
    [self.navigationController pushViewController:controller animated:YES];
}

- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    if ([[segue identifier] isEqualToString:@"ShowDetails"]) {
        DetailViewController *detailViewController = [segue destinationViewController];
        
        detailViewController.element = [self.cvElementsArray objectAtIndex:[self.tableView indexPathForSelectedRow].row];
    }
}

@end
