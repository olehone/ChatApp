# ChatApp

A real-time chat application built with ASP.NET Core and Azure services. 

 https://chattask.azurewebsites.net/

Chat is integrated using SignalR.
Sentiment analysis is integrated using Azure Cognitive Services Text Analytics.

Features:
- Real-time chat using Azure SignalR Service
- Sentiment analysis displayed in the chat UI
- Messages stored in Azure SQL Database
- Simple frontend using plain HTML, CSS, and JavaScript

Setup:
1. Configure environment variables in Azure App Service according to appsettings.json
2. Deploy the ASP.NET Core backend and frontend files to Azure Web App
3. Ensure Azure SignalR Service and Azure SQL Database are set up and connected
4. Run the application; users can send and receive messages in real-time

Notes:
- No authentication is implemented
- Frontend uses plain JavaScript, no framework
