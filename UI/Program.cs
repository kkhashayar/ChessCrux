using Engine;

Globals board = new Globals();

board.InitializeBoard("k3r3/8/8/R3p3/4P2r/8/8/K3R3 w - - 0 1");

bool running = true;
while (running)
{
    board.PrintBoard(); // Print board before each move
    GetUserMove();
}

void GetUserMove()
{
    Console.WriteLine("Enter your move (e.g., a2 a4): ");
    string? move = Console.ReadLine();

    if (string.IsNullOrEmpty(move) || move.Length < 5)
    {
        Console.WriteLine("Invalid input. Use format like 'a2 a4'.");
        return;
    }

    string startSquare = move.Substring(0, 2);
    string endSquare = move.Substring(3, 2);

    Console.WriteLine($"Parsed input - Start Square: {startSquare}, End Square: {endSquare}");

    int startIndex = SquareNameToIndex(startSquare);
    int endIndex = SquareNameToIndex(endSquare);

    Console.WriteLine($"Converted - Start Index: {startIndex}, End Index: {endIndex}");

    if (startIndex == -1 || endIndex == -1)
    {
        Console.WriteLine("Invalid square input. Use coordinates like 'a2' or 'e4'.");
        return;
    }

    // Debug the state before making the move
    Console.WriteLine($"Attempting to move piece from {startSquare} (Index: {startIndex}) to {endSquare} (Index: {endIndex})");
    Console.WriteLine($"Piece at start square: {board.Squares[startIndex].Piece.PieceType} ({board.Squares[startIndex].Piece.PieceColor})");

    board.MovePiece(startIndex, endIndex);
}


// Helper function to convert square name (e.g., "a2") to board index
int SquareNameToIndex(string squareName)
{
    if (squareName.Length != 2)
    {
        Console.WriteLine($"Invalid square name: {squareName}");
        return -1;
    }

    char file = squareName[0]; // 'a' to 'h'
    char rank = squareName[1]; // '1' to '8'

    int fileIndex = file - 'a'; // Convert 'a' to 0, 'b' to 1, ..., 'h' to 7
    int rankIndex = rank - '1'; // Convert '1' to 0, '2' to 1, ..., '8' to 7

    Console.WriteLine($"Square: {squareName}, File: {fileIndex}, Rank: {rankIndex}");

    if (fileIndex < 0 || fileIndex > 7 || rankIndex < 0 || rankIndex > 7)
    {
        Console.WriteLine($"Invalid square input: {squareName}");
        return -1;
    }

    // Flip the rank to match the internal board layout
    int boardIndex = (7 - rankIndex) * 8 + fileIndex;
    Console.WriteLine($"Mapped Square {squareName} to Index {boardIndex}");

    return boardIndex;
}




