using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleScheduling.Lambda;

public class Function
{
    public void FunctionHandler(EventInfo input,
        ILambdaContext context)
    {
        switch (input.ActionName)
        {
            case "PatreonMail":
                context.Logger.LogInformation($"Received call for {input.ActionName}");
                break;
            case "VipPatreonMail":
                context.Logger.LogInformation($"Received call for {input.ActionName}");
                break;
            default:
                context.Logger.LogInformation($"Unknown action");
                break;
        }
    }
}

public class EventInfo
{
    public string ActionName { get; set; }
}
