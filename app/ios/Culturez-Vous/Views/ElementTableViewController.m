//
//  ElementTableViewController.m
//  Culturez-Vous
//
//  Created by Dam on 18/11/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementTableViewController.h"

@implementation ElementTableViewController

@synthesize cvElementsArray;
@synthesize managedObjectContext;
@synthesize elementManager;

-(NSString*) getElementType
{
    return @"Element";
}

-(bool) getFavorites
{
    return false;
}

- (void)loadPage:(int)pageFrom toPage:(int)pageTo
{
    NSLog(@"DEBUG : Affichage page %d à %d",pageFrom, pageTo);
    
    // Puis on charge les deux première page
    [elementManager getElements:
     [self getElementType]
                       fromPage:pageFrom toPage:pageTo
                   withCallback:^(NSArray *elements) {
                       
                       // Ajouter les éléments
                       [cvElementsArray addObjectsFromArray:elements];
                       
                       if(self.lastPage == nil || [self.lastPage intValue] < pageTo)
                       {
                           self.lastPage = [[NSNumber alloc] initWithInt:pageTo];
                       }
                       
                       // Rafraîchir la vue
                       [self.tableView reloadData];
                       
                       //                            [self.tableView performSelectorOnMainThread:@selector(reloadData) withObject:nil waitUntilDone:NO];
                   }
            withFailureCallback:^(NSError *error)
     {
         UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Lecture du cache impossible"
                                                         message:[NSString stringWithFormat:@"Et c'est assez mauvais... %@.", error]
                                                        delegate:nil
                                               cancelButtonTitle:@"OK..."
                                               otherButtonTitles:nil];
         [alert show];
     }
     ];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    [self.tableView registerNib:[UINib nibWithNibName:@"ElementCell" bundle:[NSBundle mainBundle]] forCellReuseIdentifier:@"Element"];
    
    elementManager = [[ElementManager alloc ]init];
    
    cvElementsArray = [[NSMutableArray alloc] init];
    
    /*
    [elementManager getAllElements:@"Word" withCallback:^(NSArray *elements) {
        NSLog(@"%@",elements);
        [cvElementsArray setArray:elements];
    } withFailureCallback:^(NSError *error) {
        NSLog(@"Poueté");
    }];
    */
    
    // Puis on essaie de récupèrer les nouveaux éléments en tâche de fond
    [elementManager updateElementsWithCallback:^
     {
         [self loadPage:0 toPage:2];
     }
                           withFailureCallback:^(NSError *error) {
                               
                               UIAlertView *alert = [[UIAlertView alloc]
                                                     initWithTitle:@"Mise à jour impossible"
                                                     message:[NSString stringWithFormat:@"Vérifiez votre connexion à Internet... %@.", error]
                                                     delegate:nil
                                                     cancelButtonTitle:@"OK..."
                                                     otherButtonTitles:nil];
                               [alert show];
                           }
     ];
    /*
    
    __block ElementTableViewController *controller = self;
    
    // setup pull-to-refresh
    [self.tableView addPullToRefreshWithActionHandler:^{
        NSLog(@"Pull to refresh");
        
        [controller.elementManager updateElementsWithCallback:^
         {
#warning Ajouter les nouveaux éléments ?
             [controller.tableView.pullToRefreshView stopAnimating];
         }
        withFailureCallback:^(NSError *error) {
                [controller.tableView.pullToRefreshView stopAnimating];
         }
         ];
        
        [controller.tableView.pullToRefreshView stopAnimating];
    }];
    
    // setup infinite scrolling
    [self.tableView addInfiniteScrollingWithActionHandler:^{
        NSLog(@"Infinite scroll");
        
        [controller loadPage:[self.lastPage intValue] toPage:([self.lastPage intValue] + 1)];
        
        [controller.tableView.infiniteScrollingView stopAnimating];
    }];
    
    // trigger the refresh manually at the end of viewDidLoad
    [self.tableView triggerPullToRefresh];
     
     */
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
    
    //    if([element isKindOfClass:[Word class]]) {
    //        cellIdentifier = @"Word";
    //    } else if([element isKindOfClass:[Contrepeterie class]]) {
    //        cellIdentifier = @"Contrepeterie";
    //    }
    
    cellIdentifier = @"Element";
    
    ElementTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:cellIdentifier forIndexPath:indexPath];
    
    cell.dateLabel.text = [DateFormatter getStringForDate:element.date withFormat:@"dd MMM YYYY"];
    cell.titleLabel.text = [element title];
    cell.unreadLabel.alpha = 0;
    
    
    return cell;
}

#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    DetailTableViewController *controller = [self.storyboard instantiateViewControllerWithIdentifier:@"DetailsView"];
    
    // On transmet l'élément à afficher au détail
    Element *element = [self.cvElementsArray objectAtIndex:[self.tableView indexPathForSelectedRow].row];
    
    controller.element = element;
    
    // On le marque comme lu
    [elementManager markElementAsRead:element];
    
    // Go !
    [self.navigationController pushViewController:controller animated:YES];
}

@end
