using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Graph_t
{
    public class Edge
    {

        public int[,] adMat { get; private set; }

        //convert text file to array
        public int[,] loadDist()
        {
            this.adMat = new int[8, 8];

            string allLines = File.ReadAllText("Distance.txt");
            int a = 0, b = 0;

            foreach (var row in allLines.Split('\n'))
            {
                b = 0;
                foreach (var col in row.Trim().Split(' '))
                {
                    this.adMat[a, b] = int.Parse(col.Trim());
                    b++;
                }
                a++;
            }

            return this.adMat;
        }
    }
}
