# Notes

use `npm install -g azure-functions-core-tools` over Chocolatey, because Chocolatey installs 32-bit version...

## Running on Azure

- deploy using ZipDeploy
- run on Azure Functions Elastic Premium 2 (EP2) app service plan for sufficient Memory

## CI/CD

[![CI/CD Status](https://dev.azure.com/modusnl/Modus/_apis/build/status/GLEIF.FunctionApp.CI?branchName=master)](https://dev.azure.com/modusnl/Modus/_build/latest?definitionId=4&branchName=master)

see [GLEIF.FunctionApp.yaml](./GLEIF.FunctionApp.yaml) for multi-stage YAML pipeline which performs both Build & Deploy

## References

- https://azure.microsoft.com/en-us/blog/understanding-serverless-cold-start
- https://azure.microsoft.com/nl-nl/blog/announcing-the-azure-functions-premium-plan-for-enterprise-serverless-workloads/
- https://devblogs.microsoft.com/devops/whats-new-with-azure-pipelines/
