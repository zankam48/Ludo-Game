public struct Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public override bool Equals(Object obj)
    {
        return obj is Position other && Row == other.Row && Column == other.Column;
    }

    public override int GetHashCode()
    {
        return (Row, Column).GetHashCode();
    }
}