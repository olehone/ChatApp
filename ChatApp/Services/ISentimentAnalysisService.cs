namespace ChatApp.Services;

public interface ISentimentAnalysisService
{
    Task<string?> AnalyzeSentimentAsync(string text);
}