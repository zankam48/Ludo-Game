namespace LudoGame.Interfaces;
using LudoGame.Enums;
using LudoGame.Struct;
public interface IPiece
    {
        PieceColor Color { get; }
        Position Position { get; set; }  
        PieceStatus Status { get; set; }
        int Steps { get; set; }        
        string Marker { get; }         
        public Position HomePosition { get; set; }   
    }