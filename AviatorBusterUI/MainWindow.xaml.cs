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

            scatterChart.Series = new SeriesCollection
            {
                new ScatterSeries
                {
                    Title = "Aviator Game Data",
                    Values = new ChartValues<ScatterPoint>(),
                    MinPointShapeDiameter = 10,
                    MaxPointShapeDiameter = 15
                },
                                new LineSeries
                {
                    Title = "Trend Line",
                    Values = new ChartValues<ObservablePoint>(), // LineSeries needs ObservablePoint for X, Y pairs
                    PointGeometry = null, 
                    StrokeThickness = 2
                }
            };

            DataContext = this;
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

                // Check if a prediction can be made and display it
                if (tracker.CanPredict())
                {
                    PredictionOutput.Text = tracker.Predict();
                }
                else
                {
                    PredictionOutput.Text = "Initiating 🔃";
                }

                var scatterSeries = (ScatterSeries)scatterChart.Series[0];

                var lineSeries = (LineSeries)scatterChart.Series[1];
                lineSeries.Values.Add(new ObservablePoint(counter, gameData)); // ObservablePoint is used for the line

                // Add the X (gameData) and Y (calculated value) as ScatterPoint
                scatterSeries.Values.Add(new ScatterPoint(counter++, gameData));

            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}