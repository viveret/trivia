using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using trivia.git.Data;
using trivia.git.Models;

namespace trivia.git.Controllers;

public class GameController : Controller
{
    private readonly TriviaContext _triviaContext;

    public GameController(TriviaContext triviaContext)
    {
        _triviaContext = triviaContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StartNew()
    {
        _triviaContext.Clear();
        return RedirectToAction("AddPlayersAndTeams");
    }

    public IActionResult AddPlayersAndTeams(string playerNames, string teamNames)
    {
        // if post, then begin team assignment with players
        // if get, then show form to create new game
        if (Request.Method == "POST")
        {
            if (!string.IsNullOrWhiteSpace(playerNames))
            {
                _triviaContext.AddPlayers(playerNames.Split(','));
            }

            if (!string.IsNullOrWhiteSpace(teamNames))
            {
                _triviaContext.AddTeams(teamNames.Split(','));
            }

            // go to next page
            return RedirectToAction("AssignTeams");
        }
        return View();
    }

    public IActionResult AssignTeams()
    {
        // if post, then start game
        // if get, then show form to assign teams
        if (Request.Method == "POST")
        {
            // look at form data and match name id to team id
            foreach (var player in _triviaContext.Players.Values)
            {
                player.TeamId = Request.Form?.TryGetValue(player.Id, out var teamId) ?? false ? teamId.ToString() : string.Empty;
            }
            _triviaContext.AssignTeams();
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult RandomizeTeams()
    {
        if (Request.Method == "POST")
        {
            _triviaContext.RandomizeTeams();
            return RedirectToAction("Index");
        }
        else
        {
            return BadRequest();
        }
    }

    public IActionResult Question(string id)
    {
        var q = _triviaContext.Questions[id];
        q.IsQuestionVisible = true;
        _triviaContext.SaveData();
        return View(q);
    }

    public IActionResult Answer(string id)
    {
        var q = _triviaContext.Questions[id];
        if (Request.Method == "POST")
        {
            q.IsAnswerVisible = true;
            _triviaContext.SaveData();
        }

        return View(q);
    }

    public IActionResult RestartGame()
    {
        if (Request.Method == "POST")
        {
            _triviaContext.RestartGame();
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult ClearGame()
    {
        if (Request.Method == "POST")
        {
            _triviaContext.Clear();
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    public IActionResult AwardPoints(string playerId, int points)
    {
        if (Request.Method == "POST")
        {
            _triviaContext.AwardPoints(playerId, points);
            return RedirectToAction("Index");
        }
        else
        {
            return BadRequest();
        }
    }

    public IActionResult ShowAll()
    {
        _triviaContext.ShowAll();
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
