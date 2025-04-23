# BlazorDevContainerTest
A repo to test out the development of a Blazor->.NET API -> DB application with DevContainer

# AppSettings
In BlazorDevContainerTest\src\WebApp, add a file named appsettings.json with the following setup:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  },
  "Cognito": {
    "Authority": "https://cognito-idp.<your region>.amazonaws.com/<user pool ID>",
    "MetadataAddress": "https://cognito-idp.<your region>.amazonaws.com/<user pool ID>/.well-known/openid-configuration",
    "Domain": "<user pool domain>",
    "ClientId": "<client ID>",
    "UserPoolId": "<user pool ID>",
    "Region": "<your region>",
    "LogoutRedirectUri": "http://localhost:7174/logout"
  },
  "AllowedHosts": "*"
}

```