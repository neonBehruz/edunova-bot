namespace StudentAssistant.Bot.Configuration;

public class BotSettings
{
    public string Token { get; set; } = "DUMMY_TELEGRAM_BOT_TOKEN_REPLACE_IN_APPSETTINGS";
    public int QuestionTimeoutSeconds { get; set; } = 60;
}
