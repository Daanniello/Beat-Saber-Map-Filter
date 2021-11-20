using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatSaberMapFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<Models.DiffElement> _shownMapDataResult;
        public MainWindow()
        {
            InitializeComponent();
            //// x y
            //var dic = new Dictionary<double, double>();
            //dic.Add(1, 40);
            //dic.Add(2, 50);
            //dic.Add(3, 70);
            //dic.Add(4, 9);
            //dic.Add(5, 12);

            //DrawGraph(dic);
        }

        private void DrawGraph(Dictionary<double, double> points)
        {
            var width = Canvas.Width;
            var height = Canvas.Height;

            var spaceWidth = width / (points.Keys.Max()/* - points.Keys.Min()*/);
            var spaceHeight = height / (points.Values.Max()/* - points.Values.Min()*/);

            //Max Value Line and text
            var topLine = new Line();
            topLine.X1 = 0;
            topLine.X2 = width;
            topLine.Y1 = height - points.Values.Max() * spaceHeight;
            topLine.Y2 = height - points.Values.Max() * spaceHeight;
            topLine.Stroke = System.Windows.Media.Brushes.Black;
            topLine.HorizontalAlignment = HorizontalAlignment.Left;
            topLine.VerticalAlignment = VerticalAlignment.Center;
            var topLineText = new TextBlock();
            topLineText.Text = Math.Round(points.Values.Max(), 2).ToString();
            topLineText.Foreground = new SolidColorBrush(Color.FromRgb(237, 242, 244));
            Canvas.SetLeft(topLineText, topLine.X1 + width + 10);
            Canvas.SetTop(topLineText, topLine.Y1);
            Canvas.Children.Add(topLine);
            Canvas.Children.Add(topLineText);

            //Avg Value Line and text
            var avgLine = new Line();
            avgLine.X1 = 0;
            avgLine.X2 = width;
            avgLine.Y1 = height - points.Values.Average() * spaceHeight;
            avgLine.Y2 = height - points.Values.Average() * spaceHeight;
            avgLine.Stroke = System.Windows.Media.Brushes.Black;
            avgLine.HorizontalAlignment = HorizontalAlignment.Left;
            avgLine.VerticalAlignment = VerticalAlignment.Center;
            var avgLineText = new TextBlock();
            avgLineText.Text = Math.Round(points.Values.Average(), 2).ToString();
            avgLineText.Foreground = new SolidColorBrush(Color.FromRgb(237, 242, 244));
            Canvas.SetLeft(avgLineText, avgLine.X1 + width + 10);
            Canvas.SetTop(avgLineText, avgLine.Y1);
            Canvas.Children.Add(avgLine);
            Canvas.Children.Add(avgLineText);

            //Min Value Line and text
            var minLine = new Line();
            minLine.X1 = 0;
            minLine.X2 = width;
            minLine.Y1 = height - points.Values.Min() * spaceHeight;
            minLine.Y2 = height - points.Values.Min() * spaceHeight;
            minLine.Stroke = System.Windows.Media.Brushes.Black;
            minLine.HorizontalAlignment = HorizontalAlignment.Left;
            minLine.VerticalAlignment = VerticalAlignment.Center;
            var minLineText = new TextBlock();
            minLineText.Text = Math.Round(points.Values.Min(), 2).ToString();
            minLineText.Foreground = new SolidColorBrush(Color.FromRgb(237, 242, 244));
            Canvas.SetLeft(minLineText, minLine.X1 + width + 10);
            Canvas.SetTop(minLineText, minLine.Y1);
            Canvas.Children.Add(minLine);
            Canvas.Children.Add(minLineText);

            //Draw All datapoint lines
            for (var x = 0; x < points.Count; x++)
            {
                if (x + 1 == points.Count) continue;
                var line = new Line();
                line.Stroke = System.Windows.Media.Brushes.Red;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 20 / Math.Sqrt(points.Count);
                line.X1 = (points.Keys.ElementAt(x) * spaceWidth) - spaceWidth;
                line.X2 = (points.Keys.ElementAt(x + 1) * spaceWidth) - spaceWidth;
                line.Y1 = height - (points.Values.ElementAt(x) * spaceHeight);
                line.Y2 = height - (points.Values.ElementAt(x + 1) * spaceHeight);
                //line.Y1 = (height - ((points.Values.ElementAt(x) * spaceHeight) - spaceHeight)) + (points.Values.ElementAt(0) * spaceHeight);
                //line.Y2 = (height - ((points.Values.ElementAt(x + 1) * spaceHeight) - spaceHeight)) + (points.Values.ElementAt(0) * spaceHeight);
                Canvas.Children.Add(line);
            }
        }

        private async void ShowResultsButton_Click(object sender, RoutedEventArgs e)
        {
            graphCombobox.Items.Clear();

            var downloadMin = Convert.ToInt32(DownloadMinInput.Text);
            var downloadMax = Convert.ToInt32(DownloadMaxInput.Text);
            var upvotesMin = Convert.ToInt32(UpvotesMinInput.Text);
            var upvotesMax = Convert.ToInt32(UpvotesMaxInput.Text);
            var ratioMin = Convert.ToInt32(RatioMinInput.Text);
            var ratioMax = Convert.ToInt32(RatioMaxInput.Text);
            var downvotesMin = Convert.ToInt32(DownloadMinInput.Text);
            var downvotesMax = Convert.ToInt32(DownloadMaxInput.Text);
            var bpmMin = Convert.ToInt32(BPMMinInput.Text);
            var bpmMax = Convert.ToInt32(BPMMaxInput.Text);
            var njsMin = Convert.ToDouble(NJSMinInput.Text);
            var njsMax = Convert.ToDouble(NJSMaxInput.Text);
            var npsMin = Convert.ToInt32(NPSMinInput.Text);
            var npsMax = Convert.ToInt32(NPSMaxInput.Text);
            var ntsMin = Convert.ToInt32(NotesMinInput.Text);
            var ntsMax = Convert.ToInt32(NotesMaxInput.Text);
            var starsMin = Convert.ToDouble(StarsMinInput.Text);
            var starsMax = Convert.ToDouble(StarsMaxInput.Text);
            var ppMin = Convert.ToDouble(PPMinInput.Text);
            var ppMax = Convert.ToDouble(PPMaxInput.Text);

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://cdn.wes.cloud/beatstar/bssb/v2-all.json");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //Collection with hashcode as key and mapMainDetails as value
                var allBeatSaberMaps = JsonConvert.DeserializeObject<Dictionary<string, Models.Map>>(responseBody);

                //Collection with only mapMainDetails 
                var allBeatSaberMapsValues = allBeatSaberMaps.Values;

                //Collection with only mapSpecificDetails (has duplicate keyCodes)
                var AllMaps = new List<Models.DiffElement>();
                foreach (var diff in allBeatSaberMapsValues)
                {
                    foreach (var map in diff.Diffs)
                    {
                        //Add More map details. Also calculate more map details [TODO]
                        map.KeyCode = diff.Key;
                        if (map.Length > 0 && map.Notes > 0) map.Nps = map.Notes / map.Length;
                        map.Pp = map.Pp.Replace(".", ",");
                        map.Star = map.Star.Replace(".", ",");
                        if (Convert.ToDouble(map.Pp) > 0) map.IsRanked = true;
                        map.BPM = diff.Bpm;
                        map.DownloadCount = diff.DownloadCount;
                        map.Upvotes = diff.UpVotes;
                        map.Downvotes = diff.DownVotes;
                        map.Heat = diff.Heat;
                        map.Rating = diff.Rating;
                        map.UploadDate = diff.Uploaddate;

                        //Check for every filter option if the map should be added or not
                        var shouldBeAdded = true;
                        //Global Filters--------------------
                        if ((bool)GlobalEnabledCheckBox.IsChecked)
                        {
                            if (!(diff.Bpm >= bpmMin && diff.Bpm <= bpmMax)) shouldBeAdded = false;
                            if (!(diff.DownloadCount >= downloadMin && diff.DownloadCount <= downloadMax)) shouldBeAdded = false;
                            if (!(diff.UpVotes >= upvotesMin && diff.UpVotes <= upvotesMax)) shouldBeAdded = false;
                            if (!(diff.DownVotes >= downvotesMin && diff.DownVotes <= downvotesMax)) shouldBeAdded = false;
                            if (!((diff.Rating * 100) >= ratioMin && (diff.Rating * 100) <= ratioMax)) shouldBeAdded = false;
                            if (EasyCheckBox.IsChecked == false && map.Diff == "Easy") shouldBeAdded = false;
                            if (NormalCheckBox.IsChecked == false && map.Diff == "Normal") shouldBeAdded = false;
                            if (HardCheckBox.IsChecked == false && map.Diff == "Hard") shouldBeAdded = false;
                            if (ExpertCheckBox.IsChecked == false && map.Diff == "Expert") shouldBeAdded = false;
                            if (ExpertPlusCheckBox.IsChecked == false && map.Diff == "Expert+") shouldBeAdded = false;
                        }

                        //Map Filters--------------------                      
                        if ((bool)MapEnabledCheckBox.IsChecked)
                        {
                            if (!(map.Njs >= njsMin && map.Njs <= njsMax)) shouldBeAdded = false;
                            if (!(map.Nps >= npsMin && map.Nps <= npsMax)) shouldBeAdded = false;
                            if (!(map.Notes >= ntsMin && map.Notes <= ntsMax)) shouldBeAdded = false;
                        }

                        //ScoreSaber Filters-----------------
                        if ((bool)ScoresaberEnabledCheckBox.IsChecked)
                        {
                            if (!(Convert.ToDouble(map.Star) >= starsMin && Convert.ToDouble(map.Star) <= starsMax)) shouldBeAdded = false;
                            if (!(Convert.ToDouble(map.Pp) >= ppMin && Convert.ToDouble(map.Pp) <= ppMax)) shouldBeAdded = false;
                            if ((bool)RankedOnlyCheckBox.IsChecked) if (!map.IsRanked) shouldBeAdded = false;
                        }

                        if (shouldBeAdded) AllMaps.Add(map);
                    }
                };

                //Max range to output. 
                var maxOutputCount = Convert.ToInt32(MaxOutputInput.Text);
                if (maxOutputCount == 0) maxOutputCount = AllMaps.Count();
                var rangedMapDifficulties = AllMaps.Take(maxOutputCount);
                ResultsCountLabel.Content = $"Total Results: {AllMaps.Count()} - Shown Results: {maxOutputCount}";
                MapDataGrid.ItemsSource = rangedMapDifficulties;





                //Add colum headers into combobox from graph
                foreach (var colum in MapDataGrid.Columns)
                {
                    if (colum.Header.ToString() == "Scores" || colum.Header.ToString() == "UploadDate" || colum.Header.ToString() == "KeyCode" || colum.Header.ToString() == "Diff") continue;
                    graphCombobox.Items.Add(colum.Header);
                }
                _shownMapDataResult = rangedMapDifficulties;
            }
        }

        private void graphCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Graph clear
            Canvas.Children.Clear();

            //Draw Graph
            var dic = new Dictionary<double, double>();

            if (graphCombobox.SelectedItem == null) return;
            var prop = typeof(Models.DiffElement).GetProperty(graphCombobox.SelectedItem.ToString());
            var count = 1;
            foreach (var mapData in _shownMapDataResult)
            {
                var value = prop.GetValue(mapData);
                dic.Add(count, Convert.ToDouble(value));
                count++;
            }
            DrawGraph(dic);
        }
    }
}
