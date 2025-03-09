public class LudoBoard
{
    private const int BOARD_SIZE = 15;
    private string[][] board;
    private List<Position> safeZones;
    private Dictionary<PieceColor, List<Position>> startPositions = new Dictionary<PieceColor, List<Position>>();
    private Dictionary<PieceColor, List<Position>> goalPositions = new Dictionary<PieceColor, List<Position>>();
    private Dictionary<string, List<(int row, int col)>> mainPaths;
	private Dictionary<string, List<(int row, int col)>> goalPaths;
    private int pieceIndex = 0;      
    private bool pieceFinished = false;
    private List<(int row, int col)> fullPaths;

    public LudoBoard()
    {
        board = new string[BOARD_SIZE][];
        safeZones = new List<Position>();

        for (int r = 0; r < BOARD_SIZE; r++)
        {
            board[r] = new string[BOARD_SIZE];
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                board[r][c] = " ";
            }
        }

        InitializeHomes();
        DefineSafeZones();
        SetPath();

    }

    public void InitializeHomes()
    {
        // Red home
        MarkHome(0, 0, 6, 6, "R");

        // Blue home
        MarkHome(0, 9, 6, 15, "B");

        // Green home
        MarkHome(9, 0, 15, 6, "G");

        // Yellow home
        MarkHome(9, 9, 15, 15, "Y");
    }

    private void MarkHome(int startRow, int startCol, int endRow, int endCol, string symbol)
    {
        for (int r = startRow; r < endRow; r++)
        {
            for (int c = startCol; c < endCol; c++)
            {
                board[r][c] = symbol;
            }
        }
    }

    public void SetPath()
    {

        MarkPath(7, 6, 7, 1); // LEFT
        MarkPath(7, 1, 9, 1); // DOWN
        MarkPath(9, 1, 9, 6); // RIGHT
        MarkPath(10, 7, 15, 7); // DOWN
        MarkPath(15, 7, 15, 9); // RIGHT
        MarkPath(15, 9, 10, 9); // UP
        MarkPath(9, 10, 9, 15); // RIGHT       
        MarkPath(9, 15, 7, 15); // UP      
        MarkPath(7, 15, 7, 10); // LEFT
        MarkPath(6, 9, 1, 9); // UP
        MarkPath(1, 9, 1, 7); // LEFT
        MarkPath(1, 7, 6, 7); // DOWN
    }
    // row = y, col = x
    private void MarkPath(int row1, int col1, int row2, int col2)
    {
        int r1 = row1 - 1;
        int c1 = col1 - 1;
        int r2 = row2 - 1;
        int c2 = col2 - 1;

        int dr = (r2 > r1) ? 1 : (r2 < r1) ? -1 : 0;
        int dc = (c2 > c1) ? 1 : (c2 < c1) ? -1 : 0;

        int steps = Math.Max(Math.Abs(r2 - r1), Math.Abs(c2 - c1));

        int rr = r1;
        int cc = c1;

        for (int i = 0; i <= steps; i++)
        {
            if (board[rr][cc] == " ")
            {
                board[rr][cc] = ".";
            }
            rr += dr;
            cc += dc;
        }
    }

    public void PrintBoard()
    {
        for (int r = 0; r < BOARD_SIZE; r++)
        {
            Console.WriteLine(string.Join(" ", board[r]));
        }
    }

    public void DefineSafeZones()
    {   
        safeZones.Add(new Position(1,1));
        safeZones.Add(new Position(2,2));
        safeZones.Add(new Position(3,3));
        safeZones.Add(new Position(4,4));
    }


    public bool IsOverlapped(Player player1, Player player2)
    {
        foreach (var piece1 in player1.GetPieces())
        {
            foreach (var piece2 in player2.GetPieces())
            {
                if (piece1.Position.Equals(piece2.Position) && !IsSafeZone(piece1.Position))
                {
                    if (!IsSafeZone(piece1.Position))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    
    public bool IsSafeZone(Position position)
    {
        return safeZones.Contains(position);
    }

    public bool IsAtHome(Piece piece)
    {
        return startPositions[piece.Color].Contains(piece.Position);
    }

    public bool HasPieceAtHome(Player player)
    {
        foreach (var piece in player.GetPieces())
        {
            if (IsAtHome(piece)) 
            {
                return true; 
            }
        }
        return false;
    }

    public bool IsAtGoal(Piece piece)
    {
        return goalPositions[piece.Color].Contains(piece.Position);
    }
    
}

