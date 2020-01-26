using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;

namespace FifoLambdaDemo
{
    public class FifoLambdaDemoStack : Stack
    {
        internal FifoLambdaDemoStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            var sqsDemoError =
                new Queue
                (
                    this,
                    "DemoError",
                    new QueueProps
                    {
                        QueueName = "demo-error",
                        RetentionPeriod = Duration.Days(14)
                    }
                );

            var sqsDemo =
                new Queue
                (
                    this,
                    "Demo",
                    new QueueProps
                    {                        
                        QueueName = "demo.fifo",
                        RetentionPeriod = Duration.Days(14),
                        DeadLetterQueue = 
                            new DeadLetterQueue
                            {
                                MaxReceiveCount = 3,
                                Queue = sqsDemoError
                            },
                        Fifo = true,
                        ContentBasedDeduplication = false                        
                    }
                );

            var s3LambdaCodeBucket =
                new Bucket
                (
                    this,
                    "DemoLambdaCodeBucket",
                    new BucketProps
                    {
                        BucketName = "lambda-code-bucket"
                    }
                );

            var lambdaFunction =
                new Function
                (
                    this,
                    "DemoLambda",
                    new FunctionProps
                    {
                        Code = Code.FromBucket(s3LambdaCodeBucket, "demo-lambda"),
                        Handler = "DemoLambda.Handler",
                        Runtime = Runtime.DOTNET_CORE_2_1,
                        RetryAttempts = 2,
                        DeadLetterQueueEnabled = true,
                        DeadLetterQueue = sqsDemoError,
                        Timeout = Duration.Seconds(30)
                    }
                );

            lambdaFunction.AddEventSource(new SqsEventSource(sqsDemo));
        }
    }
}
