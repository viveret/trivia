// This model will represent a category with properties like Id, Name, and a list of Questions.

namespace trivia.git.Models;

public class Category
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Question> Questions { get; set; } = new List<Question>();
}