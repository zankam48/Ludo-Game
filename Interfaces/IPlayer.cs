namespace LudoGame.Interfaces;
using LudoGame.Enums;
using LudoGame.Classes;
public interface IPlayer
{
    string Name { get; }
    PieceColor Color { get; }
    Piece[] Pieces { get; }
    int Score { get; }

    // PieceColor GetColor();
    void GetScore();
    // bool HasPieceAtHome();
}
