
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

IAmazonSecretsManager secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.EUWest2);

var listVersionIdsRequest = new ListSecretVersionIdsRequest
{
    SecretId = "Production_SecretsManagement.Api_Database__ConnectionString"
};

var version = await secretsManager.ListSecretVersionIdsAsync(listVersionIdsRequest);

var request = new GetSecretValueRequest
{
    SecretId = "Production_SecretsManagement.Api_Database__ConnectionString",
    VersionId =version.Versions.First().VersionId
};

var result = await secretsManager.GetSecretValueAsync(request);

Console.WriteLine(result.SecretString);
