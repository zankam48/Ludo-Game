namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Interfaces;

public class Piece : IPiece
    {
        public PieceColor Color { get; private set; }
        public Square Position { get; set; }
        public PieceStatus Status { get; set; }
        public int Steps { get; set; }
        public string Marker { get; private set; }
        public Square HomeSquare { get; private set; }

        public Piece(PieceColor color, string marker, Square homeSquare)
        {
            Color = color;
            Marker = marker;
            HomeSquare = homeSquare;
            Position = homeSquare; 
            Status = PieceStatus.AT_HOME;
            Steps = 0;
        }
    }

