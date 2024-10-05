using Engine;
using System.Transactions;

Globals board = new Globals();

string? fen = "8/8/7k/8/7K/8/8/8 w - - 0 1";
int maxDepth = 1;

board.InitializeBoard(fen);

TestPerft(board, maxDepth);

void TestPerft(Globals board, int maxDepth)
{
    MoveGeneration moveGen = new MoveGeneration(board);
    Console.WriteLine($"Testing position: {fen} ");
    for (int depth = 1; depth <= maxDepth ; depth ++ )
    {
        long nodes = moveGen.Perft(depth); 
        Console.WriteLine($"Perft({depth}): {nodes}");
    }
}


//bool running = true;
//bool running = true;
//while (running)
//{
//    board.PrintBoard(); // Print board before each move
//    GetUserMove();

//    Console.WriteLine("Do you want to continue? (y/n)");
//    running = Console.ReadLine()?.ToLower() == "y";
//}




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

    int startIndex = Helpers.SquareNameToIndex(startSquare);
    int endIndex = Helpers.SquareNameToIndex(endSquare);

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



