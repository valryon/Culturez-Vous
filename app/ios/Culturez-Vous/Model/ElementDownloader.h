//
//  ElementManager.h
//  Culturez-Vous
//
//  Created by Dam on 21/10/12.
//  Copyright (c) 2012 Damien Mayance & Matthieu Oger. All rights reserved.
//

#import "AFXMLRequestOperation.h"
#import "SMXMLDocument.h"
#import "ElementCache.h"
#import "DateFormatter.h"
#import "Common.h"

typedef void(^DownloaderCallback)(NSArray* elements);

@interface ElementDownloader : NSObject <NSXMLParserDelegate>

- (void) downloadElementsWithPage:(int)page withCallback:(DownloaderCallback) callback withErrorCallback:(FailureCallback)failureCallback;

- (NSArray*)parseXml:(NSString*) data;

@end
