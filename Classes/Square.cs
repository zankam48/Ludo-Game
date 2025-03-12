namespace LudoGame.Classes;
using System.Collections.Generic;

public class Square
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string BaseMarker { get; set; } // Default marker (e.g., ".", "*")
    
    // ✅ Store multiple pieces in a list, but keep `Occupant` for compatibility
    private List<string> Occupants { get; set; } = new List<string>();

    public string Occupant
    {
        get
        {
            return Occupants.Count > 0 ? string.Join("", Occupants) : BaseMarker;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                Occupants.Clear();
                Occupants.Add(value);
            }
        }
    }

    public Square(int row, int col)
    {
        Row = row;
        Col = col;
        BaseMarker = " ";
    }

    // ✅ Add a piece marker without overwriting existing ones
    public void AddPiece(string marker)
    {
        if (!Occupants.Contains(marker))
        {
            Occupants.Add(marker);
        }
    }

    // ✅ Remove a specific piece while keeping others
    public void RemovePiece(string marker)
    {
        Occupants.Remove(marker);
    }

    // ✅ Clear all pieces, resetting to the base marker
    public void ResetSquare()
    {
        Occupants.Clear();
    }
}
