namespace Engine;

static public class Helpers
{

    public static bool IsValidIndex(int index) => index >= 0 && index < 64;

    public static int SquareNameToIndex(string squareName)
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
        // Console.WriteLine($"Mapped Square {squareName} to Index {boardIndex}");

        return boardIndex;
    }

    public static string IndexToSquareName(int index)
    {
        int rank = 7 - (index / 8);
        int file = index % 8;
        return $"{(char)('a' + file)}{rank + 1}";
    }


    

}
