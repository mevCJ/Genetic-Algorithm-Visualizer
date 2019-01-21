using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph_t
{

    public class Tour 
    {

        // Member variables
        public List<City> t { get; private set; }           //chromosome
        public double distance { get; private set; }
        public double fitness { get; private set; }

        public Tour(List<City> l)
        {
            this.t = l;
            this.distance = this.calcDist();
            this.fitness = this.calcFit();
        }

        // Functionality
        // Create a chromosome with 0 as the first gene and random number for the rest
        public static Tour random()
        {
            List<City> t = new List<City>();
            List<int> randList = new List<int>();

            t.Add(new City(0));         //must starts with kuala lumpur
            randList.Add(0);

            while (randList.Count <= 7)
            {
                int randNum = Form1.r.Next(0, 8);
                if (!randList.Contains(randNum))
                {
                    randList.Add(randNum);
                    t.Add(new City(randNum));
                }
            }

            return new Tour(t);
        }

        // Create a diversified population by shuffling the first gene created
        public Tour shuffle() //swapping
        {
            List<City> tmp = new List<City>(this.t);
            int n = tmp.Count;

            while (n > 1)
            {
                n--;
                int k = Form1.r.Next(1, n + 1);
                City v = tmp[k];
                tmp[k] = tmp[n];
                tmp[n] = v;
            }

            return new Tour(tmp);
        }

        //single point crossover
        public Tour crossover(Tour m)
        {
            List<City> c = this.t;

            if (Form1.r.NextDouble() < Form1.Env.crosRate)
            {
                int i = Form1.r.Next(0, m.t.Count);
                int j = Form1.r.Next(i, m.t.Count);
                List<City> s = this.t.GetRange(i, j - i + 1);
                List<City> ms = m.t.Except(s).ToList();
                c = ms.Take(i)
                    .Concat(s)
                    .Concat(ms.Skip(i))
                    .ToList();
            }
            return new Tour(c);
        }

        //mutation swapping
        public Tour mutate()
        {
            List<City> tmp = new List<City>(this.t);

            if (Form1.r.NextDouble() < Form1.Env.mutRate)
            {
                int i = Form1.r.Next(1, this.t.Count);
                int j = Form1.r.Next(1, this.t.Count);
                City v = tmp[i];
                tmp[i] = tmp[j];
                tmp[j] = v;
            }

            return new Tour(tmp);
        }

        //calculate the total distance
        private double calcDist()
        {
            double total = 0;
            for (int i = 0; i < this.t.Count; ++i)
                total += this.t[i].distanceTo(this.t[(i + 1) % this.t.Count]);

            return total;
        }

        //fitness level = 1/total distance
        private double calcFit()
        {
            return 1 / this.distance;
        }

        //return the chromosome phenotype
        public List<string> route(string[] cities)
        {
            List<string> cityName = new List<string>();

            foreach (City c in t)
            {
                cityName.Add(cities[c.cityNum]);
            }

            return cityName;
        }

        //return the chromosome genotype
        public List<int> getChromosome()
        {
            List<int> cityNum = new List<int>();

            foreach (City c in t)
            {
                cityNum.Add(c.cityNum);
            }

            return cityNum;
        }
    }
}

