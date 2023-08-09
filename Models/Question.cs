// This model will represent a question with properties like Id, Text, Answer, Points, and CategoryId.

using System.Text.Json.Serialization;

namespace trivia.git.Models;

public class Question
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Answer { get; set; }
    public int Points { get; set; }
    public bool IsQuestionVisible { get; set; }
    public bool IsAnswerVisible { get; set; }

    [JsonIgnore]
    public Category Category { get; set; }
}