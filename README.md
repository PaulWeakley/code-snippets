# Code-Snippets

## AWS

## Kafka

### Cloud Event
Based on https://github.com/cloudevents/spec

### Consumer

### Producer


# Language Dependencies
## CSharp (.Net 8)
| Library         | Version | Description                |
|-----------------|---------|----------------------------|
| Amazon.Lambda.APIGatewayEvents           | 2.7.1 | AWS SDK for API Gateway Events         |
| Amazon.Lambda.Core           | 2.3.0 | AWS Lambda Core         |
| Amazon.Lambda.Serialization.SystemTextJson         | 2.4.3 | AWS version of System.Text.Json         |
| Microsoft.Extensions.Configuration           | 8.0.0 | Configuration library         |
| Microsoft.Extensions.Configuration.Binder           | 8.0.2 | Extension Methods for GetValue in the configuration         |
| Microsoft.Extensions.Configuration.EnvironmentVariables           | 8.0.0 | Configuration extension to import Environment Variables         |
| Microsoft.Extensions.Configuration.Json           | 8.0.0 | Configuration extension for the import of JSON files (e.g. appsettings.json)         |

## JavaScript
TBD

## Python (3.12)
| Library         | Version | Description                |
|-----------------|---------|----------------------------|
| boto3           | 1.18.69 | AWS SDK for Python         |
| requests        | 2.26.0  | HTTP library for Python    |
| python-dotenv   | 1.0.1   | Loading/Reading Environment Variables      |
| confluent-kafka | 2.5.3   | Library for Kafka Interactions      |
| pymongo[srv]    | 4.9.1   | MongoDB Library      |

## TypeScript
TBD


# Release and Deployment
## CSharp
dotnet build --no-incremental -c Release
dotnet publish -c Release -r linux-x64 --self-contained false