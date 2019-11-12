1. Need to download:
    -Bot Framework v4 SDK Templates for Visual Studio (https://marketplace.visualstudio.com/items?itemName=BotBuilder.botbuilderv4)
    -Bot Framework Emulator (https://github.com/microsoft/BotFramework-Emulator/releases/)
    -Azure CLI (https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest)
2. Create Empty Bot from template.
3. Install NuGet package:
    - using Microsoft.Bot.Builder.Dialogs;
5. Add ToDoDialog and register it.
6. Add dialog steps.
7. Make ConcurrentDictionary as a cache storage and register it.
8. Create NotifyController with sending a proactive message logic.
9. Create Azure Function with timer trigger.
10. Login to Azure CLI and enter subscription Id
11. Create Azure app:
    az ad app create --display-name "application-name" --password "password123" --available-to-other-tenants
    Need to get copy appId
12. Create Web app:
    az deployment create --name "azure-bot-deplyment" --template-file "./DeploymentTemplates/template-with-new-rg.json" --location "West Europe" --parameters appId="APP_ID_FROM_PREVIOUS_STEP" appSecret="password123" botId="azure-service-bot-app" botSku=F0 newAppServicePlanName="azure-bot-service-plan" newWebAppName="azure-bot-web-app" groupName="azure-bot-group" groupLocation="West Europe" newAppServicePlanLocation="West Europe"
13. Prepere you code for deploying
    az bot prepare-deploy --lang Csharp --code-dir "." --proj-file-path "YOUR_BOT_NAME.csproj"
14. Deploy code to Azure
    az webapp deployment source config-zip --resource-group "azure-bot-group" --name "azure-bot-web-app" --src "YOUR_ARCHIVE_NAME.zip"
    Note: it should has only zip extension
15. Get telegram bot token and add it to Azure Bot Service
16. Deploy Azure function with timer trigger. Add bot notification endpoint to it.
17. Need to add Bot Application Id to Web App Application settings.