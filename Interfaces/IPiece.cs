namespace LudoGame.Interfaces;
using LudoGame.Enums;
using LudoGame.Struct;
using LudoGame.Classes;
public interface IPiece
    {
        PieceColor Color { get; }
        Square Position { get; set; }  // current board square
        PieceStatus Status { get; set; }
        int Steps { get; set; }        // steps taken along the path
        string Marker { get; }         // ANSI colored marker (e.g. "1" in color)
        Square HomeSquare { get; }     // starting (home) square
    }