using Amazon.CDK;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.AppRunner.Alpha;
using Amazon.CDK.AWS.Cognito;
using Constructs;
using System.IO;
using System;

namespace Deploy
{
    public class DeployStack : Stack
    {
        internal DeployStack(Construct scope, string id, IStackProps props = null)
            : base(scope, id, props)
        {
            // Build and upload Docker image from Dockerfile.blazor
            var _dirname = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Console.WriteLine($"dirname: {_dirname}");
            var projectRoot = Path.GetFullPath(Path.Combine(_dirname, "..", "..", "..", "..", "..", ".."));
            //Console.WriteLine($"projectRoot: {projectRoot}");

            var dockerImage = new DockerImageAsset(this, "BlazorImage", new DockerImageAssetProps
            {
                Directory = projectRoot,                      
                File = "deploy/Dockerfile.blazor"             
            });


            // App Runner service
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
            
            // AWS Cognito
            var userPool = new UserPool(this, "WisdomUserPool", new UserPoolProps
            {
                UserPoolName = "WisdomUserPool",
                PasswordPolicy = new PasswordPolicy
                {
                    MinLength = 6,
                    RequireDigits = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireSymbols = false
                },
                Mfa = Mfa.OFF,
                SignInAliases = new SignInAliases
                {
                    Email = true
                },
                AutoVerify = new AutoVerifiedAttrs
                {
                    Email = true
                },
                StandardAttributes = new StandardAttributes
                {
                    Email = new StandardAttribute
                    {
                        Required = true
                    }
                },
                AccountRecovery = AccountRecovery.EMAIL_ONLY,
                SelfSignUpEnabled = true,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            var userPoolDomain = userPool.AddDomain("UserPoolDomain", new UserPoolDomainOptions
            {
                CognitoDomain = new CognitoDomainOptions
                {
                    DomainPrefix = "wisdomapp-bh4017"
                }
            });

            var userPoolClient = userPool.AddClient("blazor-client", new UserPoolClientOptions
            {
                AuthFlows = new AuthFlow
                {
                    UserPassword = true,
                    UserSrp = true
                },
                OAuth = new OAuthSettings
                {
                    CallbackUrls = new[] {
                        "https://ups5aw2pij.eu-west-2.awsapprunner.com/signin-oidc",
                        "http://localhost:5098/signin-oidc"
                    },
                    LogoutUrls = new[] {
                        "https://ups5aw2pij.eu-west-2.awsapprunner.com/signout-callback-oidc",
                        "http://localhost:5098/signout-callback-oidc"
                    },

                    Flows = new OAuthFlows
                    {
                        AuthorizationCodeGrant = true
                    },
                    Scopes = new[]
                    {
                        OAuthScope.OPENID,
                        OAuthScope.EMAIL,
                        OAuthScope.PROFILE
                    }

                },
                SupportedIdentityProviders = new[] { UserPoolClientIdentityProvider.COGNITO }
            });

            new CfnOutput(this, "UserPoolId", new CfnOutputProps
            {
                Value = userPool.UserPoolId,
                Description = "The ID of the user pool"
            });

            new CfnOutput(this, "UserPoolClientId", new CfnOutputProps
            {
                Value = userPoolClient.UserPoolClientId,
                Description = "The ID of the user pool client"
            });

            new CfnOutput(this, "UserPoolDomain", new CfnOutputProps
            {
                Value = userPoolDomain.DomainName,
                Description = "Cognito Hosted UI Domain"
            });

            new CfnOutput(this, "AppRunnerURL", new CfnOutputProps
            {
                Value = service.ServiceUrl,
                Description = "Blazor App Public URL"
            });

        }
    }
}
