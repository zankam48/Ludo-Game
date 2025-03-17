namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Interfaces;
using LudoGame.Struct;

public class Piece : IPiece
{
    public PieceColor Color { get; private set; }
    public Position Position { get; set; }
    public PieceStatus Status { get; set; }
    public int Steps { get; set; }
    public string Marker { get; private set; }
    public Position HomePosition { get; set; }

    public Piece(PieceColor color, string marker, Position homePosition)
    {
        Color = color;
        Marker = marker;
        HomePosition = homePosition;
        Position = homePosition; 
        Status = PieceStatus.AT_HOME;
        Steps = 0;
    }
}
