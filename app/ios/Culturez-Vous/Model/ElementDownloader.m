//
//  ElementManager.m
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementDownloader.h"

@implementation ElementDownloader

#pragma Téléchargement des données depuis le WS

- (void) downloadElementsWithPage:(int) page withCallback:(DownloaderCallback) callback
{
    if(page < 1) page = 1;
    
    NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString: [NSString stringWithFormat:@"http://thegreatpaperadventure.com/CulturezVous/index.php/element/page/%d", page]]];
    
    // Appel au webservice
    AFHTTPRequestOperation *operation = [[AFHTTPRequestOperation alloc] initWithRequest:request];
    [operation
     setCompletionBlockWithSuccess:^(AFHTTPRequestOperation *operation, id responseObject)
     {
         // On parse les éléments récupérés
         NSString *response = [operation responseString];
         NSLog(@"INFO : downElementsWithPage");
         
         NSArray *elements = [self parseXml:response];
         
         if(callback) {
             callback(elements);
         }
     }
     failure:^(AFHTTPRequestOperation *operation, NSError *error)
     {
         // Erreur survenue
         NSLog(@"ERROR: downElementsWithPage: %@", error);
     }
     ];
    
    [AFXMLRequestOperation addAcceptableContentTypes: [NSSet setWithObject:@"text/xml"]];
    
    NSOperationQueue *queue = [[NSOperationQueue alloc] init];
    [queue addOperation:operation];
}

- (NSArray*) parseXml:(NSString*) xml
{
    NSError *error;
    NSData* data = [xml dataUsingEncoding:NSUTF8StringEncoding];
	SMXMLDocument *document = [SMXMLDocument documentWithData:data error:&error];
    
    if (error) {
        NSLog(@"ERROR: document parsing: %@", error);
        return NULL;
    }
    
    NSMutableArray* elementsArray = [[NSMutableArray alloc] init];
    
	// Récupération des éléménts
	for (SMXMLElement *elementXml in [document.root childrenNamed:@"element"]) {
        
        Element *element = NULL;
        
        // Pour chaque élément, on instancie et remplit un objet
		NSString *type = [elementXml valueWithPath:@"type"];
		NSString *title = [elementXml valueWithPath:@"title"];
        NSDate *date = [DateFormatter getDateFromString:[elementXml valueWithPath:@"date"] withFormat:@"yyyy-MM-dd hh:mm:ss"];
        NSNumber *dbId = [NSNumber numberWithInt:[[elementXml valueWithPath:@"id"] intValue]];
        NSNumber *voteCount = [NSNumber numberWithInt:[[elementXml valueWithPath:@"voteCount"] intValue]];
        NSString *author = [elementXml valueWithPath:@"author"];
        NSString *authorInfo = [elementXml valueWithPath:@"authorInfo"];
        
        // Mot
        if([type isEqualToString:@"mot"]){
            
            Word *word = [ElementCache createNewWordNoContext];
            
            if(word != NULL) {
                NSMutableArray* definitionsArray = [[NSMutableArray alloc] init];
                
                // Définitions
                SMXMLElement* definitionsXml = [elementXml childNamed:@"definitions"];
                
                for (SMXMLElement *defXml in [definitionsXml childrenNamed:@"definitions"]) {
                    
                    NSString *content = [defXml valueWithPath:@"content"];
                    NSString *details = [defXml valueWithPath:@"details"];
                    
                    Definition *def = [ElementCache createNewDefinitionNoContext];
                    
                    
                    def.details = details;
                    def.content = content;
                    
                    // TODO insertion de la définition dans le mot
                    NSLog(@"DEBUG: Definition found");
                }
                
                NSLog(@"DEBUG: Word found %@",word.title);
            }
            
            element = word;
        }
        // Contrepeterie
        else if([type isEqualToString:@"contrepétrie"]) {
            
            // Contenu et solution
            NSString *content = [elementXml valueWithPath:@"content"];
            NSString *solution = [elementXml valueWithPath:@"solution"];
            
            Contrepeterie *ctp = [ElementCache createNewContreperieNoContext];
            
            ctp.content = content;
            ctp.solution = solution;
            
            NSLog(@"DEBUG: Contreperie found %@",ctp.title);
            
            element = ctp;
        }
        
        element.title = title;
        element.date = date;
        element.dbId = dbId;
        element.author = author;
        element.authorInfo = authorInfo;
        
        [elementsArray addObject:element];
        
        // Flux d'exemple
        //        <element>
        //        <type>mot</type>
        //        <id>230</id>
        //        <date>2012-10-29 00:00:00</date>
        //        <title>Matutinal</title>
        //        <definitions>
        //        <definition>
        //        <details>adjectif, litt&#xE9;raire, vieux</details>
        //        <content>Qui est relatif au matin, appartient au matin.</content>
        //        </definition>
        //        <definition>
        //        <details>exemple</details>
        //        <content>&#xC9;toile matutinale.</content>
        //        </definition>
        //        </definitions>
        //        <voteCount>0</voteCount>
        //        <author>1Jour1Mot</author>
        //        <authorInfo>https://twitter.com/#!/1jour1mot</authorInfo>
        //        </element>
        //        <element>
        //        <type>contrep&#xE9;trie</type>
        //        <id>223</id>
        //        <date>2012-10-28 00:00:00</date>
        //        <title>R&#xE9;forme du budget</title>
        //        <definitions/>
        //        <content>Faites monter les salaires! Nous profiterons ainsi des plaisirs de la chope.</content>
        //        <solution>Faites monter les salopes ! Nous profiterons ainsi des plaisirs de la chair.</solution>
        //        <voteCount>80</voteCount>
        //        <author>Contrepetephile</author>
        //        <authorInfo>https://twitter.com/#!/contrepetephile</authorInfo>
        //        </element>
	}
    
    // Tri par date
    NSArray *sortedArray;
    sortedArray = [elementsArray sortedArrayUsingComparator:^NSComparisonResult(id a, id b) {
        NSDate *first = [(Element*)a date];
        NSDate *second = [(Element*)b date];
        return [second compare:first];
    }];
    
    return sortedArray;
}

@end
