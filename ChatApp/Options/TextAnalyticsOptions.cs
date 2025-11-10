
namespace ChatApp.Options;
//To insert Key and Endpoint to service
public class TextAnalyticsOptions
{
    public const string SectionName = "TextAnalytics";

    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}