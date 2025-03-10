namespace LudoGame.Interface;
public interface IPlayer
{
    string Name { get; }
    PieceColor Color { get; }
    IPiece[] Pieces { get; }
    int Score { get; }

    PieceColor GetColor();
    int GetScore();
    bool HasPieceAtHome();
}