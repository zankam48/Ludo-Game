public class Piece
{
    public PieceColor Color { get; set; }
    public Position Position { get; set; }
    private bool atGoal = false;

    public Piece(PieceColor color, Position position)
    {
        Color = color;
        Position = position;
    }
    
}

