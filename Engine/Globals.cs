using static Engine.PieceDefinitions;

namespace Engine;

public sealed class Globals
{
    public Square[] Squares { get; private set; }
    public ChessPieceColor Turn { get; set; }
    public int EnPassantSquare { get; set; } = -1;

    // Castling Rights 
    public bool WhiteKingSideCastling { get; set; } = false;
    public bool WhiteQueenSideCastling { get; set; } = false;
    public bool BlackKingSideCastling { get; set; } = false;
    public bool BlackQueenSideCastling { get; set; } = false;


    // Castling conditions 
    public bool HasWhiteKingMoved { get; set; } = false;
    public bool HasWhiteRookKingsideMoved { get; set; } = false;
    public bool HasWhiteRookQueenSideMoved { get; set; } = false;

    public bool HasBlackKingMoved { get; set; } = false;
    public bool HasBlackRookKingsideMoved { get; set; } = false;
    public bool HasBlackRookQueenSideMoved { get; set; } = false;



    // Default constructor
    public Globals()
    {
        Squares = new Square[64];
        for (int i = 0; i < 64; i++)
            Squares[i] = new Square();

        Turn = ChessPieceColor.None;
    }

    public void InitializeBoard(string fen = "")
    {
        if (string.IsNullOrEmpty(fen))
        {
            LoadPositionFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }
        else
        {
            LoadPositionFromFEN(fen);
        }
    }


    public bool CanCastleKingSide(ChessPieceColor color)
    {
        int kingIndex = (color == ChessPieceColor.White) ? 60 : 4;
        int rookIndex = (color == ChessPieceColor.White) ? 63 : 7;

        // Check if rook or king has moved using correct flags
        if (color == ChessPieceColor.White && (HasWhiteKingMoved || HasWhiteRookKingsideMoved))
            return false;
        if (color == ChessPieceColor.Black && (HasBlackKingMoved || HasBlackRookKingsideMoved))
            return false;

        // Check if squares between king and rook are empty (f1 and g1 for White)
        for (int i = kingIndex + 1; i < rookIndex; i++)
        {
            if (Squares[i].Piece.PieceType != ChessPieceType.None)
                return false;
        }

        // Check if king or squares it moves through are under attack (skip for now)
        // Implement this check later

        // Initial conditions for king-side castling are met
        return true;
    }

    public bool CanCastleQueenSide(ChessPieceColor color)
    {
        int kingIndex = (color == ChessPieceColor.White) ? 60 : 4;
        int rookIndex = (color == ChessPieceColor.White) ? 56 : 0;

        // Check if the King or Queen-side Rook has moved using separate flags
        if (color == ChessPieceColor.White && (HasWhiteKingMoved || HasWhiteRookQueenSideMoved))
            return false;
        if (color == ChessPieceColor.Black && (HasBlackKingMoved || HasBlackRookQueenSideMoved))
            return false;

        // Check if squares between king and rook are empty (b1, c1, and d1 for White)
        for (int i = rookIndex + 1; i < kingIndex; i++)
        {
            if (Squares[i].Piece.PieceType != ChessPieceType.None)
                return false;
        }

        // Check if king or squares it moves through are under attack (skip for now)
        // Implement this check later

        // Initial conditions for queen-side castling are met
        return true;
    }

    public void MovePiece(int fromIndex, int toIndex)
    {
        if (!Helpers.IsValidIndex(fromIndex) || !Helpers.IsValidIndex(toIndex))
        {
            Console.WriteLine("Invalid board coordinates.");
            return;
        }

        Piece movingPiece = Squares[fromIndex].Piece;
        if (movingPiece.PieceType == ChessPieceType.None)
        {
            Console.WriteLine("Square is empty!");
            return;
        }

        // Check for castling moves
        if (movingPiece.PieceType == ChessPieceType.King)
        {
            // King-Side Castling
            if ((fromIndex == 60 || fromIndex == 4) && toIndex == fromIndex + 2)
            {
                if (CanCastleKingSide(movingPiece.PieceColor))
                {
                    ExecuteCastling(movingPiece.PieceColor, true);
                    // Console.WriteLine($"Castling King-Side completed. Next move: {Turn}");
                    goto FlipTurn;  // Jump to turn flip
                }
            }
            // Queen-Side Castling
            if ((fromIndex == 60 || fromIndex == 4) && toIndex == fromIndex - 2)
            {
                if (CanCastleQueenSide(movingPiece.PieceColor))
                {
                    ExecuteCastling(movingPiece.PieceColor, false);
                    // Console.WriteLine($"Castling Queen-Side completed. Next move: {Turn}");
                    goto FlipTurn;  // Jump to turn flip
                }
            }
        }

        if (IsLegalMove(fromIndex, toIndex))
        {
            // Move the piece
            Squares[toIndex] = new Square(movingPiece);
            Squares[fromIndex] = new Square();

            // Handle pawn promotions
            if (movingPiece.PieceType == ChessPieceType.Pawn && (toIndex < 8 || toIndex >= 56))
                HandlePawnPromotion(toIndex);

            // Handle En Passant
            UpdateEnPassant(movingPiece, fromIndex, toIndex);
        }
        else
        {
            // Console.WriteLine("Illegal move");
            return;  // Exit without flipping the turn if illegal
        }

    FlipTurn:
        // Switch the turn once before the method exits
        Turn = (ChessPieceColor)((int)Turn ^ 1);
        // Console.WriteLine($"Turn switched. Next move: {Turn}");
    }



    public void ExecuteCastling(ChessPieceColor color, bool isKingSide)
    {
        // Determine starting positions based on color
        int kingStart = (color == ChessPieceColor.White) ? 60 : 4;
        int rookStart = isKingSide ? ((color == ChessPieceColor.White) ? 63 : 7) : ((color == ChessPieceColor.White) ? 56 : 0);

        // Determine the destination squares for King and Rook based on side
        int kingEnd = kingStart + (isKingSide ? 2 : -2);
        int rookEnd = kingEnd + (isKingSide ? -1 : 1);

        // Move the King
        Squares[kingEnd] = new Square(Squares[kingStart].Piece);
        Squares[kingStart] = new Square();

        // Move the Rook
        Squares[rookEnd] = new Square(Squares[rookStart].Piece);
        Squares[rookStart] = new Square();

        Console.WriteLine($"Castling {(color == ChessPieceColor.White ? "White" : "Black")} {(isKingSide ? "King" : "Queen")} side completed.");
    }


    private void HandlePawnPromotion(int toIndex)
    {
        Console.WriteLine("Pawn reached the last rank! Choose promotion piece (Q, R, B, N): ");
        string? promotion = Console.ReadLine()?.ToUpper();
        Squares[toIndex].Piece.PieceType = promotion switch
        {
            "Q" => ChessPieceType.Queen,
            "R" => ChessPieceType.Rook,
            "B" => ChessPieceType.Bishop,
            "N" => ChessPieceType.Knight,
            _ => ChessPieceType.Queen
        };
    }

    private void UpdateEnPassant(Piece movingPiece, int fromIndex, int toIndex)
    {
        EnPassantSquare = -1;
        if (movingPiece.PieceType == ChessPieceType.Pawn && Math.Abs(toIndex - fromIndex) == 16)
            EnPassantSquare = (fromIndex + toIndex) / 2;
    }

    public bool IsLegalMove(int fromIndex, int toIndex)
    {
        Piece piece = Squares[fromIndex].Piece;
        // Console.WriteLine($"Checking move from {fromIndex} to {toIndex} for {piece.PieceColor} {piece.PieceType}, Turn: {Turn}");
        if (piece.PieceType == ChessPieceType.None || piece.PieceColor != Turn)
            return false;

        return piece.PieceType switch
        {
            ChessPieceType.Pawn => PawnMove(fromIndex, toIndex, piece),
            ChessPieceType.King => KingMove(fromIndex, toIndex, piece),
            ChessPieceType.Rook => RookMove(fromIndex, toIndex, piece),
            _ => false
        };
    }

    private bool PawnMove(int fromIndex, int toIndex, Piece piece)
    {
        int direction = (piece.PieceColor == ChessPieceColor.White) ? -8 : 8;
        int startRank = (piece.PieceColor == ChessPieceColor.White) ? 1 : 6;
        int fromRank = 7 - (fromIndex / 8);

        if (toIndex == fromIndex + direction && Squares[toIndex].Piece.PieceType == ChessPieceType.None)
            return true;

        if (fromRank == startRank && toIndex == fromIndex + 2 * direction &&
            Squares[toIndex].Piece.PieceType == ChessPieceType.None &&
            Squares[fromIndex + direction].Piece.PieceType == ChessPieceType.None)
            return true;

        int captureLeft = fromIndex + direction - 1;
        int captureRight = fromIndex + direction + 1;
        bool validLeft = Helpers.IsValidIndex(captureLeft) && (fromIndex % 8 > 0);
        bool validRight = Helpers.IsValidIndex(captureRight) && (fromIndex % 8 < 7);

        if ((toIndex == captureLeft && validLeft || toIndex == captureRight && validRight) &&
            Squares[toIndex].Piece.PieceType != ChessPieceType.None &&
            Squares[toIndex].Piece.PieceColor != piece.PieceColor)
            return true;

        return (toIndex == captureLeft || toIndex == captureRight) && toIndex == EnPassantSquare;
    }

    private bool KingMove(int fromIndex, int toIndex, Piece piece)
    {
        int rowDiff = Math.Abs(fromIndex / 8 - toIndex / 8);
        int colDiff = Math.Abs(fromIndex % 8 - toIndex % 8);

        bool isDestinationEmpty = Squares[toIndex].Piece.PieceType == ChessPieceType.None;
        bool isOpponentPiece = Squares[toIndex].Piece.PieceColor != piece.PieceColor;

        return (rowDiff <= 1 && colDiff <= 1) && (isDestinationEmpty || isOpponentPiece);
    }

    private bool RookMove(int fromIndex, int toIndex, Piece piece)
    {
        int fromRow = fromIndex / 8;
        int fromCol = fromIndex % 8;
        int toRow = toIndex / 8;
        int toCol = toIndex % 8;

        // Ensure the move is strictly vertical or horizontal
        if (fromRow != toRow && fromCol != toCol)
        {
            Console.WriteLine("Illegal move: Rook can only move in straight lines.");
            return false;
        }

        // Determine the direction of movement
        int rowStep = (toRow == fromRow) ? 0 : (toRow > fromRow ? 1 : -1);
        int colStep = (toCol == fromCol) ? 0 : (toCol > fromCol ? 1 : -1);

        // Console.WriteLine($"Rook move - From: ({fromRow}, {fromCol}) To: ({toRow}, {toCol}), Steps: ({rowStep}, {colStep})");

        // Traverse the path, but stop before reaching the destination square
        int currentRow = fromRow + rowStep;
        int currentCol = fromCol + colStep;

        while (currentRow != toRow || currentCol != toCol)
        {
            int index = currentRow * 8 + currentCol;
            // Console.WriteLine($"Checking square at ({currentRow}, {currentCol}) Index: {index}, Piece: {Squares[index].Piece.PieceType}");

            // Check if any square in between is occupied
            if (Squares[index].Piece.PieceType != ChessPieceType.None)
            {
                // Console.WriteLine("Illegal move: Rook is blocked by another piece.");
                return false;
            }

            // Move to the next square in the path
            currentRow += rowStep;
            currentCol += colStep;
        }

        // Finally, check the destination square more accurately
        Piece destinationPiece = Squares[toIndex].Piece;

        // Check if the destination is empty or contains an opponent's piece
        if (destinationPiece.PieceType != ChessPieceType.None && destinationPiece.PieceColor == piece.PieceColor)
        {
            // Console.WriteLine("Illegal move: Rook cannot capture its own piece.");
            return false;
        }

        Console.WriteLine("Legal Rook move.");
        return true;
    }



    public void LoadPositionFromFEN(string fen)
    {
        for (int i = 0; i < 64; i++)
            Squares[i] = new Square();

        if (string.IsNullOrEmpty(fen))
        {
            InitializeBoard();
            return;
        }

        string[] sections = fen.Split(' ');
        string piecePlacement = sections[0];
        int squareIndex = 0;

        for (int i = 0; i < piecePlacement.Length; i++)
        {
            char c = piecePlacement[i];

            if (c == '/')
                continue;

            if (char.IsDigit(c))
            {
                squareIndex += (c - '0'); // Skip empty squares
            }
            else
            {
                ChessPieceColor color = char.IsUpper(c) ? ChessPieceColor.White : ChessPieceColor.Black;
                ChessPieceType type = c.ToString().ToLower() switch
                {
                    "p" => ChessPieceType.Pawn,
                    "r" => ChessPieceType.Rook,
                    "n" => ChessPieceType.Knight,
                    "b" => ChessPieceType.Bishop,
                    "q" => ChessPieceType.Queen,
                    "k" => ChessPieceType.King,
                    _ => ChessPieceType.None
                };

                Squares[squareIndex] = new Square(new Piece(type, color));
                squareIndex++;
            }
        }

        // Set the turn correctly based on FEN
        Turn = (sections[1] == "w") ? ChessPieceColor.White : ChessPieceColor.Black;
        //Console.WriteLine($"Turn set to: {Turn}");

        // Reset all castling rights
        WhiteKingSideCastling = WhiteQueenSideCastling = BlackKingSideCastling = BlackQueenSideCastling = false;
        string castlingRights = sections[2];
        if (castlingRights.Contains('K')) WhiteKingSideCastling = true;
        if (castlingRights.Contains('Q')) WhiteQueenSideCastling = true;
        if (castlingRights.Contains('k')) BlackKingSideCastling = true;
        if (castlingRights.Contains('q')) BlackQueenSideCastling = true;

        // En Passant square handling
        EnPassantSquare = -1; // Reset EnPassant square after loading
        if (sections[3] != "-")
        {
            EnPassantSquare = Helpers.SquareNameToIndex(sections[3]);
            // Console.WriteLine($"En Passant Square set to: {EnPassantSquare}");
        }

        // Additional handling of half-moves and full-moves can be added here if needed.
    }


    // Temporary method to display the board
    public void PrintBoard()
    {
        for (int rank = 0; rank < 8; rank++) // Start from rank 0 (top) and go down
        {
            for (int file = 0; file < 8; file++)
            {
                int index = rank * 8 + file; // Standard row-wise traversal
                Piece piece = Squares[index].Piece;

                if (piece.PieceType == ChessPieceType.None)
                    Console.Write(". ");
                else
                {
                    char symbol = piece.PieceType switch
                    {
                        ChessPieceType.Pawn => 'P',
                        ChessPieceType.Knight => 'N',
                        ChessPieceType.Bishop => 'B',
                        ChessPieceType.Rook => 'R',
                        ChessPieceType.Queen => 'Q',
                        ChessPieceType.King => 'K',
                        _ => '?'
                    };
                    Console.Write(piece.PieceColor == ChessPieceColor.White ? $"{symbol} " : $"{char.ToLower(symbol)} ");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public Globals Clone()
    {
        var clone = new Globals
        {
            Turn = this.Turn,
            EnPassantSquare = this.EnPassantSquare,
            WhiteKingSideCastling = this.WhiteKingSideCastling,
            WhiteQueenSideCastling = this.WhiteQueenSideCastling,
            BlackKingSideCastling = this.BlackKingSideCastling,
            BlackQueenSideCastling = this.BlackQueenSideCastling,
            HasWhiteKingMoved = this.HasWhiteKingMoved,
            HasWhiteRookKingsideMoved = this.HasWhiteRookKingsideMoved,
            HasWhiteRookQueenSideMoved = this.HasWhiteRookQueenSideMoved,
            HasBlackKingMoved = this.HasBlackKingMoved,
            HasBlackRookKingsideMoved = this.HasBlackRookKingsideMoved,
            HasBlackRookQueenSideMoved = this.HasBlackRookQueenSideMoved,
        };

        // Copy the board squares
        for (int i = 0; i < 64; i++)
            clone.Squares[i] = new Square(this.Squares[i].Piece);

        return clone;
    }


}

/*           Board layout      
 
        00 01 02 03 04 05 06 07
        08 09 10 11 12 13 14 15
        16 17 18 19 20 21 22 23
        24 25 26 27 28 29 30 31
        32 33 34 35 36 37 38 39
        40 41 42 43 44 45 46 47
        48 49 50 51 52 53 54 55
        56 57 58 59 60 61 62 63

 */


