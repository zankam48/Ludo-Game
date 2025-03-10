public class Player
{
    public string? name;
    public int Score { get; private set; }
    private List<Piece> pieces;
    public PieceColor Color { get; set; }

    public Player(string name, List<Piece> pieces)
    {
        this.name = name;
        this.pieces = pieces;
    }

    public void SetColor(PieceColor color)
    {
        Color = color; 

        foreach (var piece in pieces)
        {
            piece.Color = color;
        }
    }



    public List<Piece> GetPieces()
    {
        return pieces;
    }

    // public bool CheckWin()
    // {
    //     return false;
    // }

    // public void IncrementScore()
    // {
    //     Score += 1;
    // }

}