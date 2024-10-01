namespace Engine;

public struct Square
{
    public Piece Piece { get; set; }

    public Square()
    {
        Piece = new Piece();
    }

    // Copy contructor
    public Square(Piece piece)
    {
        Piece = piece != null
            ? new Piece(piece.PieceType, piece.PieceColor) { Moved = piece.Moved }
            : new Piece();  
    }

}
