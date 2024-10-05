using static Engine.PieceDefinitions;

namespace Engine;
public struct Square
{
    public Piece Piece { get; set; }

    // Default constructor, initializing to an empty square
    public Square()
    {
        Piece = new Piece(ChessPieceType.None, ChessPieceColor.None);
    }

    // Copy constructor
    public Square(Piece piece)
    {
        Piece = new Piece(piece.PieceType, piece.PieceColor) { Moved = piece.Moved };
    }
}
