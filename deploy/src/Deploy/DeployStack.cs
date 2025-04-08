using Amazon.CDK;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.AppRunner.Alpha;
using Constructs;
using System.IO;

namespace Deploy
{
    public class DeployStack : Stack
    {
        internal DeployStack(Construct scope, string id, IStackProps props = null)
            : base(scope, id, props)
        {
            // Build and upload Docker image from Dockerfile.blazor
            
            var projectRoot = "/workspaces/BlazorDevContainerTest";

            var dockerImage = new DockerImageAsset(this, "BlazorImage", new DockerImageAssetProps
            {
                Directory = projectRoot,
                File = ".devcontainer/Dockerfile.blazor"
            });


            // App Runner service using that image
            var service = new Service(this, "BlazorAppRunner", new ServiceProps
            {
                Source = Source.FromAsset(new AssetProps
                {
                    ImageConfiguration = new ImageConfiguration
                    {
                        Port = 8080
                    },
                    Asset = dockerImage
                }),
                AutoDeploymentsEnabled = true
            });

            new CfnOutput(this, "AppRunnerURL", new CfnOutputProps
            {
                Value = service.ServiceUrl,
                Description = "Blazor App Public URL"
            });
        }
    }
}
