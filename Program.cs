using System;
using System.Collections.Generic;

namespace PblSquares
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int userChoice = 0;
            bool isAcceptable = false;
            Random random = new Random();

            bool[,] piece1 = new bool[5, 5];
            bool[,] piece2 = new bool[5, 5];
            bool[,] piece3 = new bool[5, 5];
            bool[,] piece4 = new bool[5, 5];
            bool[,] piece5 = new bool[5, 5];
            bool[,] piece6 = new bool[5, 5];
            bool[,] piece7 = new bool[5, 5];
            bool[,] piece8 = new bool[5, 5];
            bool[,] piece9 = new bool[5, 5];
            bool[,] piece10 = new bool[5, 5];
            bool[,] piece11 = new bool[5, 5];
            bool[,] piece12 = new bool[5, 5];
            bool[,] piece13 = new bool[5, 5];
            bool[,] piece14 = new bool[5, 5];
            bool[,] piece15 = new bool[5, 5];
            bool[,] piece16 = new bool[5, 5];
            bool[,] piece17 = new bool[5, 5];
            bool[,] piece18 = new bool[5, 5];
            bool[,] piece19 = new bool[5, 5];
            bool[,] piece20 = new bool[5, 5];

            Array[] pieceStorage = new Array[20]
            {
                piece1,piece2,piece3,piece4,piece5,piece6,piece7,piece8,piece9,piece10,
                piece11,piece12,piece13,piece14,piece15,piece16,piece17,piece18,piece19,piece20
            };

            while (userChoice == 0)
            {
                bool[,] tempPiece = new bool[5, 5];
                Console.Write("Pieces number:");
                int pieceCount = Convert.ToInt16(Console.ReadLine());

                for (int pieceIndex = 1; pieceIndex <= pieceCount; pieceIndex++)
                {
                    isAcceptable = false;
                    bool[,] currentPiece = new bool[5, 5];

                    while (!isAcceptable)
                    {
                        // fill grid randomly
                        for (int i = 0; i < 5; i++)
                            for (int k = 0; k < 5; k++)
                            {
                                currentPiece[i, k] = random.Next(0, 2) == 0; // true = X, false = .
                            }

                        // find a start X and total X count
                        int totalCellCount = 0;
                        int startRow = -1, startCol = -1;
                        for (int i = 0; i < 5; i++)
                            for (int k = 0; k < 5; k++)
                                if (currentPiece[i, k])
                                {
                                    totalCellCount++;
                                    if (startRow == -1) { startRow = i; startCol = k; }
                                }

                        if (totalCellCount < 2 | totalCellCount > 12)
                        {
                            isAcceptable = false;
                            continue;
                        }
                        if (totalCellCount == 0)
                        {
                            isAcceptable = false;
                            continue;
                        }

                        // BFS ile bağlılık kontrolü: start'tan erişilen X sayısı total'a eşit olmalı 
                        // BFS =Bir graf veya ızgara üzerinde, bir başlangıç düğümünden başlayarak önce aynı uzaklıktaki tüm düğümleri (aynı seviye) gezip sonra bir sonraki seviyeye geçer.
                        int reachableCells = BFSCount(currentPiece, startRow, startCol);
                        if (reachableCells != totalCellCount)
                        {
                            isAcceptable = false;
                            continue;
                        }

                        // check for isolated trues (gerekirse; BFS zaten bağlı bileşen kontrolünü yaptı,
                        // ama istenirse her X'in en az bir komşusu olduğunu ayrıca kontrol ederiz)
                        bool hasIsolatedCell = false;
                        for (int i = 0; i < 5 && !hasIsolatedCell; i++)
                        {
                            for (int k = 0; k < 5 && !hasIsolatedCell; k++)
                            {
                                if (!currentPiece[i, k]) continue;

                                bool hasNeighbor =
                                    (i > 0 && currentPiece[i - 1, k]) ||
                                    (i < 4 && currentPiece[i + 1, k]) ||
                                    (k > 0 && currentPiece[i, k - 1]) ||
                                    (k < 4 && currentPiece[i, k + 1]);

                                if (!hasNeighbor) hasIsolatedCell = true;
                            }
                        }

                        isAcceptable = !hasIsolatedCell;
                    }

                    // normalize / shift to top-left (senin eski shift mantığını fonksiyonlaştırdık)
                    currentPiece = NormalizeShift(currentPiece);

                    pieceStorage[pieceIndex] = currentPiece;
                    PrintPiece(currentPiece);
                }

                Console.WriteLine();
                if (isAcceptable) Console.WriteLine("0=restart, 1=call back");
                if (isAcceptable) userChoice = Convert.ToInt16(Console.ReadLine());
                if (userChoice == 1)
                {
                    Console.Write("which one: ");
                    int selectedPieceIndex = Convert.ToInt16(Console.ReadLine());
                    tempPiece = (bool[,])pieceStorage[selectedPieceIndex];
                    PrintPiece(tempPiece);
                    userChoice = 0;
                }
            }
        }

        //Asagisi AI help. :/                 <----


        // Yerel fonksiyon: start'tan erişilebilen true hücre sayısını döner (ortogonal bağlantı)
        static int BFSCount(bool[,] grid, int sr, int sc)
        {
            int n = grid.GetLength(0);
            int m = grid.GetLength(1);

            if (sr < 0 || sc < 0 || sr >= n || sc >= m) return 0;
            if (!grid[sr, sc]) return 0;

            bool[,] seen = new bool[n, m];
            Queue<(int r, int c)> q = new Queue<(int r, int c)>();
            q.Enqueue((sr, sc));
            seen[sr, sc] = true;
            int count = 0;

            int[] dr = new int[] { -1, 1, 0, 0 };
            int[] dc = new int[] { 0, 0, -1, 1 };

            while (q.Count > 0)
            {
                var (r, c) = q.Dequeue();
                if (!grid[r, c]) continue;
                count++;

                for (int d = 0; d < 4; d++)
                {
                    int nr = r + dr[d];
                    int nc = c + dc[d];
                    if (nr >= 0 && nr < n && nc >= 0 && nc < m && !seen[nr, nc] && grid[nr, nc])
                    {
                        seen[nr, nc] = true;
                        q.Enqueue((nr, nc));
                    }
                }
            }

            return count;
        }

        static bool[,] NormalizeShift(bool[,] grid)
        {
            int emptyRowCount = 0;
            int emptyColumnCount = 0;
            int consecutiveEmptyRow = 0;
            int consecutiveEmptyColumn = 0;

            // boş satır ve sütun sayısını bul
            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (grid[i, k] == false)
                    {
                        consecutiveEmptyRow++;
                        if (consecutiveEmptyRow == 5)
                            emptyRowCount++;
                    }
                    else
                        consecutiveEmptyRow = 0;

                    if (grid[k, i] == false)
                    {
                        consecutiveEmptyColumn++;
                        if (consecutiveEmptyColumn == 5)
                            emptyColumnCount++;
                    }
                    else
                        consecutiveEmptyColumn = 0;
                }
            }

            // satırları yukarı kaydır
            while (emptyRowCount > 0)
            {
                for (int i = 1; i < 5; i++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        grid[i - 1, k] = grid[i, k];
                    }
                }
                for (int k = 0; k < 5; k++)
                {
                    grid[4, k] = false;
                }
                emptyRowCount--;
            }

            // sütunları sola kaydır
            while (emptyColumnCount > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int k = 1; k < 5; k++)
                    {
                        grid[i, k - 1] = grid[i, k];
                    }
                }
                for (int k = 0; k < 5; k++)
                {
                    grid[k, 4] = false;
                }
                emptyColumnCount--;
            }

            return grid;
        }

        static bool[,] RotatePiece(bool[,] grid)
        {
            bool[,] rotated = new bool[5, 5];

            // 90 derece saat yönünde döndürme
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    rotated[c, 4 - r] = grid[r, c];
                }
            }

            return NormalizeShift(rotated);
        }

        static bool[,] ReversePiece(bool[,] grid)
        {
            bool[,] reversed = new bool[5, 5];

            // yatay ayna (soldan sağa ters çevirme)
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    reversed[r, 4 - c] = grid[r, c];
                }
            }

            return NormalizeShift(reversed);
        }

        static void PrintPiece(bool[,] pieceGrid)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 5; k++)
                    Console.Write(pieceGrid[i, k] ? 'X' : '.');
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
