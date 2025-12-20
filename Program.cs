using System;
using System.Text;

namespace Squares
{
    internal class Program
    {
        static bool quitRequested = false;

        // renkleri burada tanımladım (index 1..20 kullanılacak)
        static readonly ConsoleColor[] PieceColors = new ConsoleColor[21]
        {
            ConsoleColor.Gray,      // 0 - unused
            ConsoleColor.Blue,      // A
            ConsoleColor.Cyan,      // B
            ConsoleColor.Green,     // C
            ConsoleColor.Yellow,    // D
            ConsoleColor.Magenta,   // E
            ConsoleColor.Red,       // F
            ConsoleColor.DarkYellow,// G
            ConsoleColor.DarkRed,   // H
            ConsoleColor.DarkGreen, // I
            ConsoleColor.DarkCyan,  // J
            ConsoleColor.DarkMagenta,// K
            ConsoleColor.DarkBlue,  // L
            ConsoleColor.White,     // M
            ConsoleColor.DarkGray,  // N
            ConsoleColor.Blue,      // O (reused)
            ConsoleColor.Cyan,      // P (reused)
            ConsoleColor.Green,     // Q (reused)
            ConsoleColor.Yellow,    // R (reused)
            ConsoleColor.Magenta,   // S (reused)
            ConsoleColor.Red        // T (reused)
        };

        static ConsoleColor GetPieceColor(int idx)
        {
            if (idx <= 0 || idx >= PieceColors.Length) return Console.ForegroundColor;
            return PieceColors[idx];
        }

        static ConsoleColor GetPieceColor(char letter)
        {
            int idx = (letter - 'A') + 1;
            return GetPieceColor(idx);
        }

        static void Main(string[] args)
        {
            Random random = new Random();
            int Xcount = 0;
            int Rowloc = 0;
            int Colloc = 0;

            // creating pieces
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

            // storage array
            Array[] pieceStorage = new Array[21]
            {
                null,
                piece1, piece2, piece3, piece4, piece5, piece6, piece7, piece8, piece9, piece10,
                piece11, piece12, piece13, piece14, piece15, piece16, piece17, piece18, piece19, piece20
            };

            Console.WriteLine("Chose X numbering 2-12");
            int userX;
            while (!int.TryParse(Console.ReadLine(), out userX) || userX < 2)
            {
                Console.Write("Enter a valid integer (>=2): ");
            }

            // Pieces count
            Console.Write("Pieces number (1-20): ");
            int pieceCountInput;
            while (!int.TryParse(Console.ReadLine(), out pieceCountInput) || pieceCountInput < 1)
            {
                Console.Write("Enter valid pieces count (1-20): ");
            }
            int pieceCount = pieceCountInput > 20 ? 20 : pieceCountInput;

            // Generate pieces
            for (int pieceIndex = 1; pieceIndex <= pieceCount; pieceIndex++)
            {
                bool isUnique = false;
                bool[,] currentPiece = new bool[5, 5];
                int attemptCount = 0;
                bool gaveUp = false;
                bool flag = true;

                do
                {
                    attemptCount++;
                    if (attemptCount > 2000)
                    {
                        Console.WriteLine("\n[WARNING] Could not find any more unique pieces with " + userX + " amount of squares more than " + (pieceIndex - 1));
                        Console.WriteLine("Pieces generated until now: " + (pieceIndex - 1));
                        gaveUp = true;
                        break;
                    }

                    currentPiece = new bool[5, 5];
                    flag = true;
                    Xcount = 0;

                    // place X
                    do
                    {
                        Xcount = 0;
                        for (int r = 0; r < 5; r++)
                            for (int c = 0; c < 5; c++)
                            {
                                if (flag) currentPiece[r, c] = random.Next(0, 24) == 0;
                                else currentPiece[r, c] = false;

                                if (currentPiece[r, c]) Xcount++;

                                if (Xcount == 1 && flag)
                                {
                                    Rowloc = r;
                                    Colloc = c;
                                }

                                if (Xcount >= 1) flag = false;
                            }
                    } while (Xcount == 0);

                    // random walk
                    for (int i = 0, N = 0; i < (userX - 1 + N); i++)
                    {
                        int Step = random.Next(-2, 3);

                        if (Step == -2 && Colloc > 0)
                        {
                            if (!currentPiece[Rowloc, Colloc - 1]) currentPiece[Rowloc, Colloc - 1] = true;
                            else { N++; }
                            Colloc = Colloc - 1;
                        }
                        else if (Step == -1 && Rowloc < 4)
                        {
                            if (!currentPiece[Rowloc + 1, Colloc]) currentPiece[Rowloc + 1, Colloc] = true;
                            else { N++; }
                            Rowloc = Rowloc + 1;
                        }
                        else if (Step == 1 && Rowloc > 0)
                        {
                            if (!currentPiece[Rowloc - 1, Colloc]) currentPiece[Rowloc - 1, Colloc] = true;
                            else { N++; }
                            Rowloc = Rowloc - 1;
                        }
                        else if (Step == 2 && Colloc < 4)
                        {
                            if (!currentPiece[Rowloc, Colloc + 1]) currentPiece[Rowloc, Colloc + 1] = true;
                            else { N++; }
                            Colloc = Colloc + 1;
                        }
                        else { N++; }
                    }

                    currentPiece = NormalizeShift(currentPiece);
                    currentPiece = NormalizeShift(currentPiece);

                    isUnique = !CheckIfDuplicate(currentPiece, pieceStorage, pieceIndex);

                } while (!isUnique);

                if (gaveUp) break;

                pieceStorage[pieceIndex] = currentPiece;
                // Console.WriteLine("Piece " + pieceIndex + " accepted (Unique).");
                // üstteki kabul mesajını kaldırdım — istenen "alttaki piece 6" yazısını engellemek için.
            }

            PlayRounds(pieceStorage);

            Console.WriteLine();
            Console.WriteLine("Program finished. Press any key to exit.");
            Console.ReadKey();
        }

        static void PlayRounds(Array[] pieceStorage)
        {
            int round = 1;
            int totalScore = 0;

            Console.Write("Enter min regularity (0.00-1.00): ");
            double minReq = ReadDoubleBounded(0.0, 1.0);
            Console.Write("Enter max regularity (" + minReq.ToString("0.00") + "-1.00): ");
            double maxReq = ReadDoubleBounded(minReq, 1.0);

            while (true)
            {
                bool success = PlaySingleRound(pieceStorage, round, minReq, maxReq, out int scoreEarned);

                if (!success)
                {
                    Console.WriteLine();
                    if (quitRequested) Console.WriteLine("Quit requested. Final Score: " + totalScore);
                    else Console.WriteLine("Game Over. Final Score: " + totalScore);
                    break;
                }

                totalScore += scoreEarned;
                Console.WriteLine();
                Console.WriteLine("Round completed! Round Score: " + scoreEarned + " | Total Score: " + totalScore);

                Console.WriteLine("Set new regularity interval for next round:");
                Console.Write("Min (0.00-1.00): ");
                minReq = ReadDoubleBounded(0.0, 1.0);
                Console.Write("Max (" + minReq.ToString("0.00") + "-1.00): ");
                maxReq = ReadDoubleBounded(minReq, 1.0);

                Console.WriteLine("Continue? (Y/N)");
                char ch = char.ToUpperInvariant(Console.ReadKey(true).KeyChar);
                if (ch != 'Y') break;
                round++;
            }
        }

        static bool PlaySingleRound(Array[] pieceStorage, int roundNumber, double minReq, double maxReq, out int roundScore)
        {
            roundScore = 0;
            int puzzleRows = 20;
            int puzzleCols = 30;

            char[,] target;
            bool[,] targetMask;
            int totalSquares;
            int perimeter;
            int tries = 0;
            while (true)
            {
                target = BuildPuzzle(pieceStorage, puzzleRows, puzzleCols, 160, out totalSquares, out perimeter);
                targetMask = ToBoolGrid(target);
                double side = perimeter / 4.0;
                double denom = side * side;
                double reg = denom == 0 ? 0 : totalSquares / denom;
                if (reg >= minReq && reg <= maxReq) break;
                tries++;
                if (tries > 200) break;
            }

            bool[] allowed = new bool[21];
            for (int i = 0; i < target.GetLength(0); i++)
                for (int j = 0; j < target.GetLength(1); j++)
                    if (target[i, j] != '\0')
                        allowed[(target[i, j] - 'A') + 1] = true;

            char[,] player = new char[puzzleRows, puzzleCols];
            bool[] placed = new bool[21];
            bool[] hasState = new bool[21];
            bool[] dirty = new bool[21];
            bool[][,] placedGrid = new bool[21][,];
            int[] placedRow = new int[21];
            int[] placedCol = new int[21];

            int histCount = 0;
            char[] history = new char[64];
            int cursorR = 0, cursorC = 0;
            int selectedIndex = -1;

            bool[,] working = null;
            int ghostRow = 0;
            int ghostCol = 0;

            selectedIndex = 1;
            if (selectedIndex >= 1 && selectedIndex < pieceStorage.Length && pieceStorage[selectedIndex] != null)
            {
                working = CloneGrid((bool[,])pieceStorage[selectedIndex]);
                working = NormalizeShift(working);
                ghostRow = 0; ghostCol = 0;
            }

            bool prevCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                // simple redraw loop using SetCursorPosition(0,0) to avoid scrolling
                while (true)
                {
                    Console.SetCursorPosition(0, 0);

                    // Draw header
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Round: " + roundNumber);
                    Console.WriteLine("Pieces: ");
                    CalculateMinMaxRegularity(pieceStorage, out double minRd, out double maxRd);
                    Console.WriteLine("Min. Regularity: " + minRd.ToString("F2"));
                    Console.WriteLine("Max. Regularity: " + maxRd.ToString("F2"));
                    Console.WriteLine();

                    // Column header
                    Console.Write(" ");
                    for (int c = 0; c < puzzleCols; c++)
                        Console.Write(c % 2 == 1 ? (char)('0' + ((c + 1) % 10)) : ' ');
                    Console.WriteLine();

                    // gridStartRow: puzzle ekranının üst satırını yakala — palette için referans olacak
                    int gridStartRow = Console.CursorTop;

                    // get bounds of working preview once
                    int wMinR = 0, wMinC = 0, wMaxR = 0, wMaxC = 0;
                    bool previewValid = false;
                    if (working != null) { GetBounds(working, out wMinR, out wMinC, out wMaxR, out wMaxC); previewValid = CanPlaceOnTarget(player, targetMask, working, ghostRow, ghostCol); }

                    // Draw grid
                    var defaultColor = Console.ForegroundColor;
                    for (int r = 0; r < puzzleRows; r++)
                    {
                        if (r == 0) Console.Write(' ');
                        else { Console.Write(r % 2 == 1 ? (((r / 2 + 1) * 2) % 10).ToString() : " "); Console.Write(' '); }

                        for (int c = 0; c < puzzleCols; c++)
                        {
                            // ghost preview
                            if (working != null)
                            {
                                bool isGhost = false;
                                for (int i = 0; i < 5 && !isGhost; i++)
                                    for (int j = 0; j < 5 && !isGhost; j++)
                                        if (working[i, j])
                                        {
                                            int rr = ghostRow + (i - wMinR);
                                            int cc = ghostCol + (j - wMinC);
                                            if (rr == r && cc == c) isGhost = true;
                                        }
                                if (isGhost)
                                {
                                    char letter = (char)('A' + selectedIndex - 1);
                                    var col = GetPieceColor(selectedIndex);
                                    Console.ForegroundColor = col;
                                    Console.Write(letter);
                                    Console.ForegroundColor = defaultColor;
                                    continue;
                                }
                            }

                            char ch = player[r, c];
                            if (ch != '\0')
                            {
                                // placed piece: renkli yaz
                                var col = GetPieceColor(ch);
                                Console.ForegroundColor = col;
                                Console.Write(ch);
                                Console.ForegroundColor = defaultColor;
                            }
                            else if (target[r, c] != '\0')
                            {
                                // hedef hücreler (X) koyu gri ile göster
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write('X');
                                Console.ForegroundColor = defaultColor;
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }

                        Console.WriteLine();
                    }

                    // hizalama: paletteStartX puzzle genişliğine göre hesaplanır, paletteStartY gridStartRow ile eşitlenir
                    int paletteStartX = 2 + puzzleCols + 2; // 2 (satır başı) + puzzleCols + 2 boşluk
                    int paletteStartY = gridStartRow;

                    for (int p = 1; p < pieceStorage.Length; p++)
                    {
                        if (pieceStorage[p] == null) continue;
                        try
                        {
                            PositionPrint((bool[,])pieceStorage[p], p, paletteStartX, paletteStartY);
                        }
                        catch { /* SetCursorPosition hatası olursa atla */ }
                    }

                    // Info & controls (palette yazdırıldıktan sonra normal akışa devam)
                    int covered = 0, targetTotal = 0;
                    for (int i = 0; i < puzzleRows; i++)
                        for (int j = 0; j < puzzleCols; j++)
                        {
                            if (targetMask[i, j]) targetTotal++;
                            if (player[i, j] != '\0') covered++;
                        }

                    int percent = targetTotal == 0 ? 0 : (int)Math.Round(covered * 100.0 / targetTotal);

                    Console.WriteLine();
                    Console.WriteLine("CONTROLS");
                    Console.WriteLine("  Key(s)         | Action");
                    Console.WriteLine("  ----------------+------------------------------------------------");
                    Console.WriteLine("  Arrows         | Move preview (if active) / Move selected placed piece / Move cursor");
                    Console.WriteLine("  A - T          | Select piece (A=1 .. T=20)");
                    Console.WriteLine("  *              | Rotate preview");
                    Console.WriteLine("  -              | Flip preview (mirror)");
                    Console.WriteLine("  Enter          | Place preview (when active) / Finish round (when no preview)");
                    Console.WriteLine("  Space          | Place selected piece at cursor");
                    Console.WriteLine("  Backspace / Del| Remove piece under cursor");
                    Console.WriteLine("  U              | Undo last placement");
                    Console.WriteLine("  Q              | Quit game (finalize score)");
                    Console.WriteLine("  Esc            | Give up (end round without success)");
                    Console.WriteLine();
                    Console.WriteLine("STATUS");

                    // Selected piece gösterimi renkli yapıldı
                    Console.Write("  Selected Piece : ");
                    if (selectedIndex == -1) Console.WriteLine("-");
                    else
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = GetPieceColor(selectedIndex);
                        Console.WriteLine(((char)('A' + selectedIndex - 1)).ToString());
                        Console.ForegroundColor = prev;
                    }

                    Console.WriteLine("  Cursor (R,C)   : " + cursorR + "," + cursorC);
                    Console.WriteLine("  Placed cells   : " + covered + "    Target cells: " + targetTotal);
                    Console.WriteLine("  Progress       : " + percent + "%");
                    Console.WriteLine("  Preview valid  : " + (previewValid ? "YES" : "NO"));
                    Console.WriteLine();

                    // Input
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Global quit
                    if (key.Key == ConsoleKey.Q) { quitRequested = true; return false; }

                    // If placed piece selected and has state, arrow keys move it
                    if (selectedIndex != -1 && placed[selectedIndex] && hasState[selectedIndex])
                    {
                        if (key.Key == ConsoleKey.LeftArrow) { if (TryMovePlaced(player, targetMask, placedGrid[selectedIndex], ref placedRow[selectedIndex], ref placedCol[selectedIndex], -1, 0, (char)('A' + selectedIndex - 1))) dirty[selectedIndex] = true; continue; }
                        if (key.Key == ConsoleKey.RightArrow) { if (TryMovePlaced(player, targetMask, placedGrid[selectedIndex], ref placedRow[selectedIndex], ref placedCol[selectedIndex], 1, 0, (char)('A' + selectedIndex - 1))) dirty[selectedIndex] = true; continue; }
                        if (key.Key == ConsoleKey.UpArrow) { if (TryMovePlaced(player, targetMask, placedGrid[selectedIndex], ref placedRow[selectedIndex], ref placedCol[selectedIndex], 0, -1, (char)('A' + selectedIndex - 1))) dirty[selectedIndex] = true; continue; }
                        if (key.Key == ConsoleKey.DownArrow) { if (TryMovePlaced(player, targetMask, placedGrid[selectedIndex], ref placedRow[selectedIndex], ref placedCol[selectedIndex], 0, 1, (char)('A' + selectedIndex - 1))) dirty[selectedIndex] = true; continue; }
                    }

                    // If preview active, arrows move preview
                    if (working != null)
                    {
                        int tryRow = ghostRow, tryCol = ghostCol;
                        bool isArrow = false;
                        if (key.Key == ConsoleKey.LeftArrow) { tryCol--; isArrow = true; }
                        else if (key.Key == ConsoleKey.RightArrow) { tryCol++; isArrow = true; }
                        else if (key.Key == ConsoleKey.UpArrow) { tryRow--; isArrow = true; }
                        else if (key.Key == ConsoleKey.DownArrow) { tryRow++; isArrow = true; }

                        if (isArrow)
                        {
                            if (!WouldBeCompletelyOutside(working, tryRow, tryCol, puzzleRows, puzzleCols))
                            {
                                ghostRow = tryRow; ghostCol = tryCol;
                            }
                            continue;
                        }
                    }

                    // Cursor movement
                    if (key.Key == ConsoleKey.LeftArrow && cursorC > 0) { cursorC--; continue; }
                    if (key.Key == ConsoleKey.RightArrow && cursorC < puzzleCols - 1) { cursorC++; continue; }
                    if (key.Key == ConsoleKey.UpArrow && cursorR > 0) { cursorR--; continue; }
                    if (key.Key == ConsoleKey.DownArrow && cursorR < puzzleRows - 1) { cursorR++; continue; }

                    // Select piece A-T
                    char kc = char.ToUpperInvariant(key.KeyChar);
                    if (kc >= 'A' && kc <= 'T')
                    {
                        int idx = (kc - 'A') + 1;
                        if (idx >= 1 && idx < pieceStorage.Length && allowed[idx] && pieceStorage[idx] != null)
                        {
                            selectedIndex = idx;
                            if (placed[idx] && hasState[idx]) working = null;
                            else { working = CloneGrid((bool[,])pieceStorage[idx]); working = NormalizeShift(working); ghostRow = 0; ghostCol = 0; }
                        }
                        continue;
                    }

                    // Rotate, flip
                    if (key.Key == ConsoleKey.Multiply || key.KeyChar == '*') { if (working != null) RotatePiece(working); continue; }
                    if (key.Key == ConsoleKey.Subtract || key.Key == ConsoleKey.OemMinus || key.KeyChar == '-') { if (working != null) ReversePiece(working); continue; }

                    // Place preview
                    if (key.Key == ConsoleKey.Enter && working != null && selectedIndex != -1)
                    {
                        char letter = (char)('A' + selectedIndex - 1);
                        if (TryPlaceOnTarget(player, targetMask, working, ghostRow, ghostCol, letter))
                        {
                            placed[selectedIndex] = true;
                            placedGrid[selectedIndex] = CloneGrid(working);
                            GetBounds(working, out int minR, out int minC, out int maxR, out int maxC);
                            placedRow[selectedIndex] = ghostRow;
                            placedCol[selectedIndex] = ghostCol;
                            hasState[selectedIndex] = true;
                            dirty[selectedIndex] = false;
                            history[histCount++] = letter;
                            working = null;
                        }
                        continue;
                    }

                    // Place at cursor
                    if (key.Key == ConsoleKey.Spacebar && working != null && selectedIndex != -1)
                    {
                        GetBounds(working, out int minR, out int minC, out int maxR, out int maxC);
                        char letter = (char)('A' + selectedIndex - 1);
                        if (TryPlaceOnTarget(player, targetMask, working, cursorR, cursorC, letter))
                        {
                            placed[selectedIndex] = true;
                            placedGrid[selectedIndex] = CloneGrid(working);
                            placedRow[selectedIndex] = cursorR - minR;
                            placedCol[selectedIndex] = cursorC - minC;
                            hasState[selectedIndex] = true;
                            dirty[selectedIndex] = false;
                            history[histCount++] = letter;
                            working = null;
                        }
                        continue;
                    }

                    // Remove / Undo
                    if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete)
                    {
                        char here = player[cursorR, cursorC];
                        if (here != '\0')
                        {
                            int idx = (here - 'A') + 1;
                            ClearLetter(player, here);
                            placed[idx] = false;
                            hasState[idx] = false;
                            placedGrid[idx] = null;
                            for (int h = histCount - 1; h >= 0; h--)
                                if (history[h] == here)
                                {
                                    for (int s = h; s < histCount - 1; s++) history[s] = history[s + 1];
                                    histCount--;
                                    break;
                                }
                        }
                        continue;
                    }

                    if (key.Key == ConsoleKey.U)
                    {
                        if (histCount > 0)
                        {
                            char last = history[histCount - 1];
                            history[histCount - 1] = '\0';
                            histCount--;
                            int idx = (last - 'A') + 1;
                            ClearLetter(player, last);
                            placed[idx] = false;
                            hasState[idx] = false;
                            placedGrid[idx] = null;
                        }
                        continue;
                    }

                    // Finish round
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (BoardsMatch(targetMask, player))
                        {
                            double side3 = perimeter / 4.0;
                            double denom3 = side3 * side3;
                            double reg3 = denom3 == 0 ? 0 : totalSquares / denom3;
                            roundScore = (int)Math.Round(totalSquares * Math.Pow(4 * reg3, 4));
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Not matching the puzzle. Round failed.");
                            System.Threading.Thread.Sleep(700);
                            return false;
                        }
                    }

                    if (key.Key == ConsoleKey.Escape) return false;
                }
            }
            finally
            {
                Console.CursorVisible = prevCursorVisible;
            }
        }

        // HELPER METHODS

        static bool CheckIfDuplicate(bool[,] newPiece, Array[] storage, int currentIndex)
        {
            if (currentIndex <= 1) return false;
            bool[,] checkPiece = CloneGrid(newPiece);
            for (int rot = 0; rot < 4; rot++)
            {
                checkPiece = NormalizeShift(checkPiece);
                for (int i = 1; i < currentIndex; i++)
                {
                    if (storage[i] != null)
                    {
                        bool[,] existingPiece = (bool[,])storage[i];
                        if (AreMatricesEqual(checkPiece, existingPiece)) return true;
                    }
                }
                RotatePiece(checkPiece);
            }
            return false;
        }

        static bool AreMatricesEqual(bool[,] p1, bool[,] p2)
        {
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (p1[r, c] != p2[r, c]) return false;
            return true;
        }

        static bool[,] CloneGrid(bool[,] source)
        {
            bool[,] dest = new bool[5, 5];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    dest[r, c] = source[r, c];
            return dest;
        }

        static bool[,] NormalizeShift(bool[,] grid)
        {
            int rows = 5, cols = 5;
            int top = -1, left = -1;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (grid[r, c]) { if (top == -1) top = r; if (left == -1 || c < left) left = c; }
            if (top == -1) return grid;
            bool[,] outGrid = new bool[5, 5];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    if (r + top < rows && c + left < cols && grid[r + top, c + left]) outGrid[r, c] = true;
                    else outGrid[r, c] = false;
            return outGrid;
        }

        static void RotatePiece(bool[,] grid)
        {
            bool[,] rotated = new bool[5, 5];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    rotated[c, 4 - r] = grid[r, c];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    grid[r, c] = rotated[r, c];
        }

        static void ReversePiece(bool[,] grid)
        {
            bool[,] reversed = new bool[5, 5];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    reversed[r, 4 - c] = grid[r, c];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    grid[r, c] = reversed[r, c];
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

        static void PositionPrint(bool[,] grid, int pieceIndex, int startX, int startY)
        {
            int columnIndex = (pieceIndex - 1) % 5;
            int rowIndex = (pieceIndex - 1) / 5;
            int baseX = startX + columnIndex * 8;
            int baseY = startY + rowIndex * 5;
            char letter = (char)('A' + (pieceIndex - 1));
            for (int i = 0; i < 4; i++)
            {
                try { Console.SetCursorPosition(baseX, baseY + i); }
                catch { continue; }
                for (int k = 0; k < 5; k++)
                {
                    if (grid[i, k])
                    {
                        var prev = Console.ForegroundColor;
                        Console.ForegroundColor = GetPieceColor(pieceIndex);
                        Console.Write(letter);
                        Console.ForegroundColor = prev;
                    }
                    else Console.Write('.');
                }
            }
        }

        static double CalculateRegularity(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int totalSquares = 0, perimeter = 0;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (!grid[r, c]) continue;
                    totalSquares++;
                    if (r == 0 || !grid[r - 1, c]) perimeter++;
                    if (r == rows - 1 || !grid[r + 1, c]) perimeter++;
                    if (c == 0 || !grid[r, c - 1]) perimeter++;
                    if (c == cols - 1 || !grid[r, c + 1]) perimeter++;
                }
            if (perimeter == 0) return 0.0;
            double side = perimeter / 4.0;
            return totalSquares / (side * side);
        }

        static void CalculateMinMaxRegularity(Array[] pieceStorage, out double minReg, out double maxReg)
        {
            minReg = double.MaxValue;
            maxReg = double.MinValue;
            bool found = false;
            for (int i = 1; i < pieceStorage.Length; i++)
            {
                if (pieceStorage[i] == null) continue;
                bool[,] p = (bool[,])pieceStorage[i];
                double r = CalculateRegularity(p);
                if (r > 0.0)
                {
                    found = true;
                    if (r < minReg) minReg = r;
                    if (r > maxReg) maxReg = r;
                }
            }
            if (!found) { minReg = 0.0; maxReg = 0.0; }
        }

        static char[,] BuildPuzzle(Array[] pieceStorage, int rows, int cols, int maxSquares, out int totalSquares, out int perimeter)
        {
            char[,] board = new char[rows, cols];
            totalSquares = 0;
            perimeter = 0;
            Random r = new Random();
            bool anyPlaced = false;

            for (int i = 1; i < pieceStorage.Length; i++)
            {
                if (!(pieceStorage[i] is bool[,] src)) continue;
                bool[,] work = CloneGrid(src);
                if (r.Next(0, 2) == 1) ReversePiece(work);
                int rot = r.Next(0, 4);
                for (int k = 0; k < rot; k++) RotatePiece(work);
                work = NormalizeShift(work);
                int pieceSquares = CountSquares(work);
                if (pieceSquares == 0) continue;
                if (totalSquares + pieceSquares > maxSquares) continue;
                GetBounds(work, out int minR, out int minC, out int maxR, out int maxC);
                int h = maxR - minR + 1;
                int w = maxC - minC + 1;
                if (h > rows || w > cols) continue;

                bool placedFlag = false;
                for (int t = 0; t < 200 && !placedFlag; t++)
                {
                    int top = r.Next(0, rows - h + 1);
                    int left = r.Next(0, cols - w + 1);
                    bool overlap = false;
                    bool touches = false;
                    for (int pr = 0; pr < 5 && !overlap; pr++)
                    {
                        for (int pc = 0; pc < 5; pc++)
                        {
                            if (!work[pr, pc]) continue;
                            int rr = top + (pr - minR);
                            int cc = left + (pc - minC);
                            if (rr < 0 || cc < 0 || rr >= rows || cc >= cols) { overlap = true; break; }
                            if (board[rr, cc] != '\0') { overlap = true; break; }
                            if (anyPlaced)
                            {
                                if (rr > 0 && board[rr - 1, cc] != '\0') touches = true;
                                if (rr < rows - 1 && board[rr + 1, cc] != '\0') touches = true;
                                if (cc > 0 && board[rr, cc - 1] != '\0') touches = true;
                                if (cc < cols - 1 && board[rr, cc + 1] != '\0') touches = true;
                            }
                        }
                    }
                    if (overlap) continue;
                    if (!anyPlaced && !placedFlag) { } else if (!touches) continue;
                    char letter = (char)('A' + (i - 1));
                    for (int pr = 0; pr < 5; pr++)
                        for (int pc = 0; pc < 5; pc++)
                            if (work[pr, pc])
                            {
                                int rr = top + (pr - minR);
                                int cc = left + (pc - minC);
                                board[rr, cc] = letter;
                            }
                    placedFlag = true;
                    anyPlaced = true;
                }
                if (placedFlag) totalSquares += pieceSquares;
            }

            bool[,] mask = ToBoolGrid(board);
            ComputeTotals(mask, out int ts, out int per);
            totalSquares = ts;
            perimeter = per;
            return board;
        }

        static int CountSquares(bool[,] g)
        {
            int cnt = 0;
            for (int i = 0; i < g.GetLength(0); i++)
                for (int j = 0; j < g.GetLength(1); j++)
                    if (g[i, j]) cnt++;
            return cnt;
        }

        static void GetBounds(bool[,] g, out int minR, out int minC, out int maxR, out int maxC)
        {
            minR = 5; minC = 5; maxR = -1; maxC = -1;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (g[i, j])
                    {
                        if (i < minR) minR = i;
                        if (j < minC) minC = j;
                        if (i > maxR) maxR = i;
                        if (j > maxC) maxC = j;
                    }
            if (maxR == -1) { minR = minC = 0; maxR = maxC = 0; }
        }

        static bool TryPlace(char[,] board, bool[,] piece, int top, int left, char letter, int minR, int minC)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = top + (i - minR);
                        int cc = left + (j - minC);
                        if (rr < 0 || cc < 0 || rr >= board.GetLength(0) || cc >= board.GetLength(1)) return false;
                        if (board[rr, cc] != '\0') return false;
                    }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = top + (i - minR);
                        int cc = left + (j - minC);
                        board[rr, cc] = letter;
                    }

            return true;
        }

        static bool[,] ToBoolGrid(char[,] board)
        {
            int r = board.GetLength(0);
            int c = board.GetLength(1);
            bool[,] b = new bool[r, c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    b[i, j] = board[i, j] != '\0';
            return b;
        }

        static void ComputeTotals(bool[,] grid, out int totalSquares, out int perimeter)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            totalSquares = 0;
            perimeter = 0;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    if (!grid[r, c]) continue;
                    totalSquares++;
                    if (r == 0 || !grid[r - 1, c]) perimeter++;
                    if (r == rows - 1 || !grid[r + 1, c]) perimeter++;
                    if (c == 0 || !grid[r, c - 1]) perimeter++;
                    if (c == cols - 1 || !grid[r, c + 1]) perimeter++;
                }
        }

        static bool CanPlaceOnTarget(char[,] player, bool[,] targetMask, bool[,] piece, int row, int col)
        {
            GetBounds(piece, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = row + (i - minR);
                        int cc = col + (j - minC);
                        if (rr < 0 || cc < 0 || rr >= player.GetLength(0) || cc >= player.GetLength(1)) return false;
                        if (!targetMask[rr, cc]) return false;
                        if (player[rr, cc] != '\0') return false;
                    }
            return true;
        }

        static bool TryPlaceOnTarget(char[,] player, bool[,] targetMask, bool[,] piece, int row, int col, char letter)
        {
            GetBounds(piece, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = row + (i - minR);
                        int cc = col + (j - minC);
                        if (rr < 0 || cc < 0 || rr >= player.GetLength(0) || cc >= player.GetLength(1)) return false;
                        if (!targetMask[rr, cc]) return false;
                        if (player[rr, cc] != '\0') return false;
                    }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = row + (i - minR);
                        int cc = col + (j - minC);
                        player[rr, cc] = letter;
                    }
            return true;
        }

        static void ClearLetter(char[,] board, char letter)
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    if (board[i, j] == letter) board[i, j] = '\0';
        }

        static bool BoardsMatch(bool[,] targetMask, char[,] player)
        {
            for (int i = 0; i < player.GetLength(0); i++)
                for (int j = 0; j < player.GetLength(1); j++)
                {
                    bool t = targetMask[i, j];
                    bool p = player[i, j] != '\0';
                    if (t != p) return false;
                }
            return true;
        }

        static double ReadDoubleBounded(double min, double max)
        {
            while (true)
            {
                string s = Console.ReadLine();
                double v;
                if (double.TryParse(s, out v))
                {
                    if (v < min) v = min;
                    if (v > max) v = max;
                    return v;
                }
                Console.Write("Enter a number (" + min.ToString("0.00") + " - " + max.ToString("0.00") + "): ");
            }
        }

        static bool TryMovePlaced(char[,] player, bool[,] targetMask, bool[,] pieceGrid, ref int top, ref int left, int dx, int dy, char letter)
        {
            int newLeft = left + dx;
            int newTop = top + dy;

            // remove current letter
            for (int i = 0; i < player.GetLength(0); i++)
                for (int j = 0; j < player.GetLength(1); j++)
                    if (player[i, j] == letter) player[i, j] = '\0';

            GetBounds(pieceGrid, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (pieceGrid[i, j])
                    {
                        int rr = newTop + (i - minR);
                        int cc = newLeft + (j - minC);
                        if (rr < 0 || cc < 0 || rr >= player.GetLength(0) || cc >= player.GetLength(1)) { PlaceBack(player, pieceGrid, top, left, letter); return false; }
                        if (!targetMask[rr, cc]) { PlaceBack(player, pieceGrid, top, left, letter); return false; }
                        if (player[rr, cc] != '\0') { PlaceBack(player, pieceGrid, top, left, letter); return false; }
                    }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (pieceGrid[i, j])
                    {
                        int rr = newTop + (i - minR);
                        int cc = newLeft + (j - minC);
                        player[rr, cc] = letter;
                    }

            top = newTop;
            left = newLeft;
            return true;
        }

        static bool TryReorientPlaced(char[,] player, bool[,] targetMask, bool[,] newGrid, ref int top, ref int left, char letter)
        {
            // remove existing letter
            for (int i = 0; i < player.GetLength(0); i++)
                for (int j = 0; j < player.GetLength(1); j++)
                    if (player[i, j] == letter) player[i, j] = '\0';

            GetBounds(newGrid, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (newGrid[i, j])
                    {
                        int rr = top + (i - minR);
                        int cc = left + (j - minC);
                        if (rr < 0 || cc < 0 || rr >= player.GetLength(0) || cc >= player.GetLength(1)) return false;
                        if (!targetMask[rr, cc]) return false;
                        if (player[rr, cc] != '\0') return false;
                    }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (newGrid[i, j])
                    {
                        int rr = top + (i - minR);
                        int cc = left + (j - minC);
                        player[rr, cc] = letter;
                    }

            return true;
        }

        static void PlaceBack(char[,] player, bool[,] grid, int top, int left, char letter)
        {
            GetBounds(grid, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (grid[i, j])
                    {
                        int rr = top + (i - minR);
                        int cc = left + (j - minC);
                        player[rr, cc] = letter;
                    }
        }

        static bool WouldBeCompletelyOutside(bool[,] piece, int newTop, int newLeft, int boardRows, int boardCols)
        {
            GetBounds(piece, out int minR, out int minC, out int maxR, out int maxC);
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (piece[i, j])
                    {
                        int rr = newTop + (i - minR);
                        int cc = newLeft + (j - minC);
                        if (rr >= 0 && rr < boardRows && cc >= 0 && cc < boardCols) return false;
                    }
            return true;
        }
    }
}