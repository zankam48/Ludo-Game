namespace LudoGame.Classes;
public class Square
    {
        public int Row { get; set; }
        public int Col { get; set; }
        // Occupant is what is currently displayed (it could be a piece marker, ".", "*", etc.)
        public string Occupant { get; set; }
        // BaseMarker is the default marker (for example, "." for a path, "*" for a safe zone)
        public string BaseMarker { get; set; }

        public Square(int row, int col)
        {
            Row = row;
            Col = col;
            Occupant = " ";
            BaseMarker = " ";
        }
    }