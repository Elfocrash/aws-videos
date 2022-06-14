using Amazon;
using Amazon.SQS;
using Publisher;
using Publisher.Messages;

var sqsClient = new AmazonSQSClient(RegionEndpoint.EUWest2);

var publisher = new SqsPublisher(sqsClient);

await publisher.PublishAsync("customers", new CustomerCreated
{
    Id = 1,
    FullName = "Nick Chapsas"
});

await Task.Delay(5000);

await publisher.PublishAsync("customers", new CustomerDeleted
{
    Id = 1
});
