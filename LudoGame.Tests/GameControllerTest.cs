using Xunit;
using LudoGame.Controller;
using LudoGame.Classes;
using LudoGame.Enums;
using Moq;
using System.Collections.Generic;

public class GameControllerTests
{
    private GameController gameController;
    private List<Player> players;
    private Dice dice;
    private Board board;

    public GameControllerTests()
    {
        // Arrange: Set up test data
        board = new Board();
        dice = new Dice();
        players = new List<Player>
        {
            new Player("Alice", PieceColor.RED, new Position[] { new Position(2, 2), new Position(2, 4), new Position(4, 2), new Position(4, 4) }),
            new Player("Bob", PieceColor.BLUE, new Position[] { new Position(2, 10), new Position(2, 12), new Position(4, 10), new Position(4, 12) })
        };
        gameController = new GameController(players, dice, board);
    }

    [Fact]
    public void GameController_Initializes_Correctly()
    {
        // Act: Check initial game state
        var currentPlayer = gameController.currentPlayer;

        // Assert: Ensure the first player is set correctly
        Assert.Equal("Alice", currentPlayer.Name);
        Assert.Equal(PieceColor.RED, currentPlayer.Color);
    }

    [Fact]
    public void CanMovePiece_WhenAtHomeAndRollIs6_ReturnsTrue()
    {
        // Arrange
        Piece testPiece = players[0].Pieces[0];

        // Act
        bool result = gameController.CanMovePiece(testPiece, 6);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanMovePiece_WhenAtHomeAndRollIsNot6_ReturnsFalse()
    {
        // Arrange
        Piece testPiece = players[0].Pieces[0];

        // Act
        bool result = gameController.CanMovePiece(testPiece, 5);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MovePiece_WhenValidMove_UpdatesPosition()
    {
        // Arrange
        Piece testPiece = players[0].Pieces[0];
        gameController.CanMovePiece(testPiece, 6);

        // Act
        gameController.MovePiece(testPiece, 6);

        // Assert
        Assert.Equal(PieceStatus.IN_PLAY, testPiece.Status);
    }

    [Fact]
    public void NextPlayerTurn_CorrectlySwitchesToNextPlayer()
    {
        // Act
        gameController.NextPlayerTurn();
        
        // Assert
        Assert.Equal("Bob", gameController.currentPlayer.Name);
    }

    [Fact]
    public void GetWinner_WhenAllPiecesAtGoal_ReturnsWinner()
    {
        // Arrange
        foreach (var piece in players[0].Pieces)
        {
            piece.Status = PieceStatus.AT_GOAL;
        }

        // Act
        var winner = gameController.GetWinner();

        // Assert
        Assert.NotNull(winner);
        Assert.Equal("Alice", winner.Name);
    }
}
