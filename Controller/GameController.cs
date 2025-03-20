namespace LudoGame.Controller;
using LudoGame.Interfaces;
using LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Struct;
using System.Linq;
using System.Collections.Generic;

public class GameController
{
    private IPlayer[] players;
    private IDice dice;
    private Board board;
    private IDisplay _display;
    public IPlayer currentPlayer;
    public GameState state;
    public int currentPlayerIndex;

    public Func<Dice, int> OnDiceRoll;
    public Action<IPlayer> OnNextPlayerTurn;
    public delegate void HandleSixRollDelegate(IPlayer player, IPiece piece, int rollResult);
    public HandleSixRollDelegate OnSixRoll;

    public GameController(IPlayer[] players, IDice dice, Board board, IDisplay display)
    {
        this.players = players;
        this.dice = dice;  
        this.board = board;
        _display = display;
        state = GameState.NOT_STARTED;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
    }

    public void StartGame()
    {
        state = GameState.PLAYING;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
        _display.DisplayMessage("Game Started!");
        OnDiceRoll = (d) => d.Roll();
        OnNextPlayerTurn = (player) => _display.DisplayMessage($"\n🔄 It's now {player.Name}'s turn ({player.Color})!");
        OnSixRoll = (player, piece, rollResult) =>
        {
            _display.DisplayMessage($"🎉 {player.Name} rolled a 6!");
            if (piece.Status == PieceStatus.AT_HOME)
                _display.DisplayMessage("🏠 Bringing a piece out of home!");
        };

        while (true)
        {
            IPlayer currentPlayer = this.currentPlayer;
            _display.DisplayMessage($"\nIt's {currentPlayer.Name}'s turn ({currentPlayer.Color})!");
            bool triggerNext = false;

            bool continueRolling;
            do
            {
                continueRolling = false;
                _display.DisplayMessage("🎲 Press any key to roll the dice...");
                Display.InputKey(true);
                // int rollValue = RollDice();
                int rollValue = Convert.ToInt32(Console.ReadLine());
                _display.DisplayMessage($"🎲 {currentPlayer.Name} rolled a {rollValue}.");

                if (!CanPlayerMove(currentPlayer, rollValue))
                {
                    _display.DisplayMessage("❌ No available moves. Turn skipped.");
                    break;
                }

                _display.DisplayMessage("Your pieces:");
                for (int i = 0; i < currentPlayer.Pieces.Length; i++)
                {
                    var piece = currentPlayer.Pieces[i];
                    string status = GetPieceStatus(piece);
                    _display.DisplayMessage($"  [{i + 1}] Piece {i + 1}: {status}");
                }

                bool validMoveSelected = false;

                while (!validMoveSelected)
                {
                    int selectedIndex = _display.GetIntInput("Select a piece to move (1-4): ");
                    if (selectedIndex < 1 || selectedIndex > 4)
                    {
                        _display.DisplayMessage("❌ Invalid selection. Try again.");
                        continue;
                    }

                    int pieceIdx = selectedIndex - 1;
                    Piece? chosenPiece = SelectPiece(currentPlayer, pieceIdx, rollValue);

                    if (chosenPiece == null)
                    {
                        _display.DisplayMessage("❌ Invalid move! Choose a piece that can actually move.");
                        continue;
                    }

                    validMoveSelected = true;
                    MovePiece(chosenPiece, rollValue);
                }

                _display.DisplayBoard(board);

                if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                {
                    _display.DisplayMessage($"🎉 {currentPlayer.Name} has finished all pieces!");
                    NextPlayerTurn();
                    triggerNext = true;
                }

                if (state == GameState.FINISHED)
                {
                    _display.DisplayMessage("\n🎉 Game Over! Here are the final rankings :\n");
                    IPlayer[] ranking = GetWinner();

                    for (int i=0; i<ranking.Length; i++)
                    {
                        _display.DisplayMessage($"🏆 Rank {i + 1}: {ranking[i].Name} ({ranking[i].Color}) - Score: {ranking[i].Score}");
                    }

                    IPlayer lastPlayer = ranking[ranking.Length - 1];
                    _display.DisplayMessage($"💀 {lastPlayer.Name} ({lastPlayer.Color}) LOSES the game!");
                    Environment.Exit(0);
                }

                if (rollValue == 6)
                    continueRolling = true;

            } while (continueRolling);
            
            if (!triggerNext){
                NextPlayerTurn();
            }

        }

    }

    // play again
    public void EndGame()
    {
        state = GameState.FINISHED;
    }

    public void ResetGame()
    {

    }

    public Piece? SelectPiece(IPlayer player, int pieceIndex, int diceValue)
    {
        if (pieceIndex < 0 || pieceIndex >= player.Pieces.Length) 
            return null;

        Piece piece = player.Pieces[pieceIndex];
        if (!CanMovePiece(piece, diceValue)) 
            return null;
        
        return piece;
    }

    public string GetPieceStatus(IPiece piece)
    {
        if (piece.Status == PieceStatus.AT_HOME) return PieceStatus.AT_HOME.ToString();
        if (piece.Status == PieceStatus.AT_GOAL) return PieceStatus.AT_GOAL.ToString();
        return (piece.Position.Row, piece.Position.Column).ToString();
    }

    public bool CanPlayerMove(IPlayer player, int rollValue)
    {
        foreach (var piece in player.Pieces)
        {
            if (CanMovePiece(piece, rollValue))
                return true;
        }
        return false;
    }

    public IPlayer[] GetWinner()
    {
        foreach (var player in players)
        {
            player.AddScore(); 
        }
        IPlayer[] rankedPlayers = players.OrderByDescending(p => p.GetScore()).ToArray();

        return rankedPlayers;
    }

  
    public bool CanMovePiece(IPiece piece, int diceValue)
    {
        if (piece.Color != currentPlayer.Color) 
            return false;

        if (piece.Status == PieceStatus.AT_GOAL) 
            return false;

        if (piece.Status == PieceStatus.AT_HOME)
        {
            return diceValue == 6;
        }

        if (piece.Status == PieceStatus.IN_PLAY)
        {
            int newSteps = piece.Steps + diceValue;

            Path mainPath = board.PathManager.GetMainPath(piece.Color);
            Path goalPath = board.PathManager.GetGoalPath(piece.Color);

            int mainCount = mainPath.Count;   
            int goalCount = goalPath.Count;   
            int total = mainCount + goalCount; 

            if (newSteps > total - 1)
                return false;
            
            return true;
        }

        return false;
    }

 
    public void MovePiece(IPiece ipiece, int diceValue)
    {
        Piece piece = ipiece as Piece;
        if (piece == null) return;  
        board.KickPieceDelegate = KickPiece;

        if (piece.Status == PieceStatus.AT_HOME)
        {
            Path mainPath = board.PathManager.GetMainPath(piece.Color);
            Square startSquare = mainPath.GetSquare(0);

            board.HandleCollision(piece, startSquare);

            Square oldHomeSquare = board.GetSquare(piece.HomePosition.Row, piece.HomePosition.Column);
            board.UpdatePiecePosition(piece, oldHomeSquare, startSquare);

            piece.Position = startSquare.Pos;
            piece.Status = PieceStatus.IN_PLAY;
            piece.Steps = 0;
            return;
        }

        if (piece.Status == PieceStatus.IN_PLAY)
        {
            int newSteps = piece.Steps + diceValue;

            Path mainPath = board.PathManager.GetMainPath(piece.Color);
            Path goalPath = board.PathManager.GetGoalPath(piece.Color);

            int mainCount = mainPath.Count;
            int goalCount = goalPath.Count;
            int total = mainCount + goalCount;

            Square oldSquare = board.GetSquare(piece.Position.Row, piece.Position.Column);
            Square newSquare = null;

            if (newSteps < mainCount)
            {
                newSquare = mainPath.GetSquare(newSteps);
            }
            else
            {
                int over = newSteps - mainCount;
                newSquare = goalPath.GetSquare(over);
                if (over == goalCount - 1)
                {
                    piece.Status = PieceStatus.AT_GOAL;
                }
            }

            board.HandleCollision(piece, newSquare);

            board.UpdatePiecePosition(piece, oldSquare, newSquare);

            piece.Position = newSquare.Pos;
            piece.Steps = newSteps;
        }
    }


    public void KickPiece(Piece occupant)
    {
        if (occupant == null) return;

        Square oldSquare = board.GetSquare(occupant.Position.Row, occupant.Position.Column);
        Square homeSquare = board.GetSquare(occupant.HomePosition.Row, occupant.HomePosition.Column);
        board.UpdatePiecePosition(occupant, oldSquare, homeSquare);
        occupant.Position = homeSquare.Pos;
        occupant.Status = PieceStatus.AT_HOME;
        occupant.Steps = 0;
    }


    public int GetRemainingPlayers()
    {
        return players.Count(p => p.Pieces.Any(piece => piece.Status != PieceStatus.AT_GOAL));
    }


    public void HandleSixRoll(IPiece piece, int rollResult)
    {
        if (rollResult == 6)
            OnSixRoll?.Invoke(currentPlayer, piece, rollResult);
    }

    public void NextPlayerTurn()
    {
        if (GetRemainingPlayers() == 1)
        {
            EndGame();
            return;
        }
        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            currentPlayer = players[currentPlayerIndex];

            if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                continue;

            break;
        } 
        while (true);

        OnNextPlayerTurn?.Invoke(currentPlayer);
    }

    public int RollDice()
    {
        return OnDiceRoll != null ? OnDiceRoll((Dice)dice) : dice.Roll();
    }
}



