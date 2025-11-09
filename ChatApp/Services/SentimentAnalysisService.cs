using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Options;
using Microsoft.Extensions.Options;

namespace ChatApp.Services;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly TextAnalyticsClient? _client;
    private readonly ILogger<SentimentAnalysisService> _logger;
    private readonly bool _enabled;

    public SentimentAnalysisService(
        IOptions<TextAnalyticsOptions> options,
        ILogger<SentimentAnalysisService> logger)
    {
        _logger = logger;
        var config = options.Value;
        _enabled = config.Enabled;

        if (_enabled && !string.IsNullOrEmpty(config.Endpoint) && !string.IsNullOrEmpty(config.Key))
        {
            try
            {
                _client = new TextAnalyticsClient(
                    new Uri(config.Endpoint),
                    new AzureKeyCredential(config.Key));
                _logger.LogInformation("Sentiment analysis service initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize sentiment analysis client. Service will be disabled.");
                _enabled = false;
            }
        }
        else
        {
            _logger.LogWarning("Sentiment analysis is disabled or not configured");
            _enabled = false;
        }
    }

    public async Task<string?> AnalyzeSentimentAsync(string text)
    {
        if (!_enabled || _client == null || string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            var response = await _client.AnalyzeSentimentAsync(text);
            var sentiment = response.Value.Sentiment.ToString();
            _logger.LogDebug("Analyzed sentiment: {Sentiment} for text length: {Length}", sentiment, text.Length);
            return sentiment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment for message");
            return null;
        }
    }
}