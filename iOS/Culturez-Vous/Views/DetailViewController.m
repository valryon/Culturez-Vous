//
//  DetailViewController.m
//  Culturez-Vous
//
//  Created by Dam on 30/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "DetailViewController.h"

@interface DetailViewController ()

@end

@implementation DetailViewController

// Saisie de l'élement à afficher
- (void)setElement:(Element *)newElement {
    
    if(newElement != _element){
        _element = newElement;
    }
    
    [self configureView];
}

// Préparation de la vue pour un élément
- (void)configureView
{
    
}

@end
