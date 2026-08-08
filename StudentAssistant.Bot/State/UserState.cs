namespace StudentAssistant.Bot.State;

public enum UserStateStep
{
    None = 0,
    MainMenu = 1,
    SelectingLevel = 2,
    SelectingDifficulty = 3,
    SelectingQuestionCount = 4,
    InTest = 5
}
