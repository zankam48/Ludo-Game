namespace LudoGame.Classes;

public class Path
    {
        private List<Square> squares;
        public Path() { squares = new List<Square>(); }
        public void AddSquare(Square square) { squares.Add(square); }
        public Square? GetSquare(int index)
        {
            if (index >= 0 && index < squares.Count)
                return squares[index];
            return null;
        }
        public int Count => squares.Count;
        public List<Square> GetSquares() => squares;
    }