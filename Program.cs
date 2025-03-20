    namespace LudoGame
    {
        using LudoGame.Classes;
        using LudoGame.Controller;
        using LudoGame.Enums;
        using LudoGame.Interfaces;
        using LudoGame.Struct;
        using System;
        using System.Collections.Generic;
        using System.Linq;

        class Program
        {
            static void Main(string[] args)
            {
                IDisplay display = new Display();

                int playerCount = 0;
                while (playerCount < 2 || playerCount > 4)
                {
                    playerCount = display.GetIntInput("Enter the number of players (2-4): ");
                    if (playerCount < 2 || playerCount > 4)
                        display.DisplayMessage("Invalid input. Please enter a number between 2 and 4.");
                }

                Board board = new Board();
                display.DisplayBoard(board);

                IPlayer[] players = new IPlayer[playerCount];
                List<PieceColor> availableColors = new List<PieceColor> { PieceColor.RED, PieceColor.BLUE, PieceColor.YELLOW, PieceColor.GREEN };

                for (int i = 0; i < playerCount; i++)
                {
                    string playerName = display.GetInput($"Enter name for Player {i + 1}: ");
                    display.DisplayMessage("Choose piece color:");
                    for (int j = 0; j < availableColors.Count; j++)
                        display.DisplayMessage($"{j + 1}. {availableColors[j]}");

                    int colorIndex;
                    while (true)
                    {
                        colorIndex = display.GetIntInput("Enter the number corresponding to your color choice: ");
                        if (colorIndex >= 1 && colorIndex <= availableColors.Count)
                            break;
                        display.DisplayMessage("❌ Invalid choice. Please select from the available colors.");
                    }
                    PieceColor chosenColor = availableColors[colorIndex - 1];

                    Position[] homePositions = new Position[4];
                    for (int j = 0; j < 4; j++)
                    {
                        Square homeSq = board.GetHomeSquare(chosenColor, j);
                        homePositions[j] = homeSq.Pos;
                    }
                    IPlayer newPlayer = new Player(playerName, chosenColor, homePositions);
                    players[i] = newPlayer;
                    
                    foreach (var piece in newPlayer.Pieces)
                        board.RegisterPieceAtHome(piece);

                    availableColors.RemoveAt(colorIndex - 1);
                }

                Dice dice = new Dice();
                GameController gameController = new GameController(players, dice, board, display);
                gameController.StartGame();
                
            }
        }
    }
