using System;

namespace Graph_t
{
    public class City
    {
        // Member variables
        public int cityNum { get; private set; }
        int[,] cityAdMat = new int[8, 8];
        Edge edge = new Edge();

        public City(int cityNum)
        { 
            this.cityNum = cityNum;
            cityAdMat = edge.loadDist();
        }

        // Functionality
        //find distance from read file
        public int distanceTo(City c)
        {
            //return distance between cities
            return cityAdMat[this.cityNum,c.cityNum];
        }
    }
}
