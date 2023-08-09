// This model will represent a category with properties like Id, Name, and a list of Questions.

namespace trivia.git.Models;

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
}