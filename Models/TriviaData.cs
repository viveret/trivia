namespace trivia.git.Models;

public class TriviaData
{
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Player> Players { get; set; } = new List<Player>();
    public List<Team> Teams { get; set; } = new List<Team>();

    public void RestartGame()
    {
        foreach (var category in Categories)
        {
            foreach (var question in category.Questions)
            {
                question.IsQuestionVisible = false;
                question.IsAnswerVisible = false;
            }
        }

        foreach (var player in Players)
        {
            player.Score = 0;
            player.TeamId = null;
            player.Team = null;
        }

        foreach (var team in Teams)
        {
            team.Players.Clear();
        }
    }

    public void Clear()
    {
        Players.Clear();
        Teams.Clear();
        RestartGame();
    }
}
