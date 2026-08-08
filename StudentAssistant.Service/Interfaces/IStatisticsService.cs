namespace StudentAssistant.Service.Interfaces;

public class StatisticsOverviewDto
{
    public int TotalUsers { get; set; }
    public int TotalTestsTaken { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public double AverageScorePercentage { get; set; }
}

public interface IStatisticsService
{
    Task<StatisticsOverviewDto> GetOverviewAsync();
}

