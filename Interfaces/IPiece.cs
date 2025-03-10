namespace LudoGame.Interface;
using LudoGame.Enums;
public interface IPiece
{
    PieceColor Color { get; }
    Position Position { get; set; }
    PieceStatus Status { get; set; } 
}