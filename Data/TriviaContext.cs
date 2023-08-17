// This class will handle the in-memory data storage. It will load the data from the TriviaData.json.json file on startup and save the data back to the file if any data in memory changes or on shutdown.

using System.Collections.Concurrent;
using System.Text.Json;
using trivia.git.Models;

namespace trivia.git.Data;

public class TriviaContext
{
    private TriviaConfig Config { get; set; } = new TriviaConfig();
    private TriviaData Data { get; set; } = new TriviaData();
    public ConcurrentDictionary<string, Question> Questions { get; } = new ConcurrentDictionary<string, Question>();
    public ConcurrentDictionary<string, Player> Players { get; } = new ConcurrentDictionary<string, Player>();
    public ConcurrentDictionary<string, Team> Teams { get; } = new ConcurrentDictionary<string, Team>();

    public int GameDepth => Data.Categories.Max(c => c.Questions.Count);

    public TriviaContext()
    {
        LoadData();
    }

    public string TriviaDataPath => Config.TriviaDataPath ?? "TriviaData.json";

    public void LoadData()
    {
        if (File.Exists("TriviaConfig.json"))
        {
            var json = File.ReadAllText("TriviaConfig.json");
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var config = JsonSerializer.Deserialize<TriviaConfig>(json);
            if (config == null)
            {
                return;
            }

            Config = config;
        }

        if (File.Exists(TriviaDataPath))
        {
            var json = File.ReadAllText(TriviaDataPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var data = JsonSerializer.Deserialize<TriviaData>(json);
            if (data == null)
            {
                return;
            }

            Data = data;

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
        File.WriteAllText(TriviaDataPath, JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void RestartGame()
    {
        Data.RestartGame();
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

    public void AddPlayers(string[] playerNames)
    {
        foreach (var playerName in playerNames)
        {
            var playerNameWithoutEmail = playerName.Split('<')[0].Trim();
            var player = new Player
            {
                Id = playerNameWithoutEmail.Replace(' ', '_').ToLower(),
                Name = playerNameWithoutEmail,
                Score = 0,
            };
            Data.Players.Add(player);
            Players.TryAdd(player.Id, player);
        }
    }

    public void AddTeams(string[] teamNames)
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
            if (!string.IsNullOrEmpty(player.Value.TeamId))
            {
                player.Value.Team = Teams.TryGetValue(player.Value.TeamId ?? string.Empty, out var team) ? team : null;
                Teams[player.Value.TeamId ?? string.Empty].Players.Add(player.Value);
            }
            else
            {
                player.Value.Team = null;
            }
        }
        SaveData();
    }

    public bool IsNewGame()
    {
        return Data.Players.Count == 0;
    }

    public bool IsCompletedGame()
    {
        return Data.Categories.All(c => c.Questions.All(q => q.IsAnswerVisible));
    }

    public List<Team> GetWinningTeams()
    {
        var teams = Teams.Values.GroupBy(t => t.Players.Sum(p => p.Score)).ToDictionary(k => k.Key, v => v.ToList());
        var maxScore = teams.Keys.Max();
        return teams[maxScore];
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

    public void RandomizeTeams()
    {
        // randomize players into buckets
        var random = new Random();
        var buckets = Teams.Values.ToArray();

        // shuffle buckets
        for (var i = 0; i < buckets.Length; i++)
        {
            var j = random.Next(i, buckets.Length);
            var temp = buckets[i];
            buckets[i] = buckets[j];
            buckets[j] = temp;
        }

        // evenly distribute players into buckets
        var players = Players.Values.ToArray();
        
        // shuffle players
        for (var i = 0; i < players.Length; i++)
        {
            var j = random.Next(i, players.Length);
            var temp = players[i];
            players[i] = players[j];
            players[j] = temp;
        }

        for (var i = 0; i < players.Length; i++)
        {
            var j = i % buckets.Length;
            players[i].TeamId = buckets[j].Id;
        }

        AssignTeams();
    }
}