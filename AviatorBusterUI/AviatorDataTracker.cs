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
        private const double defaultLimit = 1.0;
        private const double Min = 1.25;
        private const double Med1 = 1.5;
        private const double Med2 = 1.75;
        private const double MAX = 2.0;

        // Counters for consecutive condition tracking
        private int flewPointGreaterThanMaxCounter = 0;
        private int flewPointLessThanMinCounter = 0;
        private int flewPointLessThanMinOverMaxCounter = 0;
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
            if( _historicalData.Count >= 5)
            {
                return true;
            }
            return false;
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

                if (flewPointGreaterThanMaxCounter < 2)
                {
                    // If FlewPoint exceeds Max once but with possibility of being much greater than MAX
                    if (flewPoint > 10)
                    {
                        limit = defaultLimit.ToString() + "- Skip";
                    }
                }
                else if (flewPointGreaterThanMaxCounter >= 2)
                {
                    // Condition 2 + 3: If FlewPoint exceeds Max twice consecutively
                    if ((flewPoint > MAX) && (flewPoint < 4.5))
                    {
                        limit = "Limit: " + Med2;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                    else 
                    {
                        limit = "Limit: " + Med1;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                }
                else
                {
                    limit = "Limit: " + Min;

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
                flewPointLessThanMinOverMaxCounter++;

                if (flewPointLessThanMinOverMaxCounter > 4)
                {
                    limit = "Limit: " + MAX;
                    flewPointLessThanMinOverMaxCounter = 0;
                }
                else if (flewPointLessThanMinCounter >= 2)
                {
                    limit = "Limit: " + Med1;
                    flewPointLessThanMinCounter = 0;
                }
                else
                {
                    limit = "Limit: " + Med1;
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
            return string.IsNullOrEmpty(limit) ? defaultLimit.ToString() : limit;
        }

        public double PredictLimit()
        {
            InitiatePredict();
            double flewPoint = _historicalData.Last.Value;
            double limit = 1;  // Default limit is set to 1

            // Condition 1 & 2: If FlewPoint > Max
            if (flewPoint > MAX)
            {
                flewPointGreaterThanMaxCounter++;

                if (flewPointGreaterThanMaxCounter < 2)
                {
                    // If FlewPoint exceeds Max once but with possibility of being much greater than MAX
                    if (flewPoint > 10)
                    {
                        return defaultLimit;
                    }
                }
                else if (flewPointGreaterThanMaxCounter >= 2)
                {
                    // Condition 2 + 3: If FlewPoint exceeds Max twice consecutively
                    if ((flewPoint > MAX) && (flewPoint < 4.5))
                    {
                        limit = Med2;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                    else
                    {
                        limit = Med1;
                        flewPointGreaterThanMaxCounter = 0;
                    }
                }
                else
                {
                    limit = Min;
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
                flewPointLessThanMinOverMaxCounter++;

                if (flewPointLessThanMinOverMaxCounter > 4)
                {
                    limit = MAX;
                    flewPointLessThanMinOverMaxCounter = 0;
                }
                else if (flewPointLessThanMinCounter >= 2)
                {
                    limit = Med1;
                    flewPointLessThanMinCounter = 0; // Reset consecutive counter after two consecutive occurrences
                }
                else
                {
                    limit = Med1; // If it's less than Min but not yet twice consecutively
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
                    limit = Med1;
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
                    limit = Min;
                    flewPointLessThanMed2Counter = 0;
                }
            }
            else
            {
                flewPointLessThanMed2Counter = 0;
            }

            // Return the final prediction or default limit of 1 if no conditions are met
            return limit;
        }
    }
}
