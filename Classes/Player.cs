namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Interfaces;

public class Player : IPlayer
    {
        public string Name { get; private set; }
        public PieceColor Color { get; private set; }
        public Piece[] Pieces { get; private set; }
        public int Score { get; set; }

        public void GetScore()
        {
            Score += Pieces.Count(p => p.Status == PieceStatus.AT_GOAL);
        }

        public Player(string name, PieceColor color, Board board)
        {
            Name = name;
            Color = color;
            Score = 0;

            Pieces = new Piece[4];
            for (int i = 0; i < 4; i++)
            {
                Square homeSquare = board.GetHomeSquare(color, i);
                string marker = "";
                switch (color)
                {
                    case PieceColor.RED: marker = $"\u001b[31m{i + 1}\u001b[0m"; break;
                    case PieceColor.BLUE: marker = $"\u001b[34m{i + 1}\u001b[0m"; break;
                    case PieceColor.GREEN: marker = $"\u001b[32m{i + 1}\u001b[0m"; break;
                    case PieceColor.YELLOW: marker = $"\u001b[33m{i + 1}\u001b[0m"; break;
                }

                Pieces[i] = new Piece(color, marker, homeSquare);

                board.RegisterPieceAtHome(Pieces[i]);
            }
        }
    }