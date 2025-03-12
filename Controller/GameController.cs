namespace LudoGame.Controller;
using LudoGame.Interfaces;
using LudoGame.Classes;
using LudoGame.Enums;
// public delegate void HandleSixRollDelegate(IPlayer player, IPiece piece, int rollResult);

public class GameController
    {
        private List<Player> players;
        private Dice dice;
        private Board board;
        public Player currentPlayer;
        public int currentPlayerIndex;

        // Delegates for dice roll, next turn, and handling a six.
        public Func<Dice, int> OnDiceRoll;
        public Action<Player> OnNextPlayerTurn;
        public delegate void HandleSixRollDelegate(Player player, IPiece piece, int rollResult);
        public HandleSixRollDelegate OnSixRoll;

        public GameController(List<Player> players, Dice dice, Board board)
        {
            this.players = players;
            this.dice = dice;
            this.board = board;
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

        // --- MODIFIED: MovePiece now handles collision via board.HandleCollision,
        //               and calls the new UpdatePiecePosition that uses piece objects. ---
        public bool MovePiece(IPiece ipiece, int diceValue)
        {
            Piece piece = ipiece as Piece;
            if (piece == null) return false;

            // Ensure the piece belongs to the current player.
            if (piece.Color != currentPlayer.Color) return false;

            // If the piece is at home:
            if (piece.Status == PieceStatus.AT_HOME)
            {
                if (diceValue == 6)
                {
                    // Move the piece to start of main path
                    var mainPath = board.PathManager.GetMainPath(piece.Color);
                    Square startSquare = mainPath.GetSquare(0);

                    // 1) Collision check
                    board.HandleCollision(piece, startSquare, this);

                    // 2) Update position
                    board.UpdatePiecePosition(piece, piece.HomeSquare, startSquare);

                    piece.Position = startSquare;
                    piece.Status = PieceStatus.IN_PLAY;
                    piece.Steps = 0;
                    return true;
                }
                else
                {
                    return false; // can't move out of home unless dice = 6
                }
            }
            // If the piece is already in play:
            if (piece.Status == PieceStatus.IN_PLAY)
            {
                int newSteps = piece.Steps + diceValue;
                var mainPath = board.PathManager.GetMainPath(piece.Color);
                var goalPath = board.PathManager.GetGoalPath(piece.Color);

                Square oldSquare = piece.Position;
                Square newSquare = null;

                // If still on main path
                if (newSteps < mainPath.Count)
                {
                    newSquare = mainPath.GetSquare(newSteps);
                }
                else
                {
                    // Transition to goal path
                    int over = newSteps - mainPath.Count;
                    if (over < goalPath.Count)
                    {
                        newSquare = goalPath.GetSquare(over);
                    }
                    else
                    {
                        // The piece has reached or exceeded the final goal position
                        newSquare = goalPath.GetSquare(goalPath.Count - 1);
                        piece.Status = PieceStatus.AT_GOAL;
                    }
                }

                // 1) Collision check
                board.HandleCollision(piece, newSquare, this);

                // 2) Update position on the board
                board.UpdatePiecePosition(piece, oldSquare, newSquare);

                piece.Position = newSquare;
                piece.Steps = newSteps;
                return true;
            }
            return false;
        }

        // --- NEW: KickPiece method to send occupant back home ---
        public void KickPiece(IPiece ipiece)
        {
            Piece occupant = ipiece as Piece;
            if (occupant == null) return;

            Square oldSquare = occupant.Position;
            Square homeSquare = occupant.HomeSquare;

            // Remove occupant from old square, place occupant back in home square
            board.UpdatePiecePosition(occupant, oldSquare, homeSquare);

            occupant.Position = homeSquare;
            occupant.Status = PieceStatus.AT_HOME;
            occupant.Steps = 0;
        }

        public void HandleCollision(Piece movingPiece, Square targetSquare, Board board)
        {
            if (targetSquare == null) return;

            if (piecePositions.TryGetValue(targetSquare, out Piece occupant))
            {
                // occupant is the piece currently on targetSquare
                bool isOccupantInSafeZone = safeCoords.Contains((occupant.Position.Row, occupant.Position.Col));
                if (!isOccupantInSafeZone && (occupant.Color != movingPiece.Color))
                {
                    // Kick occupant piece
                    KickPiece(occupant);
                    // occupant is now removed from occupantMap inside KickPiece
                }
                // If occupant is the same color, you could allow stacking or do nothing
                // For now, do nothing if same color
            }
        }

        public void HandleSixRoll(IPiece piece, int rollResult)
        {
            if (rollResult == 6)
                OnSixRoll?.Invoke(currentPlayer, piece, rollResult);
        }

        public void NextPlayerTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentPlayer = players[currentPlayerIndex];
            OnNextPlayerTurn?.Invoke(currentPlayer);
        }

        public int RollDice()
        {
            Console.WriteLine("🎲 Press any key to roll the dice...");
            Console.ReadKey(true);
            return OnDiceRoll != null ? OnDiceRoll(dice) : dice.Roll();
        }

    internal void KickPiece(Piece occupant)
    {
        throw new NotImplementedException();
    }
}
