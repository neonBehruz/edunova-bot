namespace StudentAssistant.Bot.State;

public enum UserStateStep
{
    None = 0,
    MainMenu = 1,
    SelectingLevel = 2,
    SelectingSubject = 3,
    SelectingDifficulty = 4,
    SelectingQuestionCount = 5,
    InTest = 6
}
