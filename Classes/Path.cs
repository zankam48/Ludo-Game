namespace LudoGame.Classes;

public class Path
    {
        private List<Square> _squares;
        public Path() { _squares = new List<Square>(); }
        public void AddSquare(Square square) { _squares.Add(square); }
        public Square? GetSquare(int index)
        {
            if (index >= 0 && index < _squares.Count)
                return _squares[index];
            return null;
        }
        public int Count => _squares.Count;
        public List<Square> GetSquares() => _squares;
    }