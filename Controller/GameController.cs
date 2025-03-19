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
    public IPlayer currentPlayer;
    public GameState state;
    public int currentPlayerIndex;

    public Func<Dice, int> OnDiceRoll;
    public Action<IPlayer> OnNextPlayerTurn;
    public delegate void HandleSixRollDelegate(IPlayer player, IPiece piece, int rollResult);
    public HandleSixRollDelegate OnSixRoll;

    public GameController(IPlayer[] players, IDice dice, Board board)
    {
        this.players = players;
        this.dice = dice;  
        this.board = board;
        state = GameState.NOT_STARTED;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
        // pake interface idice iboard
        // pake array instead of list, kl gk inumerable
    }

    public void StartGame()
    {
        state = GameState.PLAYING;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
        OnNextPlayerTurn?.Invoke(currentPlayer);

    }

    // play again
    public void EndGame()
    {
        state = GameState.FINISHED;
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


    // jgn pake hardcode string, return enum pake to string
    // enum pake attribute (ada spasinya)
    // bs juga extensions method
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


        if (piece.Status == PieceStatus.AT_HOME)
        {
            Path mainPath = board.PathManager.GetMainPath(piece.Color);
            Square startSquare = mainPath.GetSquare(0);

            HandleCollision(piece, startSquare, board);

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

            HandleCollision(piece, newSquare, board);

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


    public void HandleCollision(Piece movingPiece, Square targetSquare, Board board)
    {
        if (targetSquare == null) return;

        if (board.safeCoords.Contains(targetSquare.Pos))
            return;

        if (board.piecePositions.TryGetValue(targetSquare.Pos, out List<Piece> piecesOnSquare))
        {
            foreach (var occupant in piecesOnSquare.Where(p => p.Color != movingPiece.Color).ToList())
            {
                KickPiece(occupant);
            }
        }
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



