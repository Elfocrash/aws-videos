
using Amazon;
using Amazon.SimpleNotificationService;
using Contracts;
using Publisher;

var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.EUWest2);

var publisher = new SnsPublisher(client);

var customerUpdated = new CustomerUpdated
{
    Id = 1,
    FullName = "Nick Chapsas",
    LifetimeSpent = 420
};

await publisher.PublishAsync("<topicARN>",
    customerUpdated);


