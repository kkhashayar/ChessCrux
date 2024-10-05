using static Engine.PieceDefinitions;

namespace Engine
{
    public class MoveGeneration
    {
        private Globals globals;

        // Constructor
        public MoveGeneration(Globals globals)
        {
            this.globals = globals;
        }

        public bool IsKingInCheck(ChessPieceColor color)
        {
            // king position 
            int kingIndex = -1; 
            for(int i = 0; i < 64; i++)
            {
                if (globals.Squares[i].Piece.PieceType == ChessPieceType.King && 
                    globals.Squares[i].Piece.PieceColor == color)
                {
                    kingIndex = i;
                    break; 
                }
            }
            if(kingIndex == -1)
            {
                Console.WriteLine("No king on the board");
                return false;
            }
            // Check for attacks by opponent
            // ChessPieceColor opponentColor = (ChessPieceColor)((int)color & 1);

            var opponentColor = ChessPieceColor.None;
            if(color == ChessPieceColor.White)opponentColor = ChessPieceColor.Black;    
            else if(color == ChessPieceColor.Black) opponentColor = ChessPieceColor.White;

            // Check for Pawn attacks
            if (IsSquareUnderPawnAttack(kingIndex, opponentColor)) return true;

            // Check for Knight attacks
            //if (IsSquareUnderKnightAttack(kingIndex, opponentColor)) return true;

            //// Check for Rook or Queen (Horizontal/Vertical) attacks
            //if (IsSquareUnderRookOrQueenAttack(kingIndex, opponentColor)) return true;

            //// Check for Bishop or Queen (Diagonal) attacks
            //if (IsSquareUnderBishopOrQueenAttack(kingIndex, opponentColor)) return true;
            if (IsSquareUnderKingAttack(kingIndex, opponentColor)) return true;

            return false;
        }
        /**************** Methods to check square under attack ****************/
        public bool IsSquareUnderPawnAttack(int squareIndex, ChessPieceColor attackerColor)
        {
            // Calculate rank and file of the target square
            int rank = squareIndex / 8;
            int file = squareIndex % 8;

            // Check for pawn attacks based on color
            if (attackerColor == ChessPieceColor.White)
            {
                // White pawns attack diagonally upwards (rank - 1)
                if (rank > 0)
                {
                    // Check for attacks from the left diagonal (rank - 1, file - 1)
                    if (file > 0 && globals.Squares[(rank - 1) * 8 + (file - 1)].Piece.PieceType == ChessPieceType.Pawn &&
                        globals.Squares[(rank - 1) * 8 + (file - 1)].Piece.PieceColor == ChessPieceColor.White)
                        return true;

                    // Check for attacks from the right diagonal (rank - 1, file + 1)
                    if (file < 7 && globals.Squares[(rank - 1) * 8 + (file + 1)].Piece.PieceType == ChessPieceType.Pawn &&
                        globals.Squares[(rank - 1) * 8 + (file + 1)].Piece.PieceColor == ChessPieceColor.White)
                        return true;
                }
            }
            else if (attackerColor == ChessPieceColor.Black)
            {
                // Black pawns attack diagonally downwards (rank + 1)
                if (rank < 7)
                {
                    // Check for attacks from the left diagonal (rank + 1, file - 1)
                    if (file > 0 && globals.Squares[(rank + 1) * 8 + (file - 1)].Piece.PieceType == ChessPieceType.Pawn &&
                        globals.Squares[(rank + 1) * 8 + (file - 1)].Piece.PieceColor == ChessPieceColor.Black)
                        return true;

                    // Check for attacks from the right diagonal (rank + 1, file + 1)
                    if (file < 7 && globals.Squares[(rank + 1) * 8 + (file + 1)].Piece.PieceType == ChessPieceType.Pawn &&
                        globals.Squares[(rank + 1) * 8 + (file + 1)].Piece.PieceColor == ChessPieceColor.Black)
                        return true;
                }
            }

            // No pawn attacks found
            return false;
        }

        public bool IsSquareUnderKingAttack(int squareIndex, ChessPieceColor attackerColor)
        {
            int[] kingMoves = { -1, 1, -8, 8, -9, -7, 7, 9 }; // All possible king moves

            foreach (int move in kingMoves)
            {
                int targetIndex = squareIndex + move;

                if (Helpers.IsValidIndex(targetIndex)) // Ensure index is within the board
                {
                    Piece piece = globals.Squares[targetIndex].Piece;
                    if (piece.PieceType == ChessPieceType.King && piece.PieceColor == attackerColor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /************End of Methods to check square under attack ****************/

        public List<int[]> GenerateAllMoves()
        {
            List<int[]> allMoves = new List<int[]>();
            List<int[]> legalMoves = new List<int[]>();

            for (int index = 0; index < 64; index++)
            {
                Piece piece = globals.Squares[index].Piece;

                if (piece.PieceType != ChessPieceType.None && piece.PieceColor == globals.Turn)
                {
                    switch (piece.PieceType)
                    {
                        case ChessPieceType.Pawn:
                            GeneratePawnMoves(index, allMoves);
                            break;
                        case ChessPieceType.Knight:
                            GenerateKnightMoves(index, allMoves);
                            break;
                        case ChessPieceType.King:
                            GenerateKingMoves(index, allMoves);
                            break;// Rest of cases 
                    }
                }
            }
            // Check if king is in check after each move
            foreach (var move in allMoves)
            {
                int fromIndex = move[0];
                int toIndex = move[1];

                // Clone the board and make the move
                Globals clonedBoard = globals.Clone();
                clonedBoard.MovePiece(fromIndex, toIndex);

                // Create a new MoveGeneration instance to check for king's status
                MoveGeneration clonedMoveGen = new MoveGeneration(clonedBoard);

                // If the move does not leave the king in check, add it to legal moves
                if (!clonedMoveGen.IsKingInCheck(globals.Turn))
                {
                    legalMoves.Add(move);
                }
            }
            return legalMoves;
        }

        public long Perft(int depth)
        {
            if (depth == 0) return 1;

            List<int[]> moves = GenerateAllMoves();
            if (moves.Count == 0) Console.WriteLine("No moves generated!");

            Console.WriteLine($"Depth: {depth}, Moves: {moves.Count}");

            long nodes = 0;

            // Use a Dictionary to store each 
            Dictionary<string, long> moveCounts = new Dictionary<string, long>();

            foreach (var move in moves)
            {
                int fromIndex = move[0];
                int toIndex = move[1];

                // Clone the current board state
                Globals clonedBoard = globals.Clone();

                // Make the move on the cloned board
                clonedBoard.MovePiece(fromIndex, toIndex);

                // Recursively call Perft on the cloned board
                MoveGeneration clonedMoveGen = new MoveGeneration(clonedBoard);
                long childNodes = clonedMoveGen.Perft(depth - 1);

                // Format move like "e2e4"
                string moveString = Helpers.IndexToSquareName(fromIndex) + Helpers.IndexToSquareName(toIndex);

                moveCounts[moveString] = childNodes;
                nodes += childNodes;
            }
            if (depth == 1)
            {
                foreach (var move in moveCounts)
                {
                    Console.WriteLine($"{move.Key}: {move.Value}");
                }
            }

            return nodes;
        }

        // Piece move generation 
        private void GenerateKingMoves(int index, List<int[]> allMoves)
        {
            int[] kingOffsets = { -9, -8,-7,-1, 1, 7, 8, 9};
            foreach (var offset in kingOffsets)
            {
                int toIndex = index + offset;
                if (Helpers.IsValidIndex(toIndex))
                {
                    if (globals.Squares[toIndex].Piece.PieceColor != globals.Squares[index].Piece.PieceColor)
                    {
                        allMoves.Add(new int[] { index, toIndex });
                    }
                }
            }
        }
        private void GeneratePawnMoves(int fromIndex, List<int[]> moveList)
        {
            Piece piece = globals.Squares[fromIndex].Piece;

            // Determine the direction based on color
            int direction = (piece.PieceColor == ChessPieceColor.White) ? -8 : 8;

            // Single square move
            int singleStep = fromIndex + direction;
            if (Helpers.IsValidIndex(singleStep) && globals.Squares[singleStep].Piece.PieceType == ChessPieceType.None)
            {
                moveList.Add(new int[] { fromIndex, singleStep });
            }

            // Double square move from the starting rank
            int startRank = (piece.PieceColor == ChessPieceColor.White) ? 6 : 1;
            int fromRank = fromIndex / 8;
            if (fromRank == startRank)
            {
                int doubleStep = fromIndex + 2 * direction;
                if (Helpers.IsValidIndex(doubleStep) && globals.Squares[doubleStep].Piece.PieceType == ChessPieceType.None &&
                    globals.Squares[singleStep].Piece.PieceType == ChessPieceType.None)
                {
                    moveList.Add(new int[] { fromIndex, doubleStep });
                }
            }

            // Capture moves (left and right)
            int captureLeft = fromIndex + direction - 1;
            int captureRight = fromIndex + direction + 1;

            if (Helpers.IsValidIndex(captureLeft) && (fromIndex % 8 > 0) &&
                globals.Squares[captureLeft].Piece.PieceType != ChessPieceType.None &&
                globals.Squares[captureLeft].Piece.PieceColor != piece.PieceColor)
            {
                moveList.Add(new int[] { fromIndex, captureLeft });
            }

            if (Helpers.IsValidIndex(captureRight) && (fromIndex % 8 < 7) &&
                globals.Squares[captureRight].Piece.PieceType != ChessPieceType.None &&
                globals.Squares[captureRight].Piece.PieceColor != piece.PieceColor)
            {
                moveList.Add(new int[] { fromIndex, captureRight });
            }
        }

        private void GenerateKnightMoves(int fromIndex, List<int[]> moveList)
        {
            Piece piece = globals.Squares[fromIndex].Piece;

            // Knight movement pattern offsets
            int[] knightOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

            foreach (int offset in knightOffsets)
            {
                int targetIndex = fromIndex + offset;
                if (Helpers.IsValidIndex(targetIndex))
                {
                    Piece targetPiece = globals.Squares[targetIndex].Piece;
                    if (targetPiece.PieceType == ChessPieceType.None || targetPiece.PieceColor != piece.PieceColor)
                    {
                        moveList.Add(new int[] { fromIndex, targetIndex });
                    }
                }
            }
        }

        // rest of methods 
    }
}
