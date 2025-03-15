namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Controller;
public class Board
    {
        public const int BOARD_SIZE = 15;

        private Square[,] grid;

        // --- NEW: Dictionary to track which piece is on which square ---
        public Dictionary<Square, Piece> piecePositions;

        public PathManager PathManager { get; private set; }
        public List<(int, int)> safeCoords = new List<(int, int)>
            {
                (13,6), (12,8), (8,13), (6,12), (1,8), (2,6), (6,1), (8,2)
            };

      

        public Board()
        {
            grid = new Square[BOARD_SIZE, BOARD_SIZE];
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    grid[r, c] = new Square(r, c);
                }
            }

            // Initialize occupant mapping
            piecePositions = new Dictionary<Square, Piece>();

            MarkEdges();
            MarkSafeZones(safeCoords);
            PathManager = new PathManager(this);
            InitializePathVisuals();
            AssignHomes();
        }

        

        public Square GetSquare(int row, int col)
        {
            if (row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE)
                return grid[row, col];
            return null;
        }

        // Get the home square for a given color and piece index (0–3).
        public Square GetHomeSquare(PieceColor color, int pieceIndex)
        {
            // Home positions:
            // Red: (1,1), (1,4), (4,1), (4,4)
            // Blue: (1,10), (1,13), (4,10), (4,13)
            // Green: (10,1), (13,1), (10,4), (13,4)
            // Yellow: (10,10), (13,10), (10,13), (13,13)
            switch (color)
            {
                case PieceColor.RED:
                    if (pieceIndex == 0) return GetSquare(2, 2);
                    if (pieceIndex == 1) return GetSquare(2, 4);
                    if (pieceIndex == 2) return GetSquare(4, 2);
                    if (pieceIndex == 3) return GetSquare(4, 4);
                    break;
                case PieceColor.BLUE:
                    if (pieceIndex == 0) return GetSquare(2, 10);
                    if (pieceIndex == 1) return GetSquare(2, 12);
                    if (pieceIndex == 2) return GetSquare(4, 10);
                    if (pieceIndex == 3) return GetSquare(4, 12);
                    break;
                case PieceColor.GREEN:
                    if (pieceIndex == 0) return GetSquare(10, 2);
                    if (pieceIndex == 1) return GetSquare(12, 2);
                    if (pieceIndex == 2) return GetSquare(10, 4);
                    if (pieceIndex == 3) return GetSquare(12, 4);
                    break;
                case PieceColor.YELLOW:
                    if (pieceIndex == 0) return GetSquare(10, 10);
                    if (pieceIndex == 1) return GetSquare(12, 10);
                    if (pieceIndex == 2) return GetSquare(10, 12);
                    if (pieceIndex == 3) return GetSquare(12, 12);
                    break;
            }
            return null;
        }

        // --- NEW: Called by Player constructor to place a piece in home from the start ---
        public void RegisterPieceAtHome(Piece piece)
        {
            Square home = piece.HomeSquare;
            if (home != null)
            {
                // Store occupant in dictionary so we know which piece is here
                piecePositions[home] = piece;
                // Show the piece marker in that home square
                home.Occupant = piece.Marker;
            }
        }

        // Mark board edges with "."
        private void MarkEdges()
        {
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                GetSquare(0, c).Occupant = ".";
                GetSquare(0, c).BaseMarker = ".";
                GetSquare(BOARD_SIZE - 1, c).Occupant = ".";
                GetSquare(BOARD_SIZE - 1, c).BaseMarker = ".";
            }
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                GetSquare(r, 0).Occupant = ".";
                GetSquare(r, 0).BaseMarker = ".";
                GetSquare(r, BOARD_SIZE - 1).Occupant = ".";
                GetSquare(r, BOARD_SIZE - 1).BaseMarker = ".";
            }
        }

        // Mark safe zones with "*"
        private void MarkSafeZones(List<(int, int)>safeCoords)
        {
            
            foreach (var (r, c) in safeCoords)
            {
                Square sq = GetSquare(r, c);
                if (sq != null)
                {
                    sq.Occupant = "*";
                    sq.BaseMarker = "*";
                }
            }
        }

        // Set path squares (if not already marked) with "."
        private void InitializePathVisuals()
        {
            foreach (Square sq in PathManager.GetFullPath().GetSquares())
            {
                if (sq.BaseMarker == " ")
                {
                    sq.Occupant = ".";
                    sq.BaseMarker = ".";
                }
            }
        }

        // Assign the home markers for each color. (Here, the squares themselves are already set up.)
        private void AssignHomes()
        {
            // In this design the home squares are used directly via GetHomeSquare.
            // They will display a piece as soon as RegisterPieceAtHome is called.
        }

        // --- NEW: Check if a square is a home square for any color (used to decide occupant text) ---
        public bool IsHomeSquare(Square sq)
        {
            int r = sq.Row, c = sq.Col;
            if ((r == 2 && (c == 2 || c == 4)) || (r == 4 && (c == 2 || c == 4))) return true;      // RED
            if ((r == 2 && (c == 10 || c == 12)) || (r == 4 && (c == 10 || c == 12))) return true;  // BLUE
            if ((r == 10 && (c == 2 || c == 4)) || (r == 12 && (c == 2 || c == 4))) return true;    // GREEN
            if ((r == 10 && (c == 10 || c == 12)) || (r == 12 && (c == 10 || c == 12))) return true;// YELLOW
            return false;
        }

        // --- NEW: Update the occupant dictionary and the board squares after a move ---
        public void UpdatePiecePosition(Piece piece, Square oldSquare, Square newSquare)
        {
            if (oldSquare != null)
    {
        oldSquare.RemovePiece(piece.Marker); // Remove only the moving piece

        // ✅ Ensure old square is completely cleared from `piecePositions`
        if (!oldSquare.Occupant.Any(char.IsDigit)) // No more pieces left
        {
            piecePositions.Remove(oldSquare);
        }
    }

    // ✅ Ensure new square reference is set properly
    if (newSquare != null)
    {
        newSquare.AddPiece(piece.Marker);

        // ✅ Remove any previous references in case of outdated links
        if (piecePositions.ContainsValue(piece))
        {
            var previousSquare = piecePositions.FirstOrDefault(x => x.Value == piece).Key;
            if (previousSquare != null && previousSquare != newSquare)
            {
                piecePositions.Remove(previousSquare); // ✅ Remove outdated entry
            }
        }

        // ✅ Finally, update `piecePositions`
        piecePositions[newSquare] = piece;
    }

            if (piece.Status == PieceStatus.AT_HOME)
            {
                if (piecePositions.ContainsKey(oldSquare))
                    piecePositions.Remove(oldSquare);
            }
            else
            {
                piecePositions[newSquare] = piece;
            }
        }



        // --- NEW: Check if there's already a piece on targetSquare and handle collision if different color ---
        

        public void PrintBoard()
        {
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    Console.Write(grid[r, c].Occupant + " ");
                }
                Console.WriteLine();
            }
        }
    }
