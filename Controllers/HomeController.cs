using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trivia.git.Data;
using trivia.git.Models;

namespace trivia.git.Controllers;

public class HomeController : Controller
{
    private readonly TriviaContext _triviaContext;

    public HomeController(TriviaContext triviaContext)
    {
        _triviaContext = triviaContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
