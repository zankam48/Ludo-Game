namespace LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Struct;
using System.Linq;
using System.Collections.Generic;

public class Board
{
    public const int BoardSize = 15;
    public Square[,] grid;
    public Dictionary<Position, List<Piece>> piecePositions;
    public PathManager PathManager { get; private set; }
    public List<Position> safeCoords = new List<Position>
    {
        new Position(13,6), new Position(12,8), new Position(8,13), new Position(6,12), 
        new Position(1,8), new Position(2,6), new Position(6,1), new Position(8,2)
    };

    public Action<Piece> KickPieceDelegate { get; set; }

    public Board()
    {
        grid = new Square[BoardSize, BoardSize];
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                grid[r, c] = new Square(r, c);
            }
        }

        piecePositions = new Dictionary<Position, List<Piece>>();

        MarkEdges();
        MarkSafeZones(safeCoords);
        PathManager = new PathManager((r, c) => GetSquare(r, c));
        InitializePathVisuals();
    }

    public Square GetSquare(int row, int col)
    {
        if (row >= 0 && row < BoardSize && col >= 0 && col < BoardSize)
            return grid[row, col];
        return null;
    }

    public Square GetHomeSquare(PieceColor color, int pieceIndex)
    {
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

    public void HandleCollision(Piece movingPiece, Square targetSquare)
    {
        if (targetSquare == null) return;

        if (safeCoords.Contains(targetSquare.Pos))
            return;

        if (piecePositions.TryGetValue(targetSquare.Pos, out List<Piece> piecesOnSquare))
        {
            foreach (var occupant in piecesOnSquare.Where(p => p.Color != movingPiece.Color).ToList())
            {
                KickPieceDelegate?.Invoke(occupant);
            }
        }
    }

    

    public void RegisterPieceAtHome(Piece piece)
    {
        Position homePos = piece.HomePosition;
        if (piecePositions.ContainsKey(homePos))
        {
            piecePositions[homePos].Add(piece);
        }
        else
        {
            piecePositions[homePos] = new List<Piece> { piece };
        }

        Square homeSquare = GetSquare(homePos.Row, homePos.Column);
        if (homeSquare != null)
        {
            homeSquare.Occupant = piece.Marker;
        }
    }

    private void MarkEdges()
    {
        for (int c = 0; c < BoardSize; c++)
        {
            GetSquare(0, c).Occupant = ".";
            GetSquare(0, c).BaseMarker = ".";
            GetSquare(BoardSize - 1, c).Occupant = ".";
            GetSquare(BoardSize - 1, c).BaseMarker = ".";
        }
        for (int r = 0; r < BoardSize; r++)
        {
            GetSquare(r, 0).Occupant = ".";
            GetSquare(r, 0).BaseMarker = ".";
            GetSquare(r, BoardSize - 1).Occupant = ".";
            GetSquare(r, BoardSize - 1).BaseMarker = ".";
        }
    }

    private void MarkSafeZones(List<Position> safeCoords)
    {
        foreach (var pos in safeCoords)
        {
            Square sq = GetSquare(pos.Row, pos.Column);
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
        if (piecePositions.TryGetValue(square.Pos, out List<Piece> pieces))
            return pieces;
        return new List<Piece>();
    }

    public bool IsHomeSquare(Square sq)
    {
        int r = sq.Row, c = sq.Col;
        if ((r == 2 && (c == 2 || c == 4)) || (r == 4 && (c == 2 || c == 4))) return true;
        if ((r == 2 && (c == 10 || c == 12)) || (r == 4 && (c == 10 || c == 12))) return true;
        if ((r == 10 && (c == 2 || c == 4)) || (r == 12 && (c == 2 || c == 4))) return true;
        if ((r == 10 && (c == 10 || c == 12)) || (r == 12 && (c == 10 || c == 12))) return true;
        return false;
    }

    public void UpdatePiecePosition(Piece piece, Square oldSquare, Square newSquare)
    {
        if (oldSquare != null)
        {
            oldSquare.RemovePiece(piece.Marker);
            Position oldPos = oldSquare.Pos;
            if (piecePositions.ContainsKey(oldPos))
            {
                piecePositions[oldPos].Remove(piece);
                if (piecePositions[oldPos].Count == 0)
                {
                    piecePositions.Remove(oldPos);
                }
            }
        }

        if (newSquare != null)
        {
            newSquare.AddPiece(piece.Marker);
            Position newPos = newSquare.Pos;
            var previousEntry = piecePositions.FirstOrDefault(x => x.Value.Contains(piece));
            if (!previousEntry.Equals(default(KeyValuePair<Position, List<Piece>>)) && !previousEntry.Key.Equals(newPos))
            {
                piecePositions[previousEntry.Key].Remove(piece);
                if (piecePositions[previousEntry.Key].Count == 0)
                {
                    piecePositions.Remove(previousEntry.Key);
                }
            }
            if (piecePositions.ContainsKey(newPos))
            {
                piecePositions[newPos].Add(piece);
            }
            else
            {
                piecePositions[newPos] = new List<Piece> { piece };
            }
        }
    }
}
