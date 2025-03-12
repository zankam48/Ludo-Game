namespace LudoGame;

using LudoGame;
using LudoGame.Classes;
using LudoGame.Controller;
using LudoGame.Enums;
using LudoGame.Interfaces;
using System;


class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Console Ludo Game!");
            
            


            int playerCount = 0;
            while (playerCount < 2 || playerCount > 4)
            {
                Console.Write("Enter the number of players (2-4): ");
                if (int.TryParse(Console.ReadLine(), out playerCount) && playerCount >= 2 && playerCount <= 4)
                    break;
                Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
            }

            // Create the board.
            Board board = new Board();
            board.PrintBoard();

            // Create players. Each gets a unique color.
            List<Player> players = new List<Player>();
List<PieceColor> availableColors = new List<PieceColor> { PieceColor.RED, PieceColor.BLUE, PieceColor.YELLOW, PieceColor.GREEN };

for (int i = 0; i < playerCount; i++)
{
    Console.Write($"Enter name for Player {i + 1}: ");
    string playerName = Console.ReadLine() ?? $"Player{i + 1}";

    // Dynamically display remaining available colors
    Console.WriteLine("Choose piece color:");
    for (int j = 0; j < availableColors.Count; j++) // ✅ `Count` now works correctly
    {
        Console.WriteLine($"{j + 1}. {availableColors[j]}"); // ✅ Displays only remaining colors
    }

    int colorIndex;
    while (true)
    {
        Console.Write("Enter the number corresponding to your color choice: ");
        if (int.TryParse(Console.ReadLine(), out colorIndex) && colorIndex >= 1 && colorIndex <= availableColors.Count)
        {
            break; // ✅ Valid selection
        }
        Console.WriteLine("❌ Invalid choice. Please select from the available colors.");
    }

    PieceColor chosenColor = availableColors[colorIndex - 1]; // ✅ Get selected color
    players.Add(new Player(playerName, chosenColor, board)); // ✅ Assign player color
    availableColors.RemoveAt(colorIndex - 1); // ✅ Remove chosen color from available options
}


            // Create dice and the game controller.
            Dice dice = new Dice();
            GameController gameController = new GameController(players, dice, board);

            // Set up delegates.
            gameController.OnDiceRoll = (d) => d.Roll();
            gameController.OnNextPlayerTurn = (player) =>
            {
                Console.WriteLine($"\n🔄 It's now {player.Name}'s turn ({player.Color})!");
            };
            gameController.OnSixRoll = (player, piece, rollResult) =>
            {
                Console.WriteLine($"🎉 {player.Name} rolled a 6!");
                if (piece.Status == PieceStatus.AT_HOME)
                {
                    Console.WriteLine("🏠 Bringing a piece out of home!");
                }
            };

            // Game loop.
            while (true)
            {
                Player currentPlayer = gameController.currentPlayer;
                Console.WriteLine($"\nIt's {currentPlayer.Name}'s turn ({currentPlayer.Color})!");

                bool continueRolling;
                do
                {
                    continueRolling = false;
                    int rollValue = gameController.RollDice();
                    Console.WriteLine($"🎲 {currentPlayer.Name} rolled a {rollValue}.");

                    if (!gameController.CanPlayerMove(currentPlayer, rollValue))
                    {
                        Console.WriteLine("❌ No available moves. Turn skipped.");
                        break;
                    }

                    // Display pieces status.
                    Console.WriteLine("Your pieces:");
                    for (int i = 0; i < currentPlayer.Pieces.Length; i++)
                    {
                        var piece = currentPlayer.Pieces[i];
                        string status = gameController.GetPieceStatus(piece);
                        Console.WriteLine($"  [{i + 1}] Piece {i + 1}: {status}");
                    }

                    // Prompt the player to select a piece.
                    Piece selectedPiece;
                    while (true)
                    {
                        Console.Write("Select a piece to move (1-4): ");
                        if (!int.TryParse(Console.ReadLine(), out int selectedIndex) ||
                            selectedIndex < 1 || selectedIndex > 4)
                        {
                            Console.WriteLine("❌ Invalid selection. Try again.");
                            continue;
                        }
                        selectedPiece = currentPlayer.Pieces[selectedIndex - 1];

                        // If piece is at home but roll is < 6, we must move an in-play piece if possible.
                        if (selectedPiece.Status == PieceStatus.AT_HOME && rollValue < 6 &&
                            gameController.HasPieceInPlay(currentPlayer))
                        {
                            Console.WriteLine("❌ Invalid move! Choose a piece that is already in play.");
                            continue;
                        }
                        break;
                    }

                    // Check for six roll
                    if (rollValue == 6)
                    {
                        gameController.HandleSixRoll(selectedPiece, rollValue);
                        gameController.MovePiece(selectedPiece, rollValue);
                        continueRolling = true; // Extra turn for rolling 6.
                    }
                    else
                    {
                        gameController.MovePiece(selectedPiece, rollValue);
                    }

                    board.PrintBoard();

                } while (continueRolling);

                gameController.NextPlayerTurn();
            }
        }
    }


