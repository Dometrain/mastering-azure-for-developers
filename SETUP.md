# Before You Start: Environment and Code Setup Guide

This guide covers everything you need to set up your development environment, create the Azure infrastructure, and deploy the starter code for this course.

> **Returning from the previous courses?** You likely already have most prerequisites installed. Skip to [Create the Azure Infrastructure](#create-the-azure-infrastructure).

---

## Prerequisites

### Development Tools

| Tool | Purpose | Install Link |
|------|---------|-------------|
| **Visual Studio** (with ASP.NET & Web Development + Azure Development workloads) *or* **VS Code** (with [Azure Tools extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-node-azure-pack)) | IDE | [Visual Studio](https://visualstudio.microsoft.com/) |
| **Azure CLI** | Create & manage Azure resources | [Install Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) |
| **Azure Functions Core Tools** | Run & deploy Azure Functions | [Install Core Tools](https://github.com/Azure/azure-functions-core-tools/tree/v4.x) |

### Verify your installations

```powershell
# Azure CLI
az --version

# Update Azure CLI if needed
az upgrade

# Azure Functions Core Tools
func --version
```

> **Tip:** If you installed the Azure Development workload in Visual Studio, the Azure Functions Core Tools should already be installed.

---

## Create the Azure Infrastructure

The `create-azure-infrastructure.ps1` script creates all required Azure resources from scratch.

> ⚠️ **Warning:** The script **deletes** the resource groups from the previous courses in this series (`appdev-rg-dev-we` and `appdev-rg-dev-we-travelinspiration`). Back up anything you want to keep before running it.

### 1. Configure the script parameters

Open `create-azure-infrastructure.ps1` and update these parameters:

| Parameter | What to change | Example |
|-----------|---------------|---------|
| `$UniqueIdentifier` | Set a short alphanumeric suffix (max 8 chars) to ensure globally unique resource names | `"i9e5m2"` |
| `$PublisherName` | Your name (used for APIM) | `"YourName"` |
| `$PublisherEmail` | Your email (used for APIM) | `"you@example.com"` |
| `$EntraAdminDisplayName` | Your Entra ID display name | `"Your Name"` |
| `$EntraAdminObjectId` | Your Entra ID user object ID (find this in the Azure Portal → Entra ID → Users → your user) | `"b2ff641c-..."` |

The `$SqlAdminUser` and `$SqlAdminPassword` parameters can be left as-is or changed to your preference.

### 2. Run the script

```powershell
.\create-azure-infrastructure.ps1
```

This takes several minutes. The script will output an **Application Insights connection string** at the end — note it down for use in your app settings during development.

### Resources created

| Resource Group | Resource | Name Pattern |
|---------------|----------|-------------|
| `appdev-rg-dev-we[-suffix]` | Log Analytics | `appdev-log-dev-we[-suffix]` |
| | Application Insights | `appdev-appi-dev-we[-suffix]` |
| | API Management (Developer tier) | `appdev-apim-dev-we[-suffix]` |
| `appdev-rg-dev-we-ti[-suffix]` | App Service Plan (B1) | `appdev-plan-dev-we-ti[-suffix]` |
| | Web App — Client | `appdev-app-dev-we-ticlient[-suffix]` |
| | Web App — JS Client | `appdev-app-dev-we-ticlientjs[-suffix]` |
| | Web App — Destinations API | `appdev-app-dev-we-tidestapi[-suffix]` |
| | Storage Account | `appdevstdevweti[suffix]` |
| | Function App — Itineraries API | `appdev-func-dev-we-tiitinapi[-suffix]` |
| | SQL Server | `appdev-sql-dev-we-ti[-suffix]` |
| | SQL Database (Basic DTU) | `appdev-sqldb-dev-we-ti[-suffix]` |

> **Why "ti"?** Function app names get truncated to 32 characters for host ID generation. Keeping names short avoids host ID collisions when scaling or using deployment slots. See [Azure resource name rules](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-name-rules).

---

## Configure and Run the Starter Code

### 1. Allow your IP to access SQL Server

1. In the Azure Portal, navigate to your SQL Server resource → **Networking**
2. Set **Public network access** to **Selected networks**
3. Add your current client IP address as a firewall rule
4. **Save**

> You'll need to repeat this whenever your IP address changes.

### 2. Set the database connection string

In `TravelInspiration.API.Itineraries/local.settings.json`, set the `TravelInspirationDbConnection` value:

```
Server=tcp:<your-sql-server>.database.windows.net,1433;Initial Catalog=<your-sql-database>;Persist Security Info=False;User ID=<SqlAdminUser>;Password=<SqlAdminPassword>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Replace `<your-sql-server>`, `<your-sql-database>`, `<SqlAdminUser>`, and `<SqlAdminPassword>` with the values from the script parameters.

### 3. Run EF Core migrations

From the **Package Manager Console** (default project: `TravelInspiration.API.Itineraries`):

```powershell
Add-Migration InitialMigration
Update-Database
```

This creates the database tables and seeds them with demo data.

### 4. (Optional) Connect to Application Insights locally

If you want local logs to appear in Application Insights, add the connection string from the script output to `TravelInspiration.API.Destinations/appsettings.Development.json`:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "<your-app-insights-connection-string>"
  }
}
```

---

## Deploy to Azure

**Deploy the Itineraries API**

From the `TravelInspiration.API.Itineraries` directory:

```powershell
dotnet build --configuration Release
func azure functionapp publish <your-function-app-name> --dotnet-version 9.0
```

**Deploy the Destinations API:**

From the `TravelInspiration.API.Destinations` directory:

```powershell
dotnet publish --configuration Release --output "./publish"
Compress-Archive -Path "./publish/*" -DestinationPath "c:/tmp/app.zip" -Force
az webapp deploy --resource-group appdev-rg-dev-we-ti-<suffix> --name appdev-app-dev-we-tidestapi-<suffix> --src-path "c:/tmp/app.zip" --type zip
```

**Deploy the Web Client:**

From the `TravelInspiration.Client.Web` directory:


```powershell
dotnet publish --configuration Release --output "./publish"
Compress-Archive -Path "./publish/*" -DestinationPath "c:/tmp/app.zip" -Force
az webapp deploy --resource-group appdev-rg-dev-we-ti-<suffix> --name appdev-app-dev-we-ticlient-<suffix> --src-path "c:/tmp/app.zip" --type zip
```

Replace `<suffix>` with your `UniqueIdentifier` value.

---

Ready to go, I hope you'll enjoy the course! 🚀
