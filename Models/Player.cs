// This model will represent aplayer with properties like Id, Name, Score, and TeamId.

using System.Text.Json.Serialization;

namespace trivia.git.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Score { get; set;}
    public string? TeamId { get; set; }

    [JsonIgnore]
    public Team? Team { get; set; }
}