# Notes

use `npm install -g azure-functions-core-tools` over Chocolatey, because Chocolatey installs 32-bit version...

## Running on Azure
- deploy using ZipDeploy
- due to Memory contraints, run on Azure Functions Premium 2 (EP2) app service plan

## ToDo
- extend with monthly scheduled HTTP download
- fix Error 
'''
Exception while executing function: ExtractEntity One or more errors occurred. 
(The condition specified using HTTP conditional header(s) is not met.) 
The condition specified using HTTP conditional header(s) is not met. 
'''

## References
- https://azure.microsoft.com/en-us/blog/understanding-serverless-cold-start
- https://azure.microsoft.com/nl-nl/blog/announcing-the-azure-functions-premium-plan-for-enterprise-serverless-workloads/