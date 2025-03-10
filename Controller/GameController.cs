public delegate void HandleSixRollDelegate(IPlayer player, IPiece piece, int rollResult);

public class GameController {
    private Board board;
    private List<IPlayer> players;
    private IDice dice;
    private IPlayer currentPlayer;
    private bool isGameOver = false;
    private Dictionary<IPlayer, IPiece[]> pieces = new Dictionary<IPlayer, IPiece[]>();

    public event Action<IPlayer> OnNextPlayerTurn;
    public event Func<IDice, int> OnDiceRoll;
    public event HandleSixRollDelegate OnSixRoll;

    public GameController(List<IPlayer> players, Board board, IDice dice) {
        this.players = players;
        this.board = board;
        this.dice = dice;
        currentPlayer = players[0];
    }

    public void StartGame() { }
    public void EndGame() { }
    public void NextPlayerTurn() { }
    public void ExecuteTurn() { }
    public void SkipTurn() { }
    public IPlayer GetWinner() => null;
    public void UpdateBoard() { }

    public IPiece SelectPiece(IPiece piece) => piece;
    public void MovePiece(IPiece piece, int diceValue) { }
    public void AddPiece(IPiece piece) { }
    public void KickPiece(IPiece piece) { }
    public int RollDice() => dice.Roll();
    public void HandleSixRoll(IPlayer player, IPiece piece, int rollResult) { }
}
