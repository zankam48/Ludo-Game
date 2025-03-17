// namespace LudoGame.Classes;
// using LudoGame.Interfaces;
// using LudoGame.Enums;

// public class PlayerAI
// {
//     public string Name = "Stockfish";
//     public PieceColor Color { get; private set; }
//     public Piece[] Pieces { get; private set; }
//     public int Score { get; set; }

//     public PlayerAI(string name, PieceColor color, Board board)
//     {
//         Name = name;
//         Color = color;
//         Score = 0;

//         // Create 4 pieces. The home square for each piece is retrieved from the board.
//         Pieces = new Piece[4];
//         for (int i = 0; i < 4; i++)
//         {
//             Square homeSquare = board.GetHomeSquare(color, i);
//             // Build the piece marker using ANSI codes (for example, red: "\u001b[31m1\u001b[0m")
//             string marker = "";
//             switch (color)
//             {
//                 case PieceColor.RED: marker = $"\u001b[31m{i + 1}\u001b[0m"; break;
//                 case PieceColor.BLUE: marker = $"\u001b[34m{i + 1}\u001b[0m"; break;
//                 case PieceColor.GREEN: marker = $"\u001b[32m{i + 1}\u001b[0m"; break;
//                 case PieceColor.YELLOW: marker = $"\u001b[33m{i + 1}\u001b[0m"; break;
//             }

//             Pieces[i] = new Piece(color, marker, homeSquare);
            

//             // --- NEW: Immediately register the piece at home so it appears in the board from the start ---
//             board.RegisterPieceAtHome(Pieces[i]);
//         }
//     }

// }

// /***
// Condition :
// - Choose which piece to move by the AI
//   -> if there's a piece where the AI can kick another player piece, then choose it
//   -> distance from opps player
//   -> distance from the goal
//   -> likelihood to attack opps 
//   -> seeking safe zones
//   -> threat from opps
//   -> the need to move forward
// ***/