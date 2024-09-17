using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviatorBusterUI
{
    public class AviatorDataTracker
    {
        private readonly LinkedList<double> _historicalData;
        private const int MaxCapacity = 50;

        // Define variables for the preset points
        private const double Min = 1.25;
        private const double Med1 = 1.5;
        private const double Med2 = 1.75;
        private const double MAX = 2.0;

        // Counters for consecutive condition tracking
        private int flewPointGreaterThanMaxCounter = 0;
        private int flewPointLessThanMinCounter = 0;
        private int flewPointLessThanMed1Counter = 0;
        private int flewPointLessThanMed2Counter = 0;

        public AviatorDataTracker()
        {
            _historicalData = new LinkedList<double>();
        }

        // Method to add new data point
        public void AddData(double newData)
        {
            if (_historicalData.Count == MaxCapacity)
            {
                _historicalData.RemoveFirst();  // Remove oldest data point
            }
            _historicalData.AddLast(newData);  // Add new data point
        }

        // Method to check if prediction can be made
        public bool CanPredict()
        {
            return _historicalData.Count >= 5;
        }

        public void InitiatePredict()
        {
            while (!CanPredict())
            {
                return;
            }
        }

        // Method to predict preset points based on specified conditions
        public string Predict()
        {
            InitiatePredict();
            double flewPoint = _historicalData.Last.Value;
            string limit = "";

            // Condition 1 & 2: If FlewPoint > Max
            if (flewPoint > MAX)
            {
                flewPointGreaterThanMaxCounter++;

                // Condition 2 + 3: If FlewPoint exceeds Max twice consecutively
                if (flewPointGreaterThanMaxCounter >= 2)
                {
                    if ((flewPoint > MAX) && (flewPoint < 3.5))
                    {
                        limit = "Limit: " + Med2;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                    else 
                    {
                        limit = "Limit: " + Med1;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                } // If FlewPoint exceeds Max once
                else if (flewPointGreaterThanMaxCounter < 2)
                {
                    if (flewPoint > 10)
                    {
                        limit = "Skip";
                    }
                    else
                    {
                        limit = "Limit: " + Min;
                    }
                }
            }
            else
            {
                flewPointGreaterThanMaxCounter = 0;
            }

            // Condition 4: If FlewPoint < Min twice consecutively
            if (flewPoint < Min)
            {
                flewPointLessThanMinCounter++;
                if (flewPointLessThanMinCounter < 2)
                {
                    limit = "Limit: " + Med1;
                }
                else if (flewPointLessThanMinCounter >= 2)
                {
                    limit = "Limit: " + Med1;
                    flewPointLessThanMinCounter = 0;
                }
                else if (flewPointLessThanMinCounter > 3)
                {
                    limit = "Limit: " + MAX;
                    flewPointLessThanMinCounter = 0;
                }
            }
            else
            {
                flewPointLessThanMinCounter = 0;
            }

            // Condition 5: If FlewPoint < Med1 twice consecutively
            if (flewPoint < Med1)
            {
                flewPointLessThanMed1Counter++;

                if (flewPointLessThanMed1Counter >= 2)
                {
                    limit = "Limit: " + Med1;
                    flewPointLessThanMed1Counter = 0;
                }
            }
            else
            {
                flewPointLessThanMed1Counter = 0;
            }

            // Condition 6: If FlewPoint < Med2 twice consecutively
            if (flewPoint < Med2)
            {
                flewPointLessThanMed2Counter++;

                if (flewPointLessThanMed2Counter >= 2)
                {
                    limit = "Limit: " + Min;
                    flewPointLessThanMed2Counter = 0;
                }
            }
            else
            {
                flewPointLessThanMed2Counter = 0;
            }

            // Return the final prediction or limit based on conditions
            return string.IsNullOrEmpty(limit) ? "..." : limit;
        }
    }
}
