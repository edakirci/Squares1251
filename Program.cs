using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Squares
{
    internal class Program
    {
        static bool quitRequested = false;
        static bool giveUpRequested = false; // NEW: ESC detection

        // defined colors here (indexes 1..20 will be used)
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

        // Inner class encapsulating the splash screen (to avoid name conflicts)
        private static class SplashScreen
        {
            // Console lock (to prevent two threads writing to screen simultaneously)
            private static readonly object _lock = new object();

            // Map that keeps track of occupied screen cells (x, y)
            private static bool[,] filledSpaces;
            private static int width = 120;
            private static int height = 30;

            // Control to stop the animation
            private static bool continueAnimation = true;

            public static void Show()
            {
                try
                {
                    Console.Clear();
                    Console.Title = "SQUARES - Tetris Animation";
                }
                catch { }

                bool prevCursor = Console.CursorVisible;
                Console.CursorVisible = false;

                try { Console.SetWindowSize(width, height); Console.SetBufferSize(width, height); }
                catch { width = Console.WindowWidth; height = Console.WindowHeight; }

                filledSpaces = new bool[width, height];

                Task.Run(() => startTetrisAnimation());

                string sLetter = @"  /$$$$$$ 
 /$$__  $$
| $$  \/
|  $$$$$$ 
 \____  $$
 /$$  \ $$
|  $$$$$$/
 \/";
                string qLetter = @"
  /$$$$$$ 
 /$$__  $$
| $$  \ $$
| $$  | $$
|  $$$$$$$
 \____  $$
      | $$
      | $$
      |/";
                string uLetter = @"
 /$$   /$$
| $$  | $$
| $$  | $$
| $$  | $$
|  $$$$$$/
 \/";
                string aLetter = @"
  /$$$$$$ 
 |____  $$
  /$$$$$$$
 /$$__  $$
|  $$$$$$$
 \_/";
                string eLetter = @"
  /$$$$$$ 
 /$$__  $$
| $$$$$$$$
| $$_/
|  $$$$$$$
 \_/";
                string rLetter = @"
  /$$$$$$ 
 /$$__  $$
| $$  \/
| $$      
| $$      
|/      ";
                string sLetterLast = @"
 /$$$$$$$
/$$_/
|  $$$$$$ 
 \____  $$
 /$$$$$$$/
|_/ ";

                drawLetter(sLetter, 5, 10, ConsoleColor.DarkMagenta);
                Thread.Sleep(50);

                drawLetter(qLetter, 18, 10, ConsoleColor.Green);
                Thread.Sleep(50);

                drawLetter(uLetter, 31, 10, ConsoleColor.DarkBlue);
                Thread.Sleep(50);

                drawLetter(aLetter, 44, 10, ConsoleColor.Magenta);
                Thread.Sleep(50);

                drawLetter(eLetter, 57, 10, ConsoleColor.Yellow);
                Thread.Sleep(50);

                drawLetter(rLetter, 70, 10, ConsoleColor.Cyan);
                Thread.Sleep(50);

                drawLetter(sLetterLast, 83, 10, ConsoleColor.Gray);

                lock (_lock)
                {
                    Console.SetCursorPosition(5, Math.Min(25, height - 1));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Press ENTER to continue");
                }

                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter) break;
                }

                continueAnimation = false;
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = prevCursor;
            }

            static void drawLetter(string art, int sol, int upper, ConsoleColor color)
            {
                string[] rows = art.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                lock (_lock)
                {
                    Console.ForegroundColor = color;
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (upper + i < height)
                        {
                            try { Console.SetCursorPosition(sol, upper + i); } catch { continue; }
                            Console.Write(rows[i]);

                            for (int j = 0; j < rows[i].Length; j++)
                            {
                                if (rows[i][j] != ' ' && sol + j < width && upper + i < height)
                                {
                                    filledSpaces[sol + j, upper + i] = true;
                                }
                            }
                        }
                    }
                }
            }

            static void startTetrisAnimation()
            {
                Random rnd = new Random();


                int maxCapacity = 100; // Ekranda aynı anda en fazla 100 blok olabilir varsayalım
                block[] activeBlocks = new block[maxCapacity];
                int activeCount = 0; // Şu an dizide kaç tane canlı blok var?

                int spawnRate = 0;

                while (continueAnimation)
                {
                    spawnRate++;

                    if (spawnRate > 2 && activeCount < maxCapacity)
                    {
                        int startX = rnd.Next(1, Math.Max(2, width - 2));

                        // Yeni bloğu dizinin sıradaki boş yerine koy
                        activeBlocks[activeCount] = new block { X = startX, Y = 0, Sembol = "[]", color = RandomColor(rnd) };
                        activeCount++; // Sayacı artır

                        spawnRate = 0;
                    }

                    // Döngü activeCount kadar döner (dizinin tamamı kadar değil)
                    for (int i = activeCount - 1; i >= 0; i--)
                    {
                        block currentBlock = activeBlocks[i]; // 'block' ismi çakışmasın diye currentBlock yaptım
                        int nextY = currentBlock.Y + 1;
                        bool hitGround = false;

                        if (nextY >= height - 1 || filledSpaces[currentBlock.X, nextY] || (currentBlock.X + 1 < width && filledSpaces[currentBlock.X + 1, nextY]))
                        {
                            hitGround = true;
                        }

                        if (hitGround)
                        {
                            lock (_lock)
                            {
                                if (currentBlock.X >= 0 && currentBlock.X < width && currentBlock.Y >= 0 && currentBlock.Y < height) filledSpaces[currentBlock.X, currentBlock.Y] = true;
                                if (currentBlock.X + 1 < width && currentBlock.Y >= 0 && currentBlock.Y < height) filledSpaces[currentBlock.X + 1, currentBlock.Y] = true;
                                try { Console.SetCursorPosition(currentBlock.X, currentBlock.Y); Console.ForegroundColor = currentBlock.color; Console.Write(currentBlock.Sembol); }
                                catch { }
                            }


                            activeCount--; // Sayısı bir azalt
                            activeBlocks[i] = activeBlocks[activeCount]; // Sonuncuyu buraya taşı
                            activeBlocks[activeCount] = null; // Sonuncunun eski yerini temizle (Referansı kopar)
                        }
                        else
                        {
                            lock (_lock)
                            {
                                try
                                {
                                    Console.SetCursorPosition(currentBlock.X, currentBlock.Y);
                                    Console.Write("  ");
                                    currentBlock.Y++;
                                    Console.SetCursorPosition(currentBlock.X, currentBlock.Y);
                                    Console.ForegroundColor = currentBlock.color;
                                    Console.Write(currentBlock.Sembol);
                                }
                                catch { }
                            }
                        }
                    }

                    Thread.Sleep(50);
                }
            }

            static ConsoleColor RandomColor(Random rnd)
            {
                ConsoleColor[] colors = { ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta };
                return colors[rnd.Next(colors.Length)];
            }

            class block
            {
                public int X { get; set; }
                public int Y { get; set; }
                public string Sembol { get; set; }
                public ConsoleColor color { get; set; }
            }
        }

        static string PiecesDisplay = "";

        static void Main(string[] args)
        {
            // Show splash screen; pressing Enter continues to the game flow.
            try { SplashScreen.Show(); } catch { /* Splash başarısız olursa oyuna devam et */ }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
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

            // INPUT SECTION: enter pieces one by one with validation (2-12)
            Console.WriteLine("Enter piece square counts (2-12). Enter each value one by one.");
            Console.WriteLine("Press ENTER on an empty line to finish input (minimum 1 piece).");

            int pieceCount = 0;
            int[] userXList = new int[20]; // up to 20 pieces

            while (pieceCount < 20)
            {
                Console.Write($"Piece #{pieceCount + 1} (2-12) or blank to finish: ");
                string line = Console.ReadLine();
                if (line == null) line = "";

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (pieceCount > 0)
                    {
                        // finish input
                        break;
                    }
                    else
                    {
                        Console.WriteLine("At least one piece is required. Please enter a number between 2 and 12.");
                        continue;
                    }
                }

                line = line.Trim();

                int v;
                if (!int.TryParse(line, out v))
                {
                    Console.WriteLine("Invalid input. Please enter an integer number between 2 and 12 (no letters, no symbols).");
                    continue;
                }

                if (v < 2 || v > 12)
                {
                    Console.WriteLine("Value out of range. Each piece must have between 2 and 12 squares.");
                    continue;
                }

                // valid
                userXList[pieceCount++] = v;
            }

            // if user entered none (should not happen due to checks), ask again
            if (pieceCount == 0)
            {
                Console.WriteLine("No valid pieces entered. Please enter at least one piece value (2-12).");
                // fallback to previous bulk input behaviour minimally to avoid blocking forever
                while (pieceCount == 0)
                {
                    Console.Write("Enter piece counts separated by spaces (2-12): ");
                    string line = Console.ReadLine() ?? "";
                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length && pieceCount < 20; i++)
                    {
                        if (int.TryParse(parts[i], out int v) && v >= 2 && v <= 12)
                        {
                            userXList[pieceCount++] = v;
                        }
                    }
                    if (pieceCount == 0) Console.WriteLine("No valid numbers found. Try again.");
                }
            }

            Console.WriteLine("Pieces accepted: " + pieceCount);
            // build display like: 4 4 5 5 6
            var sb = new StringBuilder();
            for (int i = 0; i < pieceCount; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(userXList[i]);
            }
            PiecesDisplay = sb.ToString();

            // Generate pieces (uses per-piece X values)
            for (int pieceIndex = 1; pieceIndex <= pieceCount; pieceIndex++)
            {
                int userX = userXList[pieceIndex - 1]; // squares for this piece

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
            }

            PlayRounds(pieceStorage);

            Console.WriteLine();
            Console.WriteLine("Program finished. Press any key to exit.");
            Console.ReadKey();
        }

        // Modified PlayRounds: auto-quit on round finalization (Q key) supported
        static void PlayRounds(Array[] pieceStorage)
        {
            int round = 1;
            int totalScore = 0;

            double minReq, maxReq;
            while (true)
            {
                // input for min/max regularity
                while (true)
                {
                    Console.Write("Enter min regularity (0.00-1.00) — use '.' as decimal separator (e.g. 0.20): ");
                    string sMin = Console.ReadLine() ?? "";
                    if (!TryParseDotDouble(sMin, out minReq) || minReq < 0.0 || minReq > 1.0)
                    {
                        Console.WriteLine("Min regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                        continue;
                    }

                    Console.Write("Enter max regularity (0.00-1.00) — use '.' as decimal separator (e.g. 0.80): ");
                    string sMax = Console.ReadLine() ?? "";
                    if (!TryParseDotDouble(sMax, out maxReq) || maxReq < 0.0 || maxReq > 1.0)
                    {
                        Console.WriteLine("Max regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                        continue;
                    }

                    if (maxReq <= minReq)
                    {
                        Console.WriteLine("Max regularity must be greater than min regularity (max > min). Please re-enter both values.");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);

                    bool success = PlaySingleRound(pieceStorage, round, minReq, maxReq, out int scoreEarned);

                    if (!success)
                    {
                        Console.WriteLine();
                        if (quitRequested)
                        {
                            // Add the earned score on quit and advance to next round automatically
                            totalScore += scoreEarned;
                            Console.WriteLine("Round finalized by Q. Round Score: " + scoreEarned + " | Total Score: " + totalScore);
                            quitRequested = false; // reset for next rounds
                            round++;               // advance round
                            // keep current minReq/maxReq unless user wants to change — prompt below
                            Console.WriteLine("Set new regularity interval for next round:");
                            while (true)
                            {
                                Console.Write("Min (0.00-1.00) — use '.' as decimal separator: ");
                                string sMin = Console.ReadLine() ?? "";
                                if (!TryParseDotDouble(sMin, out minReq) || minReq < 0.0 || minReq > 1.0)
                                {
                                    Console.WriteLine("Min regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                                    continue;
                                }

                                Console.Write("Max (0.00-1.00) — use '.' as decimal separator: ");
                                string sMax = Console.ReadLine() ?? "";
                                if (!TryParseDotDouble(sMax, out maxReq) || maxReq < 0.0 || maxReq > 1.0)
                                {
                                    Console.WriteLine("Max regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                                    continue;
                                }

                                if (maxReq <= minReq)
                                {
                                    Console.WriteLine("Max regularity must be greater than min regularity (max > min). Please re-enter both values.");
                                    continue;
                                }
                                break;
                            }

                            // continue outer while(true) to start next round immediately
                            continue;
                        }
                        else if (giveUpRequested)
                        {
                            // ESC pressed: show total score and exit
                            Console.WriteLine("You gave up. Total Score: " + totalScore);
                            giveUpRequested = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Game Over. Final Score: " + totalScore);
                            break;
                        }
                    }

                    totalScore += scoreEarned;
                    Console.WriteLine();
                    Console.WriteLine("Round completed! Round Score: " + scoreEarned + " | Total Score: " + totalScore);

                    Console.WriteLine("Set new regularity interval for next round:");
                    while (true)
                    {
                        Console.Write("Min (0.00-1.00) — use '.' as decimal separator: ");
                        string sMin = Console.ReadLine() ?? "";
                        if (!TryParseDotDouble(sMin, out minReq) || minReq < 0.0 || minReq > 1.0)
                        {
                            Console.WriteLine("Min regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                            continue;
                        }

                        Console.Write("Max (0.00-1.00) — use '.' as decimal separator: ");
                        string sMax = Console.ReadLine() ?? "";
                        if (!TryParseDotDouble(sMax, out maxReq) || maxReq < 0.0 || maxReq > 1.0)
                        {
                            Console.WriteLine("Max regularity invalid. Use digits and optional '.' only. Value must be between 0.00 and 1.00.");
                            continue;
                        }

                        if (maxReq <= minReq)
                        {
                            Console.WriteLine("Max regularity must be greater than min regularity (max > min). Please re-enter both values.");
                            continue;
                        }
                        break;
                    }

                    Console.WriteLine("Continue? (Y/N)");
                    char ch = char.ToUpperInvariant(Console.ReadKey(true).KeyChar);
                    if (ch != 'Y') break;
                    round++;
                }

                break;
            }
        }

        // New helper: parses dot-separated decimal values, rejects ',' or letters/special characters.
        static bool TryParseDotDouble(string s, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();

            // Virgül kullanımı yasak
            if (s.Contains(',')) return false;

            // Sadece rakamlar ve en fazla bir '.' olmalı, ve '.' varsa sağında en az 1 hane olmalı
            // Kabul edilen örnekler: "0", "1", "0.2", "0.20", "1.00"
            if (!Regex.IsMatch(s, @"^\d+(\.\d+)?$")) return false;

            // InvariantCulture ile parse et
            if (!double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value)) return false;

            // negatif değeri reddet
            if (value < 0.0) return false;

            return true;
        }

        // ✅ CONTROLS/STATUS view preserved, UI won't jump: static part is printed once, grid+status rows are updated in place.
        static bool PlaySingleRound(Array[] pieceStorage, int roundNumber, double minReq, double maxReq, out int roundScore)
        {
            roundScore = 0;
            int puzzleRows = 20;
            int puzzleCols = 30;

            char[,] target;
            bool[,] targetMask;
            int totalSquares;
            int perimeter;

            // Use a single Random instance and let BuildPuzzle attempt many random builds internally
            Random r = new Random();
            target = BuildPuzzle(pieceStorage, puzzleRows, puzzleCols, 160, minReq, maxReq, r, out totalSquares, out perimeter);

            // compute regularity of assembled puzzle to confirm
            double side = perimeter / 4.0;
            double denom = side * side;
            double reg = denom == 0 ? 0 : totalSquares / denom;

            // If BuildPuzzle could not produce a puzzle inside the requested interval, indicate failure
            if (!(reg >= minReq && reg <= maxReq))
            {
                // Could not build a puzzle matching requested regularity
                return false;
            }

            targetMask = ToBoolGrid(target);

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

            // initial selection
            selectedIndex = 1;
            if (selectedIndex >= 1 && selectedIndex < pieceStorage.Length && pieceStorage[selectedIndex] != null)
            {
                working = CloneGrid((bool[,])pieceStorage[selectedIndex]);
                working = NormalizeShift(working);
                ghostRow = 0; ghostCol = 0;
            }

            // min/max regularity (once)
            CalculateMinMaxRegularity(pieceStorage, out double minRd, out double maxRd);

            // Target total (constant)
            int targetTotalConst = 0;
            for (int i = 0; i < puzzleRows; i++)
                for (int j = 0; j < puzzleCols; j++)
                    if (targetMask[i, j]) targetTotalConst++;

            bool prevCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            // Prepare + clear console for the game screen
            PrepareConsoleForGame(puzzleRows, puzzleCols);
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            // ===== TOP INFO BOX =====
            int preferredWidth = (2 + puzzleCols + 2) + (5 * 8) + 4; // grid + gap + palette + margin
            int boxWidth = preferredWidth;
            try { boxWidth = Math.Min(Console.WindowWidth - 1, preferredWidth); } catch { }
            if (boxWidth < 40) boxWidth = 40;

            // Pass actual regularity to show under min/max
            PrintTopInfoBox(roundNumber, PiecesDisplay, minReq, maxReq, reg, boxWidth);
            Console.WriteLine(); // box sonrası 1 satır boşluk

            // REMOVED: Column header (junk text)
            // Console.Write("  ");
            // for (int c = 0; c < puzzleCols; c++)
            //     Console.Write(c % 2 == 1 ? (char)('0' + ((c + 1) % 10)) : ' ');
            // Console.WriteLine();

            // Grid start row
            int gridStartRow = Console.CursorTop;

            // Print the grid area once as a placeholder (then update with SetCursorPosition each loop)
            for (int r2 = 0; r2 < puzzleRows; r2++)
            {
                // REMOVED: row prefixes (junk text)
                // if (r2 == 0) Console.Write("  ");
                // else { Console.Write(r2 % 2 == 1 ? (((r2 / 2 + 1) * 2) % 10).ToString() : " "); Console.Write(' '); }

                for (int c = 0; c < puzzleCols; c++) Console.Write('.');
                Console.WriteLine();
            }

            // Palette (to the right) -> draw once
            int paletteStartX = 2 + puzzleCols + 2;
            int paletteStartY = gridStartRow;
            for (int p = 1; p < pieceStorage.Length; p++)
            {
                if (pieceStorage[p] == null) continue;
                try { PositionPrint((bool[,])pieceStorage[p], p, paletteStartX, paletteStartY); }
                catch { }
            }

            // Move below the grid
            int afterGridRow = gridStartRow + puzzleRows;
            try { Console.SetCursorPosition(0, afterGridRow); } catch { }

            // CONTROLS + STATUS (original view)
            Console.WriteLine();
            Console.WriteLine("CONTROLS");
            Console.WriteLine("  Key(s)         | Action");
            Console.WriteLine("  ----------------+------------------------------------------------");
            Console.WriteLine("  Arrows         | Move preview (if active) / Move selected placed piece / Move cursor");
            Console.WriteLine("  A - T          | Select piece (A=1 .. T=20)");
            Console.WriteLine("  *              | Rotate preview");
            Console.WriteLine("  -              | Flip preview (mirror)");
            Console.WriteLine("  Enter          | Place preview (when active)");
            Console.WriteLine("  Backspace / Del| Remove piece under cursor");
            Console.WriteLine("  U              | Undo last placement");
            Console.WriteLine("  Q              | Quit game (finalize score)");
            Console.WriteLine("  Esc            | Give up (show total score and exit)");
            Console.WriteLine();
            Console.WriteLine("STATUS");

            int ySelected = Console.CursorTop; Console.WriteLine("  Selected Piece : -");
            int yCursor = Console.CursorTop; Console.WriteLine("  Cursor (R,C)   : 0,0");
            int yPlaced = Console.CursorTop; Console.WriteLine("  Placed cells   : 0    Target cells: " + targetTotalConst);
            int yProgress = Console.CursorTop; Console.WriteLine("  Progress       : 0%");
            int yPreview = Console.CursorTop; Console.WriteLine("  Preview valid  : NO");
            Console.WriteLine();

            int inputRow = Console.CursorTop;

            // ===== Dynamic draw helpers =====
            void WriteTextAt(int x, int y, string text)
            {
                try { Console.SetCursorPosition(x, y); } catch { return; }

                int width = Console.WindowWidth;
                if (width <= 0) { Console.Write(text); return; }

                int max = Math.Max(0, width - x);
                if (text.Length > max) text = text.Substring(0, max);

                Console.Write(text);

                int pad = max - text.Length;
                if (pad > 0) Console.Write(new string(' ', pad));
            }

            void DrawGrid(bool[,] ghostMask, int selIdx)
            {
                var defaultColor = Console.ForegroundColor;

                for (int r3 = 0; r3 < puzzleRows; r3++)
                {
                    try { Console.SetCursorPosition(0, gridStartRow + r3); } catch { return; }

                    // REMOVED: row prefix print to avoid left-side junk
                    // if (r3 == 0) Console.Write("  ");
                    // else { Console.Write(r3 % 2 == 1 ? (((r3 / 2 + 1) * 2) % 10).ToString() : " "); Console.Write(' '); }

                    for (int c = 0; c < puzzleCols; c++)
                    {
                        // ghost
                        if (ghostMask != null && ghostMask[r3, c] && selIdx >= 1 && selIdx <= 20)
                        {
                            char letter = (char)('A' + selIdx - 1);
                            Console.ForegroundColor = GetPieceColor(selIdx);
                            Console.Write(letter);
                            Console.ForegroundColor = defaultColor;
                            continue;
                        }

                        char ch = player[r3, c];
                        if (ch != '\0')
                        {
                            Console.ForegroundColor = GetPieceColor(ch);
                            Console.Write(ch);
                            Console.ForegroundColor = defaultColor;
                        }
                        else if (target[r3, c] != '\0')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('X');
                            Console.ForegroundColor = defaultColor;
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }
                }
            }

            void UpdateStatus(bool previewValid, int covered, int percent)
            {
                // Selected Piece (colored)
                try { Console.SetCursorPosition(0, ySelected); } catch { return; }
                Console.Write("  Selected Piece : ");
                if (selectedIndex == -1)
                {
                    Console.Write('-');
                }
                else
                {
                    var prev = Console.ForegroundColor;
                    Console.ForegroundColor = GetPieceColor(selectedIndex);
                    Console.Write((char)('A' + selectedIndex - 1));
                    Console.ForegroundColor = prev;
                }
                ClearToEndOfLine();

                WriteTextAt(0, yCursor, $"  Cursor (R,C)   : {cursorR},{cursorC}");
                WriteTextAt(0, yPlaced, $"  Placed cells   : {covered}    Target cells: {targetTotalConst}");
                WriteTextAt(0, yProgress, $"  Progress       : {percent}%");
                WriteTextAt(0, yPreview, $"  Preview valid  : {(previewValid ? "YES" : "NO")}");
            }

            try
            {
                while (true)
                {
                    // ghost + validity
                    bool previewValid = false;
                    bool[,] ghostMask = null;

                    if (working != null && selectedIndex != -1)
                    {
                        previewValid = CanPlaceOnTarget(player, targetMask, working, ghostRow, ghostCol);

                        GetBounds(working, out int wMinR, out int wMinC, out int wMaxR, out int wMaxC);
                        ghostMask = new bool[puzzleRows, puzzleCols];
                        for (int i = 0; i < 5; i++)
                            for (int j = 0; j < 5; j++)
                                if (working[i, j])
                                {
                                    int rr = ghostRow + (i - wMinR);
                                    int cc = ghostCol + (j - wMinC);
                                    if (rr >= 0 && rr < puzzleRows && cc >= 0 && cc < puzzleCols)
                                        ghostMask[rr, cc] = true;
                                }
                    }

                    // covered + percent
                    int covered = 0;
                    for (int i = 0; i < puzzleRows; i++)
                        for (int j = 0; j < puzzleCols; j++)
                            if (player[i, j] != '\0') covered++;

                    int percent = targetTotalConst == 0 ? 0 : (int)Math.Round(covered * 100.0 / targetTotalConst);

                    // sadece grid + status güncelle (controls aynen kalır)
                    int selIdxForGhost = (selectedIndex >= 1) ? selectedIndex : 1;
                    DrawGrid(ghostMask, selIdxForGhost);
                    UpdateStatus(previewValid, covered, percent);

                    // input satırına dön (scroll olmasın)
                    try { Console.SetCursorPosition(0, inputRow); } catch { }
                    ClearToEndOfLine();

                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Global quit
                    if (key.Key == ConsoleKey.Q)
                    {
                        // finalize current round score based on current player state
                        int placedCells = 0;
                        for (int i = 0; i < puzzleRows; i++)
                            for (int j = 0; j < puzzleCols; j++)
                                if (player[i, j] != '\0') placedCells++;

                        // Compute regularity from current player board (same perimeter logic)
                        bool[,] playerMask = ToBoolGrid(player);
                        ComputeTotals(playerMask, out int tsComputed, out int perComputed);
                        double sidea = perComputed / 4.0;
                        double denoma = sidea * sidea;
                        double regularity = denoma == 0 ? 0.0 : tsComputed / denoma;

                        // Base score (unchanged)
                        double scoreD = tsComputed * Math.Pow(4.0, regularity * 4.0);

                        // Apply requested formula: score = score / (progress / 100)
                        // 'percent' is computed earlier in the loop from covered/targetTotalConst
                        double progress = percent / 100.0;
                        if (progress <= 0) progress = 0.000001; // avoid divide-by-zero when no progress yet

                        scoreD = scoreD / progress / 100.0;

                        roundScore = (int)Math.Round(scoreD);

                        quitRequested = true;
                        return false;
                    }

                    // ESC: show total score (handled at PlayRounds level), do not add any round score
                    if (key.Key == ConsoleKey.Escape)
                    {
                        giveUpRequested = true;
                        roundScore = 0;
                        return false;
                    }

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

                    // Place preview (Enter)
                    if (key.Key == ConsoleKey.Enter && working != null && selectedIndex != -1)
                    {
                        char letter = (char)('A' + selectedIndex - 1);
                        if (TryPlaceOnTarget(player, targetMask, working, ghostRow, ghostCol, letter))
                        {
                            placed[selectedIndex] = true;
                            placedGrid[selectedIndex] = CloneGrid(working);
                            placedRow[selectedIndex] = ghostRow;
                            placedCol[selectedIndex] = ghostCol;
                            hasState[selectedIndex] = true;
                            dirty[selectedIndex] = false;
                            history[histCount++] = letter;
                            working = null;
                        }
                        continue;
                    }

                    // Spacebar disabled per request (ignore)
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        continue;
                    }

                    // Remove (Backspace/Delete)
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

                    // Undo
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

                    // Finish round with Enter disabled if no preview
                    if (key.Key == ConsoleKey.Enter && working == null)
                    {
                        continue;
                    }
                }
            }
            finally
            {
                Console.CursorVisible = prevCursorVisible;
            }
        }

        // ===== TOP BOX HELPERS (NEW) =====

        static void PrintTopInfoBox(int roundNumber, string piecesDisplay, double minReg, double maxReg, double actualReg, int width)
        {
            if (width < 30) width = 30;
            int inner = width - 2;

            string border = "+" + new string('-', inner) + "+";
            Console.WriteLine(border);

            WriteBoxKeyValue("Round:", roundNumber.ToString(), width);
            WriteBoxKeyValue("Pieces:", piecesDisplay ?? "", width);
            WriteBoxKeyValue("Min. Regularity:", minReg.ToString("F2", CultureInfo.InvariantCulture), width);
            WriteBoxKeyValue("Max. Regularity:", maxReg.ToString("F2", CultureInfo.InvariantCulture), width);
            // NEW: exact actual regularity
            WriteBoxKeyValue("Actual Regularity:", actualReg.ToString("F4", CultureInfo.InvariantCulture), width);

            Console.WriteLine(border);
        }

        static void WriteBoxKeyValue(string key, string value, int width)
        {
            int inner = width - 2;
            string prefix = key + " ";
            if (prefix.Length > inner) prefix = prefix.Substring(0, inner);

            int maxFirst = inner - prefix.Length;
            if (maxFirst < 0) maxFirst = 0;

            var lines = WrapTextBySpaces(value ?? "", maxFirst > 0 ? maxFirst : inner);
            if (lines.Count == 0) lines.Add("");

            // first line
            string first = prefix + lines[0];
            if (first.Length > inner) first = first.Substring(0, inner);
            Console.WriteLine("|" + first.PadRight(inner) + "|");

            // subsequent lines align under value area
            string indent = new string(' ', prefix.Length);
            for (int i = 1; i < lines.Count; i++)
            {
                string line = indent + lines[i];
                if (line.Length > inner) line = line.Substring(0, inner);
                Console.WriteLine("|" + line.PadRight(inner) + "|");
            }
        }

        static List<string> WrapTextBySpaces(string text, int maxWidth)
        {
            var result = new List<string>();
            if (maxWidth <= 0) { result.Add(""); return result; }

            text = (text ?? "").Trim();
            if (text.Length == 0) { result.Add(""); return result; }

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var w in words)
            {
                if (sb.Length == 0)
                {
                    if (w.Length <= maxWidth) sb.Append(w);
                    else
                    {
                        // hard cut if a single "word" is longer than maxWidth
                        result.Add(w.Substring(0, maxWidth));
                        string rest = w.Substring(maxWidth);
                        while (rest.Length > 0)
                        {
                            int take = Math.Min(maxWidth, rest.Length);
                            result.Add(rest.Substring(0, take));
                            rest = rest.Substring(take);
                        }
                    }
                }
                else
                {
                    if (sb.Length + 1 + w.Length <= maxWidth)
                    {
                        sb.Append(' ').Append(w);
                    }
                    else
                    {
                        result.Add(sb.ToString());
                        sb.Clear();

                        if (w.Length <= maxWidth) sb.Append(w);
                        else
                        {
                            result.Add(w.Substring(0, maxWidth));
                            string rest = w.Substring(maxWidth);
                            while (rest.Length > 0)
                            {
                                int take = Math.Min(maxWidth, rest.Length);
                                result.Add(rest.Substring(0, take));
                                rest = rest.Substring(take);
                            }
                        }
                    }
                }
            }

            if (sb.Length > 0) result.Add(sb.ToString());
            return result;
        }

        // ===== HELPER METHODS =====

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

        static void PositionPrint(bool[,] grid, int pieceIndex, int startX, int startY)
        {
            int columnIndex = (pieceIndex - 1) % 5;
            int rowIndex = (pieceIndex - 1) / 5;
            int baseX = startX + columnIndex * 8;
            int baseY = startY + rowIndex * 6;
            char letter = (char)('A' + (pieceIndex - 1));
            for (int i = 0; i < 5; i++)
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

        // Modified BuildPuzzle:
        // - accepts min/max regularity and a Random instance
        // - tries multiple random constructions (shuffle piece order + random placements)
        // - returns a board that matches requested regularity interval when possible
        static char[,] BuildPuzzle(Array[] pieceStorage, int rows, int cols, int maxSquares, double minReq, double maxReq, Random r, out int totalSquares, out int perimeter)
        {
            totalSquares = 0;
            perimeter = 0;
            char[,] bestBoard = new char[rows, cols];
            int bestTS = 0, bestPer = 0;
            double bestDiff = double.MaxValue;

            int attemptsLimit = 400; // internal attempts to find a matching puzzle

            // collect available piece indices once
            var available = new List<int>();
            for (int i = 1; i < pieceStorage.Length; i++) if (pieceStorage[i] is bool[,]) available.Add(i);

            if (available.Count == 0)
            {
                // nothing to place
                return bestBoard;
            }

            for (int attempt = 0; attempt < attemptsLimit; attempt++)
            {
                char[,] board = new char[rows, cols];
                int ts = 0;
                bool anyPlaced = false;

                // shuffle piece order (Fisher-Yates)
                var order = new List<int>(available);
                for (int i = order.Count - 1; i > 0; i--)
                {
                    int j = r.Next(i + 1);
                    int tmp = order[i]; order[i] = order[j]; order[j] = tmp;
                }

                foreach (int i in order)
                {
                    if (!(pieceStorage[i] is bool[,] src)) continue;
                    bool[,] work = CloneGrid(src);
                    if (r.Next(0, 2) == 1) ReversePiece(work);
                    int rot = r.Next(0, 4);
                    for (int k = 0; k < rot; k++) RotatePiece(work);
                    work = NormalizeShift(work);
                    int pieceSquares = CountSquares(work);
                    if (pieceSquares == 0) continue;
                    if (ts + pieceSquares > maxSquares) continue;
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
                        if (!anyPlaced && !placedFlag) { }
                        else if (!touches) continue;

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
                        ts += pieceSquares;
                    }
                }

                // Evaluate regularity of constructed board
                bool[,] mask = ToBoolGrid(board);
                ComputeTotals(mask, out int tsComputed, out int perComputed);
                double side = perComputed / 4.0;
                double denom = side * side;
                double reg = denom == 0 ? 0 : tsComputed / denom;

                if (reg >= minReq && reg <= maxReq && tsComputed > 0)
                {
                    totalSquares = tsComputed;
                    perimeter = perComputed;
                    return board;
                }

                // track best (closest) result to interval
                double diff;
                if (reg < minReq) diff = minReq - reg;
                else diff = reg - maxReq;

                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestBoard = board;
                    bestTS = tsComputed;
                    bestPer = perComputed;
                }
            }

            // No exact match found: return the closest candidate
            totalSquares = bestTS;
            perimeter = bestPer;
            return bestBoard;
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

        // ===== UI / CONSOLE STABILITY HELPERS =====

        static void PrepareConsoleForGame(int puzzleRows, int puzzleCols)
        {
            // FULL UI: header + grid + controls + status
            int estimatedHeight = puzzleRows + 35;
            int estimatedWidth = (2 + puzzleCols + 2) + (5 * 8) + 2; // grid + space + palette
            TryResizeConsole(estimatedWidth, estimatedHeight);
        }

        static void TryResizeConsole(int width, int height)
        {
            try
            {
                width = Math.Min(width, Console.LargestWindowWidth);
                height = Math.Min(height, Console.LargestWindowHeight);

                // Buffer must be >= Window
                int bw = Math.Max(Console.BufferWidth, width);
                int bh = Math.Max(Console.BufferHeight, height);
                if (bw != Console.BufferWidth || bh != Console.BufferHeight)
                    Console.SetBufferSize(bw, bh);

                if (Console.WindowWidth != width || Console.WindowHeight != height)
                    Console.SetWindowSize(width, height);

                Console.WindowTop = 0;
                Console.WindowLeft = 0;
            }
            catch
            {
                // Some terminals don't allow resizing; that's fine.
            }
        }

        static void ClearToEndOfLine()
        {
            try
            {
                int curX = Console.CursorLeft;
                int curY = Console.CursorTop;
                int remaining = Console.WindowWidth - curX;
                if (remaining > 0) Console.Write(new string(' ', remaining));
                Console.SetCursorPosition(curX, curY);
            }
            catch { }
        }
    }
}