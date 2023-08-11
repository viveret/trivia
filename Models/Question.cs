// This model will represent a question with properties like Id, Text, Answer, Points, and CategoryId.

using System.Text.Json.Serialization;

namespace trivia.git.Models;

public class Question
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsQuestionVisible { get; set; }
    public bool IsAnswerVisible { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }
}