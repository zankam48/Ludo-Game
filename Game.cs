using System;
class Game 
{
    private LudoBoard board;
    // private Player[] players;
    private List<Player> players;
    private Dice? dice;
    private GameState state;
    private int currentPlayerIdx;
    private bool isGameOver;

    public Game(int numberOfPlayers)
    {
        state = GameState.NOT_STARTED;
        board = new LudoBoard();
        dice = new Dice();
        currentPlayerIdx = 0;
        isGameOver = false;

        players = new List<Player>();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Console.WriteLine($"Enter name for Player {i + 1}:");
            string? name = Console.ReadLine();
            players.Add(new Player($"Player {i + 1}", new List<Piece>()));
        }
    }

    public void StartGame()
    {
        state = GameState.PLAYING;
        while (!isGameOver){
            // draw board

        }
    }

    public void EndGame()
    {
        isGameOver = true; 
        // Player winner = GetWinner();
        // Console.WriteLine($"{winner.Name} wins with {winner.Score} points!");
    }

    public void GetWinner()
    {

    }

    public void CreateBoard(LudoBoard board)
    {
        
    }
    
    public void ChoosePiece(List<Piece> pieces)
    {
        int index;
        if (pieces.Count == 1)
        {
            index = 0;
        }
        else
        {
            Console.WriteLine("test");
            /***
            kl pieces.Counte > 1, readline = int num, return index = num;
            ***/
        }
    }

    

    // public void MovePiece(Player player, int pieceIndex, int diceValue)
    // {
    //     var piece = player.GetPieces()[pieceIndex];

    //     Position newPosition = CalculateNewPosition(piece.Position, diceValue);
    //     piece.Position = newPosition;

    //     if (board.IsAtGoal(piece))
    //     {
    //         player.IncrementScore();
    //         Console.WriteLine($"{player.Name} reached the goal! Score: {player.Score}");
    //     }
    // }



    public int RollDice()
    {
        Random random = new Random();
        int roll = random.Next(1, 7); 
        dice.diceValue = roll;            
        Console.WriteLine($"Rolled: {roll}");
        return roll;
    }

    // method ini bisa dipake kalo semisal butuh buat ngecek2 score bbrp saat, isAtGoal ntar jadi middleware gtu pake boolean terus ntar di assign ke setGoal
    // public void UpdateScore(Player player)
    // {
    //     foreach (var piece in player.GetPieces())
    //     {
    //         if (board.IsAtGoal(piece) && !piece.IsAtGoal())
    //         {
    //             player.IncrementScore();
    //             piece.SetAtGoal(); // Set a flag to prevent double scoring
    //         }
    //     }
    // }
}
