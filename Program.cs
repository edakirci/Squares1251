using System;

namespace Squares
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool flag = true;
            int Xcount = 0;
            int userChoice = 0;
            bool Valid = true;
            Random random = new Random();
            int Rowloc = 0;
            int Colloc = 0;
            int col;
            int row;

            // Parça dizilerini tanımla
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

            // Depolama dizisi
            Array[] pieceStorage = new Array[21]
            {
                null,
                piece1, piece2, piece3, piece4, piece5, piece6, piece7, piece8, piece9, piece10,
                piece11, piece12, piece13, piece14, piece15, piece16, piece17, piece18, piece19, piece20
            };

            bool[,] tempPiece = new bool[5, 5];

            Console.WriteLine("Chose X numbering 2-12");
            int userX = Convert.ToInt32(Console.ReadLine());

            while (userChoice == 0)
            {
                Valid = true;

                Console.Write("Pieces number (1-20): ");
                int pieceCount = Convert.ToInt32(Console.ReadLine());
                if (pieceCount > 20) pieceCount = 20;

                for (int pieceIndex = 1; pieceIndex <= pieceCount; pieceIndex++)
                {
                    bool isUnique = false;
                    bool[,] currentPiece = new bool[5, 5];
                    int attemptCount = 0; // DENEME SAYACI (SONSUZ DÖNGÜYÜ ENGELLEMEK İÇİN)
                    bool gaveUp = false;

                    // --- UNIQUE PARÇA BULANA KADAR DÖNECEK DÖNGÜ ---
                    do
                    {
                        attemptCount++;
                        // Eğer 2000 denemede yeni parça bulamazsa pes etsin
                        if (attemptCount > 2000)
                        {
                            Console.WriteLine("\n[UYARI] " + userX + " kare ile daha fazla EŞSİZ (Unique) parça bulunamadı.");
                            Console.WriteLine("Matematiksel sınıra ulaşmış olabilirsiniz.");
                            Console.WriteLine("Şu ana kadar bulunan parça sayısı: " + (pieceIndex - 1));
                            gaveUp = true;
                            break;
                        }

                        // 1. Matrisi Sıfırla
                        currentPiece = new bool[5, 5];
                        flag = true;
                        Xcount = 0;

                        // 2. İlk X'i yerleştir
                        do
                        {
                            Xcount = 0;
                            for (row = 0; row < 5; row++)
                            {
                                for (col = 0; col < 5; col++)
                                {
                                    if (flag) currentPiece[row, col] = random.Next(0, 24) == 0;
                                    else currentPiece[row, col] = false;

                                    if (currentPiece[row, col]) Xcount++;

                                    if (Xcount == 1 && flag)
                                    {
                                        Rowloc = row;
                                        Colloc = col;
                                    }

                                    if (Xcount >= 1) flag = false;
                                }
                            }
                        }
                        while (Xcount == 0);

                        // 3. Random Walk (Büyütme)
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

                        // 4. Normalize Et
                        currentPiece = NormalizeShift(currentPiece);
                        currentPiece = NormalizeShift(currentPiece); // Garanti olsun diye

                        // 5. COMPARISON (KARŞILAŞTIRMA) KISMI
                        if (CheckIfDuplicate(currentPiece, pieceStorage, pieceIndex))
                        {
                            isUnique = false;
                        }
                        else
                        {
                            isUnique = true;
                        }

                    } while (!isUnique);

                    // Eğer pes ettiyse dıştaki döngüyü de kırıp bitir
                    if (gaveUp)
                    {
                        break;
                    }

                    // Eşsiz parça bulundu, kaydet
                    pieceStorage[pieceIndex] = currentPiece;
                    PrintPiece(currentPiece);
                    Console.WriteLine("Piece " + pieceIndex + " accepted (Unique).");
                }

                Console.WriteLine("Be carefull");
                Console.WriteLine();
                Console.WriteLine("0=restart, 1=call back");

                try
                {
                    userChoice = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    userChoice = 0;
                }
            }

            if (userChoice == 1)
            {
                Console.Write("which one: ");
                int selectedPieceIndex = Convert.ToInt32(Console.ReadLine());

                if (selectedPieceIndex >= 1 && selectedPieceIndex < pieceStorage.Length && pieceStorage[selectedPieceIndex] != null)
                {
                    tempPiece = (bool[,])pieceStorage[selectedPieceIndex];
                    PrintPiece(tempPiece);

                    do
                    {
                        Console.WriteLine("Do you want apply any function to this piece");
                        Console.WriteLine("Be carefull");
                        Console.WriteLine("Y/N");
                        char entryChoice = Console.ReadKey().KeyChar;
                        Console.WriteLine();

                        if (entryChoice == 'Y' || entryChoice == 'y')
                        {
                            Console.WriteLine("chose your function");
                            Console.WriteLine("1.Rotate");
                            Console.WriteLine("2.Reverse");
                            char entryChoice2 = Console.ReadKey().KeyChar;
                            Console.WriteLine();

                            if (entryChoice2 == '1')
                            {
                                RotatePiece(tempPiece);
                                PrintPiece(tempPiece);
                            }
                            else if (entryChoice2 == '2')
                            {
                                ReversePiece(tempPiece);
                                PrintPiece(tempPiece);
                            }
                            else Console.WriteLine("Invalid input");
                        }

                        Console.WriteLine();
                        Console.WriteLine("Again?");
                        Console.Write("Y/N: ");
                        entryChoice = Console.ReadKey().KeyChar;
                        Console.WriteLine();

                        if (entryChoice == 'Y' || entryChoice == 'y') Valid = true; else Valid = false;
                    }
                    while (Valid);
                }
                else
                {
                    Console.WriteLine("Geçersiz parça numarası veya parça mevcut değil.");
                }

                userChoice = 0;
            }

            Console.WriteLine("Program finished. Press any key to exit.");
            Console.ReadKey();
        }

        // --- COMPARISON VE YARDIMCI METOTLAR ---

        // Yeni parça, eskilerden herhangi birinin döndürülmüş haliyle aynı mı?
        static bool CheckIfDuplicate(bool[,] newPiece, Array[] storage, int currentIndex)
        {
            // Henüz hiç parça üretilmediyse (index 1 ise) kopya olamaz
            if (currentIndex <= 1) return false;

            // Mevcut parçanın kopyasını al ki döndürürken bozulmasın
            bool[,] checkPiece = CloneGrid(newPiece);

            // 4 Kere döndürerek kontrol et (0, 90, 180, 270 derece)
            for (int rot = 0; rot < 4; rot++)
            {
                // checkPiece'i normalize et (çünkü rotate edince kayabilir)
                checkPiece = NormalizeShift(checkPiece);

                // Hafızadaki önceki tüm parçalarla karşılaştır
                for (int i = 1; i < currentIndex; i++)
                {
                    if (storage[i] != null) // Null kontrolü ekledim
                    {
                        bool[,] existingPiece = (bool[,])storage[i];
                        if (AreMatricesEqual(checkPiece, existingPiece))
                        {
                            return true; // Eşleşme bulundu, bu bir kopyadır!
                        }
                    }
                }

                // Parçayı bir sonraki kontrol için 90 derece döndür
                RotatePiece(checkPiece);
            }

            return false; // Hiçbir eşleşme yok, parça unique
        }

        // İki 5x5 matrisin birebir aynı olup olmadığını kontrol eder
        static bool AreMatricesEqual(bool[,] p1, bool[,] p2)
        {
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (p1[r, c] != p2[r, c]) return false;
                }
            }
            return true;
        }

        // Matrisin derin kopyasını oluşturur (Referans hatası olmasın diye)
        static bool[,] CloneGrid(bool[,] source)
        {
            bool[,] dest = new bool[5, 5];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    dest[r, c] = source[r, c];
                }
            }
            return dest;
        }

        static bool[,] NormalizeShift(bool[,] grid)
        {
            bool StopX = false;
            bool StopY = false;
            int emptyRowCount = 0;
            int emptyColumnCount = 0;

            for (int i = 0; i < 5; i++)
            {
                bool rowIsEmpty = true;
                for (int k = 0; k < 5; k++)
                {
                    if (grid[i, k] == true)
                    {
                        rowIsEmpty = false;
                        StopX = true;
                        break;
                    }
                }

                if (!StopX && rowIsEmpty) emptyRowCount++;
                else StopX = true;
            }

            for (int k = 0; k < 5; k++)
            {
                bool colIsEmpty = true;
                for (int i = 0; i < 5; i++)
                {
                    if (grid[i, k] == true)
                    {
                        colIsEmpty = false;
                        StopY = true;
                        break;
                    }
                }

                if (!StopY && colIsEmpty) emptyColumnCount++;
                else StopY = true;
            }

            int shiftUp = emptyRowCount;
            if (shiftUp > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (i + shiftUp < 5) grid[i, k] = grid[i + shiftUp, k];
                        else grid[i, k] = false;
                    }
                }
            }

            int shiftLeft = emptyColumnCount;
            if (shiftLeft > 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (k + shiftLeft < 5) grid[i, k] = grid[i, k + shiftLeft];
                        else grid[i, k] = false;
                    }
                }
            }

            return grid;
        }

        static void RotatePiece(bool[,] grid)
        {
            bool[,] rotated = new bool[5, 5];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    rotated[c, 4 - r] = grid[r, c];
                }
            }
            rotated = NormalizeShift(rotated);
            for (int i = 0; i < 5; i++)
                for (int k = 0; k < 5; k++)
                    grid[i, k] = rotated[i, k];
        }

        static void ReversePiece(bool[,] grid)
        {
            bool[,] reversed = new bool[5, 5];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    reversed[r, 4 - c] = grid[r, c];
                }
            }
            reversed = NormalizeShift(reversed);
            for (int i = 0; i < 5; i++)
                for (int k = 0; k < 5; k++)
                    grid[i, k] = reversed[i, k];
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