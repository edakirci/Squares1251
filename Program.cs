using System;
using System.Collections.Generic;
int total=0;

int INPUT = 0;
bool acceptable =false;
Random rnd = new();

bool[,] pieces1  = new bool[5,5];
bool[,] pieces2  = new bool[5,5];
bool[,] pieces3  = new bool[5,5];
bool[,] pieces4  = new bool[5,5];
bool[,] pieces5  = new bool[5,5];
bool[,] pieces6  = new bool[5,5];
bool[,] pieces7  = new bool[5,5];
bool[,] pieces8  = new bool[5,5];
bool[,] pieces9  = new bool[5,5];
bool[,] pieces10 = new bool[5,5];
bool[,] pieces11 = new bool[5,5];
bool[,] pieces12 = new bool[5,5];
bool[,] pieces13 = new bool[5,5];
bool[,] pieces14 = new bool[5,5];
bool[,] pieces15 = new bool[5,5];
bool[,] pieces16 = new bool[5,5];
bool[,] pieces17 = new bool[5,5];
bool[,] pieces18 = new bool[5,5];
bool[,] pieces19 = new bool[5,5];
bool[,] pieces20 = new bool[5,5];
Array[] arrays = new Array[20] {pieces1,pieces2,pieces3,pieces4,pieces5,pieces6,pieces7,pieces8,pieces9,pieces10,pieces11,pieces12,pieces13,pieces14,pieces15,pieces16,pieces17,pieces18,pieces19,pieces20};

while (INPUT == 0)
{
    bool[,] tempo= new bool[5,5];
    Console.Write("Pieces number:");
    int piecenum = Convert.ToInt16(Console.ReadLine());
    for(int a1=1;a1<=piecenum;a1++)
    {
        int bosparcax=0;
int bosparcay=0;
int bossutun=0;
int bossatir=0;
                 acceptable = false;
                bool[,] pieces = new bool[5, 5];

                while (!acceptable)
                {
                    
                    // fill grid randomly
                    for (int i = 0; i < 5; i++)
                        for (int k = 0; k < 5; k++)
                            {pieces[i, k] = rnd.Next(0, 2) == 0;} // true = X, false = .
                    
                    
                        
                    // find a start X and total X count
                     total = 0;
                    int srow = -1, scol = -1;
                    for (int i = 0; i < 5; i++)
                        for (int k = 0; k < 5; k++)
                            if (pieces[i, k])
                            {
                                total++;
                                if (srow == -1) { srow = i; scol = k; }
                            }
                    if(total<2 | total>12)
                    {
                        acceptable= false;
                     continue;
                    }
                    if (total == 0)
                    {
                        acceptable = false;
                        continue;
                    }

                    // BFS ile bağlılık kontrolü: start'tan erişilen X sayısı total'a eşit olmalı 
                    // BFS =Bir graf veya ızgara üzerinde, bir başlangıç düğümünden başlayarak önce aynı uzaklıktaki tüm düğümleri (aynı seviye) gezip sonra bir sonraki seviyeye geçer.
                    
                    int reached = BFSCount(pieces, srow, scol);  //AI
                    if (reached != total)                        //AI
                    {                                            //AI
                        acceptable = false;                  
                        continue;                                //AI
                    }                                            //AI

                    // check for isolated trues (gerekirse; BFS zaten bağlı bileşen kontrolünü yaptı,
                    // ama istenirse her X'in en az bir komşusu olduğunu ayrıca kontrol ederiz)
                    bool anyIsolated = false;
                    for (int i = 0; i < 5 && !anyIsolated; i++)
                    {
                        for (int k = 0; k < 5 && !anyIsolated; k++)
                        {
                            if (!pieces[i, k]) continue;

                            bool hasNeighbor =
                                (i > 0 && pieces[i - 1, k]) ||
                                (i < 4 && pieces[i + 1, k]) ||
                                (k > 0 && pieces[i, k - 1]) ||
                                (k < 4 && pieces[i, k + 1]);

                            if (!hasNeighbor) anyIsolated = true;
                        }
                    }

                    acceptable = !anyIsolated;
                    
                }


for(int i=0;i<5;i++)
        {
            for(int k=0;k<5;k++)
            {
                if (pieces[i,k]== false)
                {
                   bosparcax++;
                   if (bosparcax==5) 
                        bossatir++;                
                } 
                
                else 
                bosparcax=0;
                if (pieces[k,i] == false)
                {
                    bosparcay++;
                    if(bosparcay==5) bossutun++;
                } else bosparcay=0;
            }
        }
while(bossatir>0)
        {
            for(int i = 1; i < 5; i++)
            {
                for(int k = 0; k < 5; k++)
                {
                    pieces[i-1,k] = pieces[i,k];
                }
            }
                for(int k = 0; k < 5; k++)
                {
                    pieces[4,k]= false;
                }
                bossatir--;
        }
while(bossutun>0)
        {
            for(int i = 0; i < 5; i++)
            {
                for(int k = 1; k < 5; k++)
                {
                    pieces[i,k-1] = pieces[i,k];
                }
            }
                for(int k = 0; k < 5; k++)
                {
                    pieces[k,4]= false;
                }
                bossutun--;
        }



            arrays[a1]=pieces;
            print(pieces);
         


    }


                Console.WriteLine();
                if(acceptable) Console.WriteLine("0=restart, 1=call back");
                if (acceptable) INPUT = Convert.ToInt16(Console.ReadLine());
                if (INPUT==1)
                {
                    Console.Write("which one: ");
                    int a2=Convert.ToInt16(Console.ReadLine());
                    tempo= (bool[,])arrays[a2];
                    print(tempo);
                    INPUT=0;

                }       

    
}









            //Asagisi AI help. :/                 <----


            // Yerel fonksiyon: start'tan erişilebilen true hücre sayısını döner (ortogonal bağlantı)
            int BFSCount(bool[,] grid, int sr, int sc)
            {
                int n = grid.GetLength(0);
                int m = grid.GetLength(1);

                if (sr < 0 || sc < 0 || sr >= n || sc >= m) return 0;
                if (!grid[sr, sc]) return 0;

                bool[,] seen = new bool[n, m];
                Queue<(int r, int c)> q = new();
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
void print(bool[,] parray)
{
     for (int i = 0; i < 5; i++)
                {
                    for (int k = 0; k < 5; k++)
                        Console.Write(parray[i, k] ? 'X' : '.');
                    Console.WriteLine();
                }
                Console.WriteLine();
}
