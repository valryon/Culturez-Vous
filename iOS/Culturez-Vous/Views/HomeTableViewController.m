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
@synthesize elementManager;

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    cvElementsArray = [[NSMutableArray alloc] init];
    
    elementManager = [[ElementManager alloc ]init];
    
    // On essaie de récupèrer les nouveaux éléments
    [elementManager updateElementsWithCallback:^{
        
        // Puis on charge la première page
        [elementManager getElementsWithPage:1 withCallback:^(int page, NSArray *elements) {
            // Ajouter les éléments
            [cvElementsArray setArray:elements];
            
            // Rafraîchir la vue
            [self.tableView reloadData];
        }];
        //TODO code
    }];
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
        //        cellIdentifier = @"Contrepeterie";
        cellIdentifier = @"Word"; //TODO template
    }
    
    ElementTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
    
    cell.dateLabel.text = [DateFormatter getStringForDate:element.date withFormat:@"dd MMM YYYY"];
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
