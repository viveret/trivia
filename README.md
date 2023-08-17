# Trivia Game

## Summary:
This is a simple party trivia game that can be played by 3-33 players. The game is played by answering trivia questions and the team with the most points at the end of the game wins.
This app was written in C# using the .NET Core framework and the ASP.NET Core MVC framework. The app uses a JSON file to store the questions, answers, players, teams, and game data. The app uses the Bootstrap framework for styling.


(If we can support it, we should be able to run this in a container accessible only to players so they don't have to install anything and the data is centralized/accessible/secure.)

## Rules / How to Play:
- One player will be the host and will be in charge of managing players and teams, starting the game, and reading the questions and answers.
- It works best to assign players into teams before the game starts, and assign them randomly so that everything is fair.
- Each team is responsible for coming up with a name, choosing questions, and submitting an answer.
- The team that chose the question is the first to be checked against the answer, but if their answer is not correct then the teams that submitted an answer will be the only teams to receive points for that question if they are correct.
- The host will be able to see which teams submitted an answer and will be able to award points accordingly.
- The host will also be able to see the correct answer to each question.
- The host will be able to start a new game once the current game is over.


## Data Models:
- Categories:
    - `Id` (string)
    - `Name` (string)
    - Questions:
        - `Id` (string)
        - `Text` (string)
        - `Answer` (string)
        - `Source` (string)
        - `Points` (int)
        - `IsAnswerVisible` (bool)
        - `IsQuestionVisible` (bool)
- Players:
    - `Id` (string)
    - `Name` (string)
    - `TeamId` (string)
- Teams:
    - `Id` (string)
    - `Name` (string)
    - `Players` (List of Players)

## Making Changes / Adding Game Data:
- TriviaData.json contains all of the game data. This file can be edited to add new questions, answers, players, teams, etc.
- The C# project is organized into folders that correspond to different parts of the app or data models.