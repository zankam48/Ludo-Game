using LudoGame.Struct;

namespace LudoGame.Classes;
public class Square
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string BaseMarker { get; set; }
    private Stack<string> Occupants { get; set; } = new Stack<string>();
    public string Occupant { get; set; }

    public Square(int row, int col)
    {
        Row = row;
        Col = col;
        BaseMarker = " ";
        Occupant = BaseMarker;
    }

    public Position Pos => new Position(Row, Col);

    public void AddPiece(string marker)
    {
        Occupants.Push(marker);
        Occupant = marker;
    }

    public void RemovePiece(string marker)
    {
        if (Occupants.Contains(marker))
        {
            List<string> temp = new List<string>(Occupants);
            temp.Remove(marker);
            Occupants = new Stack<string>(temp);
        }

        if (Occupants.Count > 0)
            Occupant = Occupants.Peek();
        else
            Occupant = BaseMarker;
    }

    public void ResetSquare()
    {
        Occupants.Clear();
        Occupant = BaseMarker;
    }

    public bool isBlockade() => Occupants.Count > 1;
}
