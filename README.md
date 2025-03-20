# 🎲 Ludo Game – C# Console Version

Welcome to the Ludo Game, a classic board game implemented in C# with a console-based UI! 🏆 This project was built using Object-Oriented Programming (OOP) principles with a structured game flow.

## 🚀 Features

- ✅ Fully Playable Ludo Game with 2-4 Players
- ✅ Console-Based UI for a smooth experience
- ✅ Turn-Based Gameplay with dice rolling
- ✅ Automatic Collision Handling (Kick opponent pieces!)
- ✅ Safe Zones & Goal Paths
- ✅ Multiplayer (Local Play) Support
- ✅ Play Again Functionality after game ends

## 🛠️ Installation

1.  **Clone the Repository**

    ```sh
    git clone [https://github.com/yourusername/LudoGame.git](https://github.com/yourusername/LudoGame.git)
    cd LudoGame
    ```

2.  **Build the Project**

    Make sure you have .NET SDK installed. Then, run:

    ```sh
    dotnet build
    ```

3.  **Run the Game**

    ```sh
    dotnet run
    ```


## 🎲 Game Flow

1.  Choose the number of players (2-4).
2.  Each player selects a color (Red, Blue, Green, Yellow).
3.  Players take turns rolling the dice (Press any key).
4.  If a 6 is rolled, a piece can enter the board from home.
5.  Move your pieces along your path to the goal.


## ⚙️ Game Mechanics & Code Overview

### 🏗️ Key Components

| Feature             | Code Implementation             |
| ------------------- | ------------------------------- |
| - Rolling Dice     | `RollDice()` in `GameController.cs` |
| - Moving a Piece   | `MovePiece()` in `GameController.cs` |
| - Next Player Turn | `NextPlayerTurn()` in `GameController.cs` |
| - Winning Condition | `GetWinner()` in `GameController.cs` |
| - Home & Goal Path | `PathManager.cs`                |
| - Collision Handling | `HandleCollision()` in `Board.cs` |


