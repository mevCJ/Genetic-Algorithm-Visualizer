using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace Graph_t
{
    public partial class Form1 : Form
    {
        private static DataTable assignTable;
        public static Random r { get; private set; }
        int[,] loc = new int[8, 2];
        Label[] points = new Label[8];
        public static List<int> chrom = new List<int>();
        Stopwatch sw = new Stopwatch();
        public static string[] cities = {"Kuala Lumpur", "Kota Bahru", "Kuantan", "Alor Setar",
                                      "Seremban", "Melaka", "Ipoh", "Georgetown"};

        public Form1()
        {
            InitializeComponent();
            tableSource();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //find all points
            for (int i = 0; i < 8; i++)
            {
                points[i] = (Label)Controls.Find("p" + (i).ToString(), true)[0];
                points[i].BackColor = Color.Red;
            }

            //save location of the points
            for(int i = 0; i < 8; i++)
            {
                int j = 0;
                loc[i, j] = points[i].Location.X;
                loc[i, j + 1] = points[i].Location.Y;
            }

        }

        //draw route
        private void paint()
        {

            int x = 1;
            Graphics g;

            g = this.CreateGraphics();
            g.Clear(SystemColors.Control);
            Pen myPen = new Pen(Color.Red);
            myPen.Width = 1;
            for (int i = 0; i < 8; i++)
            {
                int j = 0;

                if (i == 7)
                    x = 0;

                g.DrawLine(myPen, loc[chrom[i], j], loc[chrom[i], j+1], loc[chrom[i + x], j], loc[chrom[i + x], j+1]);
            }
            g.DrawLine(myPen, loc[chrom[7], 0], loc[chrom[7], 1], loc[0, 0], loc[0, 1]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sw.Reset();
            start_btn.Enabled = false;
            wait_lbl.Text = "Please wait. This may take a moment...";

            r = new Random();
            assignTable.Clear();
            int gen = 0;
            int limitGen = 0;
            bool better = true;

            sw.Start();                                                                         //start timer
            Tour dest = Tour.random();                                                          //generate a chromosome
            Population p = Population.randomized(dest, Env.popSize);                            //create a population of chromosome from dest
            Task<int> task = new Task<int>(() => evolveChrom(gen, limitGen, better, p));        //threading to prevent freezing

            task.Start();
        }

        private int evolveChrom(int gen, int limitGen, bool better, Population p)
        {
            while (true)
            {
                if (better)
                    display(p, gen);

                better = false;
                double oldFit = p.maxFit;

                p = p.evolve();

                //if this population contain a chromosome with higher fitness
                if (p.maxFit > oldFit)
                    better = true;

                if (p.maxFit == oldFit)
                    limitGen++;

                //termination criteria: terminates if the following 20 generation has the same fitness level
                if (limitGen > 12)
                {
                    sw.Stop();
                    time_lbl.Text = sw.Elapsed.TotalSeconds.ToString("f2") + " seconds";
                    start_btn.Enabled = true;
                    wait_lbl.Text = "";
                    return 0;
                }

                gen++;
            }
        }
        public void display(Population p, int gen)
        {
            Tour best = p.findBest();
            chrom = best.getChromosome();
            List<string> route = new List<string>();
            string fullRoute = "";

            route = best.route(cities);
            foreach (string r in route)
            {
                fullRoute += r;
                fullRoute += "->";
            }
            fullRoute += "Kuala Lumpur";
            paint();
            assignTable.Rows.Add(gen, best.fitness, best.distance, fullRoute);
            System.Threading.Thread.Sleep(100);                                     //small delay when displaying the output
        }

        public static class Env
        {
            public const double crosRate = 0.8;
            public const double mutRate = 0.03;
            public const int elitism = 6;
            public const int popSize = 60;
            public const int numCities = 8;
        }

        public void tableSource()
        {
            assignTable = new DataTable("ProductTable");
            assignTable.Columns.Add("Gen", typeof(string));
            assignTable.Columns.Add("Best Fitness", typeof(string));
            assignTable.Columns.Add("Distance", typeof(string));
            assignTable.Columns.Add("Route", typeof(string));
            resultTable.DataSource = assignTable;
        }
    }
}
