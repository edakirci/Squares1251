using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
bool flag = true;
int Xcount = 0;
int userChoice = 0;
bool Valid = true;
bool isAcceptable = false;
int emptyRowCount = 0;
int emptyColumnCount = 0;
Random random = new Random();
int[] Direction = { -2, -1, 1, 2 };
int Rowloc = 0;
int Colloc = 0;
int col;
int row;

// -2 = left, -1 = down, 1 = up, 2 = right


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

Console.WriteLine("Chose X numbering 2-12");
int userX = Convert.ToInt16(Console.ReadLine());
while (userChoice == 0)
{
    Valid = true;
    bool[,] tempPiece = new bool[5, 5];
    Console.Write("Pieces number:");
    int pieceCount = Convert.ToInt16(Console.ReadLine());


    // Console.WriteLine("!isAcceptable"); // debug line-----------------------------------------------------------------
    for (int pieceIndex = 1; pieceIndex <= pieceCount; pieceIndex++)
    {
        // Console.WriteLine("Generating piece " + pieceIndex); // debug line-----------------------------------------------------------------

        bool[,] currentPiece = new bool[5, 5];

        flag = true;
        Xcount = 0;


        do
        {
            for (row = 0; row < 5; row++)
            {
                for (col = 0; col < 5; col++)
                {
                    random = new Random();
                    if (flag)
                        currentPiece[row, col] = random.Next(0, 24) == 0; // true = X, false = .
                    else currentPiece[row, col] = false;
                    if (currentPiece[row, col]) Xcount++;
                    if (Xcount == 1 && flag)
                    {
                        Rowloc = row;
                        Colloc = col;
                    }
                    if (Xcount >= 1)
                    {
                        flag = false;
                    }
                }
            }
            //DebugPiece(currentPiece, Rowloc, Colloc);
            //Console.Read();
        }
        while (Xcount == 0);
        //PrintPiece(currentPiece);
        //Console.ReadKey(); // pause to view initial piece

        for (int i = 0, N = 0; i < (userX - 1 + N); i++)
        {

            int Step = random.Next(-2, 3);
            if (Step == -2 && Colloc > 0)
            {
                if (!currentPiece[Rowloc, Colloc - 1])
                    currentPiece[Rowloc, Colloc - 1] = true;
                else { N++; }
                Colloc = Colloc - 1;
            }
            else if (Step == -1 && Rowloc < 4)
            {
                if (!currentPiece[Rowloc + 1, Colloc])
                    currentPiece[Rowloc + 1, Colloc] = true;
                else { N++; }
                Rowloc = Rowloc + 1;
            }
            else if (Step == 1 && Rowloc > 0)
            {
                if (!currentPiece[Rowloc - 1, Colloc])
                    currentPiece[Rowloc - 1, Colloc] = true;
                else { N++; }
                Rowloc = Rowloc - 1;
            }
            else if (Step == 2 && Colloc < 4)
            {
                if (!currentPiece[Rowloc, Colloc + 1])
                    currentPiece[Rowloc, Colloc + 1] = true;
                else { N++; }
                Colloc = Colloc + 1;
            }
            else if (Step == 0) { N++; }
            else { N++; }
            //DebugPiece(currentPiece, Rowloc, Colloc);
            //Console.Read();
        }

        //Console.WriteLine("normalize baslar"); // debug line-----------------------------------------------------------------
                                               // normalize / shift to top-left (senin eski shift mantığını fonksiyonlaştırdık)
        currentPiece = NormalizeShift(currentPiece);
        //PrintPiece(currentPiece); // debug line to see normalized piece -----------------------------------------------------------------
        if (emptyRowCount > 0 || emptyColumnCount > 0)    
        {
            currentPiece = NormalizeShift(currentPiece);

        }


        pieceStorage[pieceIndex] = currentPiece;
        PrintPiece(currentPiece);


        Console.WriteLine("Piece " + pieceIndex + " accepted.");



    }



    Console.WriteLine("Be carefull");
    Console.ReadLine();
    Console.WriteLine();
    Console.WriteLine("Be carefull");
    Console.WriteLine("0=restart, 1=call back");
    userChoice = Convert.ToInt16(Console.ReadLine());
    if (userChoice == 1)
    {
        Console.Write("which one: ");
        int selectedPieceIndex = Convert.ToInt16(Console.ReadLine());
        tempPiece = (bool[,])pieceStorage[selectedPieceIndex];
        PrintPiece(tempPiece);
        do
        {

            Console.WriteLine("Do you want apply any function to this piece");
            Console.WriteLine("Be carefull");
            Console.WriteLine("Y/N");
            char entryChoice = Console.ReadKey().KeyChar;

            if (entryChoice == 'Y' || entryChoice == 'y')
            {
                Console.WriteLine();
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
                else Console.Write("Invalid input");

            }
            Console.WriteLine();
            Console.WriteLine("Again?");
            Console.Write("Y/N");
            entryChoice = Console.ReadKey().KeyChar;
            if (entryChoice == 'Y' || entryChoice == 'y') Valid = true; else Valid = false;
        }
        while (Valid);
        userChoice = 0;
    }
}






static bool[,] NormalizeShift(bool[,] grid)
{
    bool StopX = false; //false = shift devam, true= shift dur
    bool StopY = false; //false = shift devam, true= shift dur
    int emptypieceCountX = 0;
    int emptypieceCountY = 0;
    int emptyRowCount = 0;
    int emptyColumnCount = 0;

    // Satırları kontrolu(sola kaydırmak için)
    for (int i = 0; i < 5; i++)
    {
        for (int k = 0; k < 5; k++)
        {
            if (!StopX)
            {
                if (grid[i, k] == false)
                {
                    emptypieceCountX++;
                    if (emptypieceCountX == 5)
                    {
                        emptyRowCount++;
                        emptypieceCountX = 0;
                    }
                }
                else if (grid[i, k] == true)
                {
                    emptypieceCountX = 0;
                    StopX = true;
                }
                if (k == 4)
                    emptypieceCountX = 0;
            }
        }
    }

    // sütun kontrolü(yukari kaydirma icin)
    for (int i = 0; i < 5; i++)
    {
        for (int k = 0; k < 5; k++)
        {
            if (!StopY)
            {
                if (grid[k, i] == false)
                {
                    emptypieceCountY++;
                    if (emptypieceCountY == 5)
                    {
                        emptyColumnCount++;
                        emptypieceCountY = 0;
                    }
                }
                else if (grid[k, i] == true)
                {
                    emptypieceCountY = 0;
                    StopY = true;
                }

                if (i == 4)
                    emptypieceCountY = 0;
            }
        }
    }
    //PrintPiece(grid); // debug line to see pieces that will be normalized -----------------------------------------------------------------
    //Console.WriteLine("emptyRowCount: " + emptyRowCount); // debug line-----------------------------------------------------------------
    //Console.WriteLine("emptyColumnCount: " + emptyColumnCount); // debug line -----------------------------------------------------------------
    //Console.Read();
    // satırları yukarı kaydır
    int R = emptyRowCount;
    int C = emptyColumnCount;
    while (emptyRowCount > 0)
    {
        for (int i = 1; i < 5; i++)
        {
            for (int k = 0; k < 5; k++)
            {
                grid[i - 1, k] = grid[i, k];
            }
        }
        emptyRowCount--;
    }

    for (int WillerasedRows = R; WillerasedRows > 0; WillerasedRows--)
        for (int k = 0; k < 5; k++)
        {
            grid[5 - WillerasedRows, k] = false;
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
        emptyColumnCount--;
    }
    for (int WillerasedCols = C; WillerasedCols > 0; WillerasedCols--)
        for (int k = 0; k < 5; k++)
        {
            grid[k, 5 - WillerasedCols] = false;
        }



    return grid;
}

static void RotatePiece(bool[,] grid)
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

    rotated = NormalizeShift(rotated);
    for (int i = 0; i < 5; i++)
        for (int k = 0; k < 5; k++)
            grid[i, k] = rotated[i, k];
}

static void ReversePiece(bool[,] grid)
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
static void DebugPieceCreation(bool[,] pieceGrid, int rowloc, int colloc)
{
    for (int i = 0; i < 5; i++)
    {
        for (int k = 0; k < 5; k++)
            if (i == rowloc && k == colloc)
                Console.Write(pieceGrid[i, k] ? 'O' : '.');
            else
                Console.Write(pieceGrid[i, k] ? 'X' : '.');
        Console.WriteLine();
    }
    Console.WriteLine();
}
