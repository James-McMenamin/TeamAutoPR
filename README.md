# PRNotifierApp

PRNotifierApp is a custom connector application for Microsoft Teams workflows that integrates with Azure DevOps. It notifies Teams channels about new and updated pull requests in Azure DevOps repositories.

## Features

- Listens for webhook events from Azure DevOps for pull request creation and updates
- Fetches detailed information about pull requests from Azure DevOps
- Creates and posts adaptive cards to specified Microsoft Teams channels
- Supports mapping multiple repositories to different Teams channels
- Configurable through JSON settings

## Prerequisites

- .NET 8.0 SDK
- Azure subscription
- Azure DevOps organization
- Microsoft Teams workspace
- Azure Functions Core Tools (for local development and deployment)

## Configuration

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/PRNotifierApp.git
   cd PRNotifierApp
   ```

2. Update the `local.settings.json` file with your configuration:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
     },
     "Teams": {
       "DefaultChannelId": "default-channel-id",
       "RepositoryChannelMappings": {
         "repo-id-1": "channel-id-1",
         "repo-id-2": "channel-id-2"
       }
     },
     "AzureDevOps": {
       "OrganizationUrl": "https://dev.azure.com/your-organization",
       "PersonalAccessToken": "your-pat-here"
     }
   }
   ```

   Replace the placeholders with your actual values.

3. Implement the `AzureDevOpsClient` and `TeamsClient` classes in the `Clients` folder to make actual API calls to Azure DevOps and Microsoft Teams.

## Obtaining Channel IDs

To configure the PRNotifierApp, you'll need to obtain the channel IDs for the Teams channels or group chats where you want to post notifications. Here's how to get these IDs:

### For a Team Channel:

1. Open Microsoft Teams in your web browser.
2. Navigate to the team and channel where you want to post notifications.
3. Copy the URL from the address bar. It should look something like this:
   `https://teams.microsoft.com/l/channel/19%3a1234567890abcdef%40thread.tacv2/General?groupId=aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee&tenantId=ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj`
4. The channel ID is the part after `19%3a` and before `%40thread.tacv2`. In this example, it would be `1234567890abcdef`.

### For a Group Chat:

1. Open Microsoft Teams in your web browser.
2. Navigate to the group chat where you want to post notifications.
3. Copy the URL from the address bar. It should look something like this:
   `https://teams.microsoft.com/l/chat/19%3a1234567890abcdef_aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee%40thread.v2/0?context=%7b%22tid%22%3a%22ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj%22%2c%22oid%22%3a%22kkkkkkkk-llll-mmmm-nnnn-oooooooooooo%22%7d`
4. The channel ID for a group chat is the entire part between `19%3a` and `%40thread.v2`. In this example, it would be `1234567890abcdef_aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee`.

Once you have the channel ID, update your `local.settings.json` or Azure Function App settings.

## Building and Running

To build the project, run:

```
dotnet build