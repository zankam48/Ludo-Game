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

    public string GetPieceStatus()
    {
        if (Status == PieceStatus.AT_HOME) return PieceStatus.AT_HOME.ToString();
        if (Status == PieceStatus.AT_GOAL) return PieceStatus.AT_GOAL.ToString();
        return (Position.Row, Position.Column).ToString();
    }

    public void UpdatePieceStatus(PieceStatus pieceStatus)
    {
        Status = pieceStatus;
    }
}
