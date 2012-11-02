//
//  ElementManager.h
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "ElementCache.h"
#import "AFXMLRequestOperation.h"
#import "SMXMLDocument.h"

typedef void(^DownloaderCallback)(NSArray* elements);

@interface ElementDownloader : NSObject <NSXMLParserDelegate>

@property (nonatomic, copy) DownloaderCallback downloadComplete;

- (void) downloadElementsWithPage:(int) page;

- (NSArray*)parseXml:(NSString*) data;

@end
