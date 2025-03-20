namespace LudoGame.Controller;
using LudoGame.Interfaces;
using LudoGame.Classes;
using LudoGame.Enums;
using LudoGame.Struct;
using System.Linq;
using System.Collections.Generic;

public class GameController
{
    private IPlayer[] _players;
    private IDice _dice;
    private Board _board;
    private IDisplay _display;
    public IPlayer currentPlayer;
    public GameState state;
    public int currentPlayerIndex;

    public Func<Dice, int>? OnDiceRoll;
    public Action<IPlayer>? OnNextPlayerTurn;
    public delegate void HandleSixRollDelegate(IPlayer player, IPiece piece, int rollResult);
    public HandleSixRollDelegate? OnSixRoll;

    public GameController(IPlayer[] players, IDice dice, Board board, IDisplay display)
    {
        _players = players;
        _dice = dice;  
        _board = board;
        _display = display;
        state = GameState.NOT_STARTED;
        currentPlayerIndex = 0;
        currentPlayer = _players[currentPlayerIndex];
    }

    public void StartGame()
    {
        state = GameState.PLAYING;
        currentPlayerIndex = 0;
        currentPlayer = _players[currentPlayerIndex];
        _display.DisplayMessage("Game Started!");
        OnDiceRoll = (d) => d.Roll();
        OnNextPlayerTurn = (player) => _display.DisplayMessage($"\n🔄 It's now {player.Name}'s turn ({player.Color})!");
        OnSixRoll = (player, piece, rollResult) =>
        {
            _display.DisplayMessage($"🎉 {player.Name} rolled a 6!");
            if (piece.Status == PieceStatus.AT_HOME)
                _display.DisplayMessage("🏠 Bringing a piece out of home!");
        };

        ExecuteTurn();
    }

    public void EndGame()
    {
        state = GameState.FINISHED;
        _display.DisplayMessage("\n🎉 Game Over! Here are the final rankings :\n");
        IPlayer[] ranking = GetWinner();

        for (int i=0; i<ranking.Length; i++)
        {
            _display.DisplayMessage($"🏆 Rank {i + 1}: {ranking[i].Name} ({ranking[i].Color}) - Score: {ranking[i].Score}");
        }

        IPlayer lastPlayer = ranking[ranking.Length - 1];
        _display.DisplayMessage($"💀 {lastPlayer.Name} ({lastPlayer.Color}) LOSES the game!");
        string input = _display.GetInput("Do you want to play again? (Y/N)");
        if (!string.IsNullOrEmpty(input) && input.ToUpper() == "Y")
        {
            // reset game
            _board = new Board();
            foreach (var player in _players)
            {
                for (int i=0; i<player.Pieces.Length; i++)
                {
                    player.Pieces[i].UpdatePieceStatus(PieceStatus.AT_HOME);
                    player.Pieces[i].Steps = 0;
                    player.Pieces[i].Position = player.Pieces[i].HomePosition;
                    _board.RegisterPieceAtHome(player.Pieces[i]);
                }
            }
            currentPlayerIndex = 0;
            currentPlayer = _players[currentPlayerIndex];
            state = GameState.PLAYING;
            _display.DisplayBoard(_board);
            _display.DisplayMessage("New Game!!");
            StartGame();
        }
        else 
        {
            _display.DisplayMessage("Thanks for playing!");
            Environment.Exit(0);
        }
    }

    public void ExecuteTurn()
    {
        while (state == GameState.PLAYING)
        {
            IPlayer currentPlayer = this.currentPlayer;
            _display.DisplayMessage($"\nIt's {currentPlayer.Name}'s turn ({currentPlayer.Color})!");
            bool triggerNext = false;

            bool continueRolling;
            do
            {
                continueRolling = false;
                _display.DisplayMessage("🎲 Press any key to roll the _dice...");
                Display.InputKey(true);
                int rollValue = RollDice();
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
                    string status = piece.GetPieceStatus();
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

                _display.DisplayBoard(_board);

                if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                {
                    _display.DisplayMessage($"🎉 {currentPlayer.Name} has finished all pieces!");
                    NextPlayerTurn();
                    triggerNext = true;
                }

                if (state == GameState.FINISHED)
                {
                    break;
                }

                if (rollValue == 6)
                    continueRolling = true;

            } while (continueRolling);
            
            if (!triggerNext && state == GameState.PLAYING){
                NextPlayerTurn();
            }

        }
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
        foreach (var player in _players)
        {
            player.AddScore(); 
        }
        IPlayer[] rankedPlayers = _players.OrderByDescending(p => p.GetScore()).ToArray();

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

            Path mainPath = _board.PathManager.GetMainPath(piece.Color);
            Path goalPath = _board.PathManager.GetGoalPath(piece.Color);

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
        Piece? piece = ipiece as Piece;
        if (piece == null) return;  
        _board.KickPieceDelegate = KickPiece;

        if (piece.Status == PieceStatus.AT_HOME)
        {
            Path? mainPath = _board.PathManager.GetMainPath(piece.Color);
            Square? startSquare = mainPath.GetSquare(0);

            _board.HandleCollision(piece, startSquare);

            Square? oldHomeSquare = _board.GetSquare(piece.HomePosition.Row, piece.HomePosition.Column);
            _board.UpdatePiecePosition(piece, oldHomeSquare, startSquare);

            piece.Position = startSquare.Pos;
            piece.UpdatePieceStatus(PieceStatus.IN_PLAY);
            piece.Steps = 0;
            return;
        }

        if (piece.Status == PieceStatus.IN_PLAY)
        {
            int newSteps = piece.Steps + diceValue;

            Path? mainPath = _board.PathManager.GetMainPath(piece.Color);
            Path? goalPath = _board.PathManager.GetGoalPath(piece.Color);

            int mainCount = mainPath.Count;
            int goalCount = goalPath.Count;
            int total = mainCount + goalCount;

            Square? oldSquare = _board.GetSquare(piece.Position.Row, piece.Position.Column);
            Square? newSquare = null;

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
                    piece.UpdatePieceStatus(PieceStatus.AT_GOAL);
                }
            }

            _board.HandleCollision(piece, newSquare);

            _board.UpdatePiecePosition(piece, oldSquare, newSquare);

            piece.Position = newSquare.Pos;
            piece.Steps = newSteps;
        }
    }


    public void KickPiece(Piece occupant)
    {
        if (occupant == null) return;

        Square oldSquare = _board.GetSquare(occupant.Position.Row, occupant.Position.Column);
        Square homeSquare = _board.GetSquare(occupant.HomePosition.Row, occupant.HomePosition.Column);
        _board.UpdatePiecePosition(occupant, oldSquare, homeSquare);
        occupant.Position = homeSquare.Pos;
        occupant.Status = PieceStatus.AT_HOME;
        occupant.Steps = 0;
    }


    public int GetRemainingPlayers()
    {
        return _players.Count(p => p.Pieces.Any(piece => piece.Status != PieceStatus.AT_GOAL));
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
            currentPlayerIndex = (currentPlayerIndex + 1) % _players.Length;
            currentPlayer = _players[currentPlayerIndex];

            if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                continue;

            break;
        } 
        while (true);

        OnNextPlayerTurn?.Invoke(currentPlayer);
    }

    public int RollDice()
    {
        return OnDiceRoll != null ? OnDiceRoll((Dice)_dice) : _dice.Roll();
    }
}



