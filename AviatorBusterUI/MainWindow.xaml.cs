using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using ISeries = LiveChartsCore.ISeries;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace AviatorBusterUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public ISeries[] ScatterValues { get; set; }
        private readonly AviatorDataTracker tracker;
        private double counter = 0;

        public MainWindow()
        {
            InitializeComponent();
            tracker = new AviatorDataTracker();

            // LineSeries for game data (this will connect points with a trendline)
            var gameDataLineSeries = new LineSeries
            {
                Title = "Aviator Game Data Trendline",
                Values = new ChartValues<ObservablePoint>(),
                LineSmoothness = 2, // Straight line connection
                StrokeThickness = 2,
                //PointGeometry = null // Hide points on the line series
            };

            // ScatterSeries for predicted limit points
            var predictedLimitScatterSeries = new ScatterSeries
            {
                Title = "Predicted Limit",
                Values = new ChartValues<ScatterPoint>(),
                MinPointShapeDiameter = 10,
                MaxPointShapeDiameter = 15
            };

            // Add both series to the scatter chart
            scatterChart.Series = new SeriesCollection { gameDataLineSeries, predictedLimitScatterSeries };
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Call the SubmitData method, passing null for the sender and event args since they're not needed
                SubmitData(null, null);
            }
        }

        private void SubmitData(object sender, RoutedEventArgs e)
        {
            // Get the game data from the TextBox
            if (double.TryParse(GameDataInput.Text, out double gameData))
            {
                tracker.AddData(gameData);

                // Add the data to the historical list in the UI
                HistoricalDataList.Items.Insert(0, gameData);

                // Remove last item
                if (HistoricalDataList.Items.Count > 50)
                {
                    HistoricalDataList.Items.RemoveAt(HistoricalDataList.Items.Count - 1); 
                }

                // Clear the input TextBox
                GameDataInput.Clear();

                // Update the line series for game data (this will connect the points with a line)
                var gameDataLineSeries = (LineSeries)scatterChart.Series[0];
                gameDataLineSeries.Values.Add(new ObservablePoint(counter++, gameData));

                // Check if a prediction can be made and display it
                if (tracker.CanPredict())
                {
                    // Use the PredictLimit method to get the limit for trendline
                    double predictedLimit = tracker.PredictLimit();

                    // Update the scatter series for the predicted limit
                    var predictedLimitScatterSeries = (ScatterSeries)scatterChart.Series[1];
                    predictedLimitScatterSeries.Values.Add(new ScatterPoint(counter, predictedLimit));

                    PredictionOutput.Text = tracker.Predict();
                }
                else
                {
                    PredictionOutput.Text = "Initiating 🔃";
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}