// This class will handle the in-memory data storage. It will load the data from the TriviaData.json.json file on startup and save the data back to the file if any data in memory changes or on shutdown.

using System.Collections.Concurrent;
using System.Text.Json;
using trivia.git.Models;

namespace trivia.git.Data;

public class TriviaContext
{
    private TriviaData Data { get; set; }
    public ConcurrentDictionary<string, Question> Questions { get; } = new ConcurrentDictionary<string, Question>();
    public ConcurrentDictionary<string, Player> Players { get; } = new ConcurrentDictionary<string, Player>();
    public ConcurrentDictionary<string, Team> Teams { get; } = new ConcurrentDictionary<string, Team>();

    public int GameDepth => Data.Categories.Max(c => c.Questions.Count);

    public TriviaContext()
    {
        LoadData();
    }

    public void LoadData()
    {
        // read from TriviaData.json
        // deserialize into Question objects
        // add to Questions dictionary

        if (File.Exists("TriviaData.json"))
        {
            var json = File.ReadAllText("TriviaData.json");
            Data = JsonSerializer.Deserialize<TriviaData>(json);

            if (Data?.Categories != null)
            {
                Questions.Clear();
                foreach (var category in Data.Categories)
                {
                    foreach (var question in category.Questions)
                    {
                        question.Category = category;
                        Questions.TryAdd(question.Id, question);
                    }
                }
            }

            if (Data?.Teams != null)
            {
                foreach (var team in Data.Teams)
                {
                    Teams.TryAdd(team.Id, team);
                }
            }

            if (Data?.Players != null)
            {
                Players.Clear();
                foreach (var player in Data.Players)
                {
                    Players.TryAdd(player.Id, player);
                }
            }

            // Connect together data model
            if (Data?.Teams != null)
            {
                foreach (var team in Data.Teams)
                {
                    team.Players = Data.Players.Where(p => p.TeamId == team.Id).ToList();
                }
            }

            if (Data?.Players != null)
            {
                foreach (var player in Data.Players)
                {
                    player.Team = Data?.Teams?.FirstOrDefault(t => t.Id == player.TeamId);
                }
            }
        }
    }

    public void SaveData()
    {
        File.WriteAllText("TriviaData.json", JsonSerializer.Serialize(Data));
    }

    public void Reset()
    {
        Data.Reset();
        SaveData();
    }

    public void Clear()
    {
        Data.Clear();
        SaveData();
    }

    public List<Category> GetCategories()
    {
        return Data.Categories.ToList();
    }

    public void CreatePlayers(string[] playerNames)
    {
        foreach (var playerName in playerNames)
        {
            var player = new Player
            {
                Id = playerName.Trim().Replace(' ', '_').ToLower(),
                Name = playerName.Trim(),
                Score = 0,
            };
            Data.Players.Add(player);
            Players.TryAdd(player.Id, player);
        }
    }

    public void CreateTeams(string[] teamNames)
    {
        foreach (var teamName in teamNames)
        {
            var team = new Team
            {
                Id = teamName.Trim().Replace(' ', '_').ToLower(),
                Name = teamName.Trim(),
            };
            Data.Teams.Add(team);
            Teams.TryAdd(team.Id, team);
        }
    }

    public void AssignTeams()
    {
        foreach (var player in Players)
        {
            player.Value.Team = Teams[player.Value.TeamId];
            Teams[player.Value.TeamId].Players.Add(player.Value);
        }
        SaveData();
    }

    public bool IsNewGame()
    {
        return Data.Players.Count == 0;
    }

    public void AwardPoints(string playerId, int points)
    {
        Players[playerId].Score += points;
        SaveData();
    }

    public void ShowAll()
    {
        foreach (var question in Questions)
        {
            question.Value.IsQuestionVisible = true;
            question.Value.IsAnswerVisible = true;
        }
        SaveData();
    }
}