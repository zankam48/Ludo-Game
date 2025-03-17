namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Controller;
public class Board
    {
        public const int BOARD_SIZE = 15;

        public Square[,] grid;

        public Dictionary<Square, List<Piece>> piecePositions;

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

            piecePositions = new Dictionary<Square, List<Piece>>();

            MarkEdges();
            MarkSafeZones(safeCoords);
            PathManager = new PathManager(this);
            InitializePathVisuals();
        }

        
        public Square GetSquare(int row, int col)
        {
            if (row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE)
                return grid[row, col];
            return null;
        }

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
        public void RegisterPieceAtHome(Piece piece)
        {
            Square home = piece.HomeSquare;
            if (home != null)
            {
                if (piecePositions.ContainsKey(home))
                {
                    piecePositions[home].Add(piece);
                }
                else
                {
                    piecePositions[home] = new List<Piece> { piece };
                }
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

        public List<Piece> GetPiecesOnSquare(Square square)
        {
            if (piecePositions.TryGetValue(square, out List<Piece> pieces))
            {
                return pieces;
            }
            return new List<Piece>();
        }


        public bool IsHomeSquare(Square sq)
        {
            int r = sq.Row, c = sq.Col;
            if ((r == 2 && (c == 2 || c == 4)) || (r == 4 && (c == 2 || c == 4))) return true;      // RED
            if ((r == 2 && (c == 10 || c == 12)) || (r == 4 && (c == 10 || c == 12))) return true;  // BLUE
            if ((r == 10 && (c == 2 || c == 4)) || (r == 12 && (c == 2 || c == 4))) return true;    // GREEN
            if ((r == 10 && (c == 10 || c == 12)) || (r == 12 && (c == 10 || c == 12))) return true;// YELLOW
            return false;
        }

        public void UpdatePiecePosition(Piece piece, Square oldSquare, Square newSquare)
        {
            if (oldSquare != null)
            {
                oldSquare.RemovePiece(piece.Marker);
                if (piecePositions.ContainsKey(oldSquare))
                {
                    piecePositions[oldSquare].Remove(piece);
                    if (piecePositions[oldSquare].Count == 0)
                    {
                        piecePositions.Remove(oldSquare);
                    }
                }
            }

            if (newSquare != null)
            {
                newSquare.AddPiece(piece.Marker);

                Square previousSquare = piecePositions.FirstOrDefault(x => x.Value.Contains(piece)).Key;
                if (previousSquare != null && previousSquare != newSquare)
                {
                    piecePositions[previousSquare].Remove(piece);
                    if (piecePositions[previousSquare].Count == 0)
                    {
                        piecePositions.Remove(previousSquare);
                    }
                }

                if (piecePositions.ContainsKey(newSquare))
                {
                    piecePositions[newSquare].Add(piece);
                }
                else
                {
                    piecePositions[newSquare] = new List<Piece> { piece };
                }
            }

            if (piece.Status == PieceStatus.AT_HOME)
            {
                if (oldSquare != null && piecePositions.ContainsKey(oldSquare))
                {
                    piecePositions[oldSquare].Remove(piece);
                    if (piecePositions[oldSquare].Count == 0)
                    {
                        piecePositions.Remove(oldSquare);
                    }
                }
            }
            else
            {
                if (newSquare != null)
                {
                    if (!piecePositions.ContainsKey(newSquare))
                    {
                        piecePositions[newSquare] = new List<Piece>();
                    }
                    if (!piecePositions[newSquare].Contains(piece))
                    {
                        piecePositions[newSquare].Add(piece);
                    }
                }
            }
        }

    }
