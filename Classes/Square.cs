public class Square
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string BaseMarker { get; set; } // Default marker for empty squares (".", "*")

    private Stack<string> Occupants { get; set; } = new Stack<string>(); // ✅ Stack for pieces (last-in, first-out)

    public string Occupant { get; set; } // ✅ Private setter to allow controlled updates

    public Square(int row, int col)
    {
        Row = row;
        Col = col;
        BaseMarker = " ";
        Occupant = BaseMarker;
    }

    // ✅ Add a piece to the stack (latest piece appears on top)
    public void AddPiece(string marker)
    {
        Occupants.Push(marker);
        Occupant = marker; // ✅ Update display
    }

    // ✅ Remove a piece and restore the previous one
    public void RemovePiece(string marker)
    {
        if (Occupants.Count > 0 && Occupants.Peek() == marker)
        {
            Occupants.Pop(); // Remove only if it's the top piece
        }

        // ✅ Update the occupant display
        Occupant = Occupants.Count > 0 ? Occupants.Peek() : BaseMarker;
    }

    // ✅ Reset the square if it's empty
    public void ResetSquare()
    {
        Occupants.Clear();
        Occupant = BaseMarker;
    }
}
