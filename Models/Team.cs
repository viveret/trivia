// This model will represent a team with properties like Id, Name, and a list of Players.

using System.Text.Json.Serialization;

namespace trivia.git.Models;

public class Team
{
    public string Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public List<Player> Players { get; set; } = new List<Player>();
}