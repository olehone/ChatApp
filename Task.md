Task: Real-time Chat Application with Sentiment Analysis

Objective: Build a real-time chat application using Azure services, with an optional integration of sentiment analysis using Azure Cognitive Services.

Notes: For the backend, you should use ASP.NET Core. You can choose any framework you prefer for the frontend.

Steps:

1. Register an account on Azure portal

o Visit Azure portal and create an account if you don't already have one.

2. Create an Azure Web App for .NET Core and UI projects

o Configure the necessary settings such as resource group, service plan, and region.

3. Create an Azure SignalR Service

o Create a new SignalR Service with the appropriate settings for your chat application.

4. Create an ASP.NET Core Web application and UI app and deploy it to the Azure Requirements:

o Real-time Chat Feature:

▪ Implement real-time chat functionality using Azure SignalR Service.

▪ Ensure that users can send and receive messages in real-time.

o Optional Sentiment Analysis Integration:

Note: This step is optional and not necessary to pass the test task. However, completing this will greatly enhance your chances of success.

▪ Integrate Azure Cognitive Services Text Analytics API to perform sentiment analysis on chat messages.

▪ Display sentiment analysis results in real-time within the chat UI if integrated.

o Data Storage:

▪ Store chat messages and sentiment analysis results (if integrated) in Azure SQL Database.

o UI Enhancements:

▪ Highlight messages with positive, negative, or neutral sentiment in the chat UI for easy identification if sentiment analysis is integrated. (you can do it with color, text note, emoji, etc.)

o Deployment:

▪ Deploy the ASP.NET Core Web application and UI Web application to the Azure.

