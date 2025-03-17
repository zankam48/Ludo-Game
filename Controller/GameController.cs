namespace LudoGame.Controller;
using LudoGame.Interfaces;
using LudoGame.Classes;
using LudoGame.Enums;

public class GameController
    {
        private List<Player> players;
        private Dice dice;
        private Board board;
        public Player currentPlayer;
        public GameState state;
        public int currentPlayerIndex;

        public Func<Dice, int> OnDiceRoll;
        public Action<Player> OnNextPlayerTurn;
        public delegate void HandleSixRollDelegate(Player player, IPiece piece, int rollResult);
        public HandleSixRollDelegate OnSixRoll;

        public GameController(List<Player> players, Dice dice, Board board)
        {
            this.players = players;
            this.dice = dice;
            this.board = board;
            this.state = GameState.NOT_STARTED;
            currentPlayerIndex = 0;
            currentPlayer = players[currentPlayerIndex];
        }

        public string GetPieceStatus(IPiece piece)
        {
            return piece.Status == PieceStatus.AT_HOME ? "At Home" :
                   piece.Status == PieceStatus.AT_GOAL ? "At Goal" :
                   $"At ({piece.Position.Row},{piece.Position.Col})";
        }

        public bool CanPlayerMove(Player player, int rollValue)
        {
            foreach (var piece in player.Pieces)
            {
                if (piece.Status == PieceStatus.IN_PLAY) return true;
                if (piece.Status == PieceStatus.AT_HOME && rollValue == 6) return true;
            }
            return false;
        }

        public bool HasPieceInPlay(Player player)
        {
            foreach (var piece in player.Pieces)
            {
                if (piece.Status == PieceStatus.IN_PLAY) return true;
            }
            return false;
        }

        // public bool CanPieceMove(Piece piece, int diceValue)
        // {
        //     var goalPath = board.PathManager.GetGoalPath(piece.Color);
        //     Square goalSquare = goalPath[last index] // goal square
        //     - check if diceValue > distance(piecepos, targetSquare) false can't move the piece, move another piece if there aren't any then 
        //     - check if diceValue > distance(piecepos, targetSquare) true can move the piece
        //     - check if diceValue = distance(piecepos, targetSquare) true can move the piece and the piece about to goal
        // }

        public bool MovePiece(IPiece ipiece, int diceValue)
        {
            Piece piece = ipiece as Piece;
            if (piece == null) return false;

            if (piece.Color != currentPlayer.Color) return false;

            if (piece.Status == PieceStatus.AT_HOME)
            {
                if (diceValue == 6)
                {
                    var mainPath = board.PathManager.GetMainPath(piece.Color);
                    Square startSquare = mainPath.GetSquare(0);

                    HandleCollision(piece, startSquare, board);

                    board.UpdatePiecePosition(piece, piece.HomeSquare, startSquare);

                    piece.Position = startSquare;
                    piece.Status = PieceStatus.IN_PLAY;
                    piece.Steps = 0;
                    return true;
                }
                else
                {
                    return false; 
                }
            }
            if (piece.Status == PieceStatus.IN_PLAY)
            {
                int newSteps = piece.Steps + diceValue;
                var mainPath = board.PathManager.GetMainPath(piece.Color);
                var goalPath = board.PathManager.GetGoalPath(piece.Color);

                Square oldSquare = piece.Position;
                Square newSquare = null;

                if (newSteps < mainPath.Count)
                {
                    newSquare = mainPath.GetSquare(newSteps);
                }
                else
                {
                    int over = newSteps - mainPath.Count;
                    if (over < goalPath.Count)
                    {
                        newSquare = goalPath.GetSquare(over);
                    }
                    else
                    {
                        newSquare = goalPath.GetSquare(goalPath.Count - 1);
                        piece.Status = PieceStatus.AT_GOAL;
                    }
                }

                HandleCollision(piece, newSquare, board);

                board.UpdatePiecePosition(piece, oldSquare, newSquare);

                piece.Position = newSquare;
                piece.Steps = newSteps;
                return true;
            }
            return false;
        }

        public void KickPiece(Piece occupant)
        {
            if (occupant == null) return;
            else {
                Square oldSquare = occupant.Position;
                Square homeSquare = occupant.HomeSquare;

                board.UpdatePiecePosition(occupant, oldSquare, homeSquare);

                occupant.Position = homeSquare;
                occupant.Status = PieceStatus.AT_HOME;
                occupant.Steps = 0;
            }

            
        }

        public void HandleCollision(Piece movingPiece, Square targetSquare, Board board)
        {
            if (targetSquare == null) return;
            
            if (board.safeCoords.Contains((targetSquare.Row, targetSquare.Col)))
                return;
            
            List<Piece> piecesOnSquare = board.GetPiecesOnSquare(targetSquare);
            
            foreach (var occupant in piecesOnSquare.Where(p => p.Color != movingPiece.Color).ToList())
            {
                KickPiece(occupant);
            }
        }


        public int GetRemainingPlayers()
        {
            return players.Count(p => p.Pieces.Any(piece => piece.Status != PieceStatus.AT_GOAL));
        }


        public Player? GetWinner()
        {
            foreach (var player in players)
            {
                if (player.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                {
                    return player;
                }
            }
            return null;
        }

        public void HandleSixRoll(IPiece piece, int rollResult)
        {
            if (rollResult == 6)
                OnSixRoll?.Invoke(currentPlayer, piece, rollResult);
        }

        // public void NextPlayerTurn()
        // {
        //     currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        //     currentPlayer = players[currentPlayerIndex];
        //     OnNextPlayerTurn?.Invoke(currentPlayer);
        // }

        public void NextPlayerTurn()
        {
            do
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                currentPlayer = players[currentPlayerIndex];

                if (currentPlayer.Pieces.All(p => p.Status == PieceStatus.AT_GOAL))
                {
                    continue; // skip to the next player
                }

                if (GetRemainingPlayers() == 1)
                {
                    this.state = GameState.FINISHED; 
                }

                break; 

            } while (true);

            OnNextPlayerTurn?.Invoke(currentPlayer);
        }


        public int RollDice()
        {
            return OnDiceRoll != null ? OnDiceRoll(dice) : dice.Roll();
        }

}