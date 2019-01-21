using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph_t
{

    public class Population
    {

        // Member variables
        public List<Tour> p { get; private set; }
        public double maxFit { get; private set; }

        // ctor
        public Population(List<Tour> l)
        {
            this.p = l;
            this.maxFit = this.calcMaxFit();
        }

        // Functionality
        //produce a diversified population with the size of n
        public static Population randomized(Tour t, int n)
        {
            List<Tour> tmp = new List<Tour>();

            for (int i = 0; i < n; ++i)
                tmp.Add( t.shuffle() );

            return new Population(tmp);
        }

        //return the maximum fitness in current population
        private double calcMaxFit()
        {
            return this.p.Max( t => t.fitness );
        }

        //roulette wheel selection for generating new population
        public Tour select()
        {
            double totalFitness = 0;
            double sum = 0;

            //find total fitness
            for(int i = 0; i < Form1.Env.popSize; i++)
                totalFitness += this.p[i].fitness;

            while (true)
            {
                //generate random number from 0 to total
                double pointer = Form1.r.NextDouble() * totalFitness;

                //go through each chromosome from 0 to total
                for (int i = 0; i < Form1.Env.popSize; i++)
                {
                    sum += this.p[i].fitness;

                    //return the chromosome when sum > pointer
                    if(sum > pointer)
                    {
                        return new Tour(this.p[i].t);
                    }
                }

            }
        }

        // Generate new population by mutation and cross-over from the population
        public Population genNewPop(int n)
        {
            List<Tour> p = new List<Tour>();

            for (int i = 0; i < n; ++i)
            {
                Tour t = this.select().crossover( this.select() );

                foreach (City c in t.t)
                    t = t.mutate();

                p.Add(t);
            }

            return new Population(p);
        }

        //select 6 elites from list
        public Population elite(int n)
        {
            List<Tour> best = new List<Tour>();
            Population tmp = new Population(p);

            for (int i = 0; i < n; ++i)
            {
                best.Add( tmp.findBest() );
                tmp = new Population( tmp.p.Except(best).ToList() );
            }

            return new Population(best);
        }

        //find best fitness chromosome
        public Tour findBest()
        {
            foreach (Tour t in this.p)
            {
                if (t.fitness == this.maxFit)
                    return t;
            }

            // Should never happen, it's here to shut up the compiler
            return null;
        }

        //perform selection, crossover and mutation
        public Population evolve()
        {
            Population best = this.elite(Form1.Env.elitism);                             //selection (elitism)
            Population np = this.genNewPop(Form1.Env.popSize - Form1.Env.elitism);      //crossover and mutation
            return new Population( best.p.Concat(np.p).ToList() );
        }
    }
}

