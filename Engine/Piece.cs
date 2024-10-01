using static Engine.PieceDefinitions;

namespace Engine;

public class Piece
{
    public ChessPieceColor PieceColor { get; set; }
    public ChessPieceType PieceType { get; set; }
    public bool Moved { get; set; }
    public Piece(ChessPieceType type, ChessPieceColor color)
    {
        PieceType = type;   
        PieceColor = color;
        Moved = false;  
    }
    public Piece()
    {
        PieceType = ChessPieceType.None;
        PieceColor = ChessPieceColor.White;
        Moved = false;
    }
}
