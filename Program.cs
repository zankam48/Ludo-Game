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

            List<Player> players = new List<Player>();
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
                Player newPlayer = new Player(playerName, chosenColor, homePositions);
                players.Add(newPlayer);
                
                foreach (var piece in newPlayer.Pieces)
                    board.RegisterPieceAtHome(piece);

                availableColors.RemoveAt(colorIndex - 1);
            }

            Dice dice = new Dice();
            GameController gameController = new GameController(players, dice, board);
            gameController.state = GameState.PLAYING;
            gameController.OnDiceRoll = (d) => d.Roll();
            gameController.OnNextPlayerTurn = (player) => display.DisplayMessage($"\n🔄 It's now {player.Name}'s turn ({player.Color})!");
            gameController.OnSixRoll = (player, piece, rollResult) =>
            {
                display.DisplayMessage($"🎉 {player.Name} rolled a 6!");
                if (piece.Status == PieceStatus.AT_HOME)
                    display.DisplayMessage("🏠 Bringing a piece out of home!");
            };

            while (true)
            {
                Player currentPlayer = gameController.currentPlayer;
                display.DisplayMessage($"\nIt's {currentPlayer.Name}'s turn ({currentPlayer.Color})!");

                bool continueRolling;
                do
                {
                    continueRolling = false;
                    display.DisplayMessage("🎲 Press any key to roll the dice...");
                    Console.ReadKey(true);
                    // int rollValue = gameController.RollDice();
                    int rollValue = Convert.ToInt32(Console.ReadLine());
                    display.DisplayMessage($"🎲 {currentPlayer.Name} rolled a {rollValue}.");

                    if (!gameController.CanPlayerMove(currentPlayer, rollValue))
                    {
                        display.DisplayMessage("❌ No available moves. Turn skipped.");
                        break;
                    }

                    display.DisplayMessage("Your pieces:");
                    for (int i = 0; i < currentPlayer.Pieces.Length; i++)
                    {
                        var piece = currentPlayer.Pieces[i];
                        string status = gameController.GetPieceStatus(piece);
                        display.DisplayMessage($"  [{i + 1}] Piece {i + 1}: {status}");
                    }

                    Piece selectedPiece;
                    bool validMoveSelected = false;

                    while (!validMoveSelected)
                    {
                        int selectedIndex = display.GetIntInput("Select a piece to move (1-4): ");
                        if (selectedIndex < 1 || selectedIndex > 4)
                        {
                            display.DisplayMessage("❌ Invalid selection. Try again.");
                            continue;
                        }

                        selectedPiece = currentPlayer.Pieces[selectedIndex - 1];

                        if (!gameController.CanMovePiece(selectedPiece, rollValue))
                        {
                            display.DisplayMessage("❌ Invalid move! Choose a piece that can actually move.");
                            continue;
                        }

                        validMoveSelected = true;
                        gameController.MovePiece(selectedPiece, rollValue);
                    }

                    display.DisplayBoard(board);

                    if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                    {
                        display.DisplayMessage($"🎉 {currentPlayer.Name} has finished all pieces!");
                    }

                    if (gameController.state == GameState.FINISHED)
                    {
                        display.DisplayMessage("Game Finished! You Last Player LOSES 👎!!!");
                        Environment.Exit(0);
                    }

                    if (rollValue == 6)
                        continueRolling = true;

                } while (continueRolling);

                gameController.NextPlayerTurn();
            }
        }
    }
}
