// namespace LudoGame;

// using LudoGame.Classes;
// using LudoGame.Controller;
// using LudoGame.Enums;
// using LudoGame.Interfaces;
// using LudoGame.Struct;

// class Program
// {
//     static void Main(string[] args)
//     {
//         IDisplay display = new Display();

//         int playerCount = 0;
//         while (playerCount < 2 || playerCount > 4)
//         {
//             playerCount = display.GetIntInput("input angka : ");
//             if (playerCount < 2 || playerCount > 4)
//             {
//                 display.DisplayMessage("invalid enter corect number:");
//             }
//         }

//         Board board = new Board();
//         display.DisplayBoard(board);

//         List<Player> players = new List<Player>();
//         List<PieceColor> availableColor = new List<PieceColor> {
//             PieceColor.BLUE,
//             PieceColor.GREEN,
//             PieceColor.RED,
//             PieceColor.YELLOW
//         };

//         for (int i=0; i<playerCount; i++)
//         {
//             string playerName = display.GetInput($"Enter name for Player {i+1}: ");
//             display.DisplayMessage("choose color");
//             for (int j=0; j<availableColor.Count; j++)
//             {
//                 display.DisplayMessage($"{j+1}.{availableColor[j]}");
//             }

//             int colorIndex;
//             while (true)
//             {
//                 colorIndex = display.GetIntInput("ENTER");
//                 if (colorIndex >= 1 && colorIndex <= availableColor.Count) break;
//                 display.DisplayMessage("�� Invalid choice. Please select from the available colors.");
//             }
//             PieceColor chosenColor = availableColor[colorIndex - 1];
//             Position[] homePositions = new Position[4];
//             for (int j=0; j<4; j++)
//             {
//                 Square homeSq = board.GetHomeSquare(chosenColor, j);
//                 homePositions[j] = homeSq.Pos;
//             }
//             Player newPlayer = new Player(playerName, chosenColor, homePositions);
//             players.Add(newPlayer);

//             foreach (var piece in newPlayer.Pieces) board.RegisterPieceAtHome(piece);
//             availableColor.RemoveAt(colorIndex - 1);
//         }

//         Dice dice = new Dice();
//         GameController gameController = new GameController(players, dice, board);
//         gameController.state = GameState.PLAYING;
//         gameController.OnDiceRoll = (d) => d.Roll();
//         gameController.OnNextPlayerTurn = (player) => display.DisplayMessage("dwd");
//         gameController.OnSixRoll = (player, piece, rollResult) =>
//         {
//             display.DisplayMessage("dw");
//             if (piece.Status == PieceStatus.AT_HOME) display.DisplayMessage("dw");
//         };

//         while (true)
//         {
//             Player currentPlayer = gameController.currentPlayer;
//             display.DisplayMessage("dwd");
//             bool triggerNext = false;

//             bool continueRolling;
//             do
//             {
//                 continueRolling = false;
//                 display.DisplayMessage("press any key to roll");
//                 Console.ReadKey(true);

//                 int rollValue = gameController.RollDice();
//                 if (!gameController.CanPlayerMove(currentPlayer, rollValue))
//                 {
//                     display.DisplayMessage("dw");
//                     break;
//                 }

//                 display.DisplayMessage("your pieces : ");

//             } while (continueRolling);
//         }
//     }
// }