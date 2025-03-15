namespace LudoGame.Interfaces;
using LudoGame.Enums;
using LudoGame.Struct;
using LudoGame.Classes;
public interface IPiece
    {
        PieceColor Color { get; }
        Square Position { get; set; }  
        PieceStatus Status { get; set; }
        int Steps { get; set; }        
        string Marker { get; }         
        Square HomeSquare { get; }     
    }