public class Board {
    private const int BOARD_SIZE = 15;
    private Square[,] grid = new Square[BOARD_SIZE, BOARD_SIZE];
    private Dictionary<PieceColor, Square> home = new Dictionary<PieceColor, Square>();
    private Dictionary<Position, List<Piece>> pieceMap = new Dictionary<Position, List<Piece>>();
    private PathManager pathManager = new PathManager();

    public Board() {
        InitializeGrid();
        pathManager.InitializePaths();
    }

    private void InitializeGrid() {
        for (int r = 0; r < BOARD_SIZE; r++) {
            for (int c = 0; c < BOARD_SIZE; c++) {
                grid[r, c] = new Square(r, c);
            }
        }
    }

    public Square GetSquare(Position pos) => grid[pos.Row, pos.Column];

    public void UpdatePiecePosition(Piece piece, Position newPosition) {
        // Logic to update piece position
    }

    public void HandleCollision(Piece movingPiece, Square targetSquare) {
        // Handle collision logic here
    }
}
