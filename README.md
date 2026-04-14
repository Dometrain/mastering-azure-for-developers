# Mastering: Azure For Developers
Starter files and fully finished sample solutions for my Mastering: Azure for Developers course. The main branch currently targets .NET 9, but you can choose the .NET version of your liking by selecting the respective branch (if available).

To get started, [read the setup guide in SETUP.MDs](SETUP.md), in the root folder.  It contains a step-by-step guide of what you need to have installed and which commands to execute.  If you've followed the previous two courses in this course series, you should feel right at home.

And most of all: enjoy the course!

Kevin.

# URL Index
Throughout the course I refer to a variety of links. This is an overview of all those links - saves you a bunch of typing ;-)

### Section 1: Courese Overview
No links.

### Section 2: Azure Security Foundations: It's About the Principal

- [Azure Tools for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-node-azure-pack) — VS Code extension pack for Azure development and resource management.
- [Install the Azure CLI](https://learn.microsoft.com/en-us/dotnet/azure/install-azure-cli) — Download and install the Azure CLI for your operating system.
- [Azure Functions Core Tools (v4.x)](https://github.com/Azure/azure-functions-core-tools/tree/v4.x) — GitHub repo with installation instructions for Azure Functions Core Tools.
- [Microsoft Entra ID Product Page](https://www.microsoft.com/en-us/security/business/identity-access/microsoft-entra-id) — Microsoft's product page for Entra ID identity and access management.
- [Microsoft Entra Admin Center](https://entra.microsoft.com) — Centralized portal for managing all Microsoft Entra ID products.

### Section 3: System-based Access: Securing Azure Resources with Application Service Principals

- [OAuth2 Client Credentials Flow – Protocol Diagram](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-client-creds-grant-flow#protocol-diagram) — Protocol diagram for the OAuth2 client credentials grant flow.
- [MSAL Overview](https://learn.microsoft.com/en-us/entra/identity-platform/msal-overview) — Overview of the Microsoft Authentication Library for token acquisition and caching.
- [MSAL .NET GitHub Repository](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) — Open-source repo with samples for MSAL for .NET.
- [Microsoft.Identity.Client on NuGet](https://www.nuget.org/packages/Microsoft.Identity.Client/) — NuGet package for MSAL, used in client apps for token acquisition.
- [Microsoft.Identity.Web Basics (Wiki)](https://github.com/AzureAD/microsoft-identity-web/wiki/Microsoft-Identity-Web-basics) — Wiki explaining the basics of the Microsoft.Identity.Web library.
- [App Service Authentication Feature Architecture](https://learn.microsoft.com/en-us/azure/app-service/overview-authentication-authorization#feature-architecture) — Architecture overview of App Service built-in authentication (Easy Auth).

### Section 4: System-based Access: Securing Azure Resources with Managed Identities

- [DefaultAzureCredential API Reference](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) — API docs for DefaultAzureCredential and its credential chain order.
- [Azure RBAC Limits](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits#azure-rbac-limits) — Azure subscription limits for role-based access control assignments.
- [MS Graph – Create App Role Assignment for Service Principal](https://learn.microsoft.com/en-us/graph/api/serviceprincipal-post-approleassignedto?view=graph-rest-1.0&tabs=http) — Graph API for assigning app roles to service principals (including managed identities).

### Section 5: User-based Access: Working with Users in Your Company

- [OIDC Protocol Diagram – Access Token Acquisition](https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc#protocol-diagram-access-token-acquisition) — Protocol diagram for OpenID Connect access token acquisition flow.
- [OpenID Foundation – How Connect Works](https://openid.net/developers/how-connect-works/) — Official OpenID Foundation resource on OpenID Connect standards and flows.
- [App Service Authentication Flow](https://learn.microsoft.com/en-us/azure/app-service/overview-authentication-authorization#authentication-flow) — Authentication flow diagram for App Service Easy Auth.
- [Sidecar Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/sidecar) — Cloud design pattern for deploying components alongside a primary application.
- [Access User Claims in App Code (Easy Auth)](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-user-identities#access-user-claims-in-app-code) — How to access user identity claims from Easy Auth headers.
- [Refresh Auth Tokens (Easy Auth)](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-oauth-tokens#refresh-auth-tokens) — How to refresh authentication tokens when using Easy Auth.
- [Microsoft.Identity.Web Integration with Easy Auth (Wiki)](https://github.com/AzureAD/microsoft-identity-web/wiki/1.2.0#integration-with-azure-app-services-authentication-of-web-apps-running-with-microsoftidentityweb) — Wiki on integrating Microsoft.Identity.Web with Easy Auth.
- [MicrosoftIdentityWebAppServiceCollectionExtensions Source Code](https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web/WebAppExtensions/MicrosoftIdentityWebAppServiceCollectionExtensions.cs) — Source code for AddMicrosoftIdentityWebAppAuthentication extension method.
- [OpenID Connect Scopes](https://learn.microsoft.com/en-us/entra/identity-platform/scopes-oidc#openid-connect-scopes) — Reference for OpenID Connect scopes supported by Entra ID.
- [ID Token Claims Reference](https://learn.microsoft.com/en-us/entra/identity-platform/id-token-claims-reference) — Full list of claims available in Entra ID identity tokens.
- [Azure CLI – az role assignment](https://learn.microsoft.com/en-us/cli/azure/role/assignment?view=azure-cli-latest#az-role-assignment-create) — CLI reference for creating Azure RBAC role assignments.

### Section 6: User-based Access: External Identities in a Workforce Tenant

- [External Identities Overview](https://learn.microsoft.com/en-us/entra/external-id/external-identities-overview) — Overview of Entra External ID for engaging users beyond your organization.
- [Supported Features Comparison (Workforce vs External Tenant)](https://learn.microsoft.com/en-us/entra/external-id/customers/concept-supported-features-customers#general-feature-comparison) — Feature comparison table between workforce and external tenant configurations.

### Section 7: User-based Access: External Identities in an External Tenant

- [Native Authentication Concept](https://learn.microsoft.com/en-us/entra/identity-platform/concept-native-authentication) — Overview of native authentication for fully custom in-app authentication UIs.
- [When to Use Native Authentication](https://learn.microsoft.com/en-us/entra/identity-platform/concept-native-authentication#when-to-use-native-authentication) — Comparison of browser-delegated vs native authentication approaches.

### Section 8: Working with Azure Key Vault

- [Key Vault REST API Reference](https://learn.microsoft.com/en-us/rest/api/keyvault/) — Full REST API reference for Azure Key Vault operations.
- [Azure CLI – az keyvault Commands](https://learn.microsoft.com/en-us/cli/azure/keyvault?view=azure-cli-latest) — Azure CLI reference for managing Key Vault resources and secrets.

### Section 9: Automating Azure Infrastructure Deployments
No links.

### Section 10: Infrastructure as Code with Bicep

- [Bicep Extension for Visual Studio](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.visualstudiobicep) — Visual Studio extension for Bicep syntax highlighting, IntelliSense, and validation.
- [Azure Quickstart Templates Gallery](https://learn.microsoft.com/en-us/samples/browse/?expanded=azure&products=azure-resource-manager) — Collection of ARM and Bicep template examples for various Azure resources.


