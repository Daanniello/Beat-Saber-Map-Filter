using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            Canvas.SetLeft(topLineText, -40);
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
            Canvas.SetLeft(avgLineText, -40);
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
            Canvas.SetLeft(minLineText, -40);
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
            var durationMin = Convert.ToDouble(DurationMinInput.Text);
            var durationMax = Convert.ToDouble(DurationMaxInput.Text);
            var npsMin = Convert.ToInt32(NPSMinInput.Text);
            var npsMax = Convert.ToInt32(NPSMaxInput.Text);
            var obstaclesMin = Convert.ToInt32(ObstaclesMinInput.Text);
            var obstaclesMax = Convert.ToInt32(ObstaclesMaxInput.Text);
            var bombsMin = Convert.ToInt32(BombsMinInput.Text);
            var bombsMax = Convert.ToInt32(BombsMaxInput.Text);
            var ntsMin = Convert.ToInt32(NotesMinInput.Text);
            var ntsMax = Convert.ToInt32(NotesMaxInput.Text);
            var starsMin = Convert.ToDouble(StarsMinInput.Text);
            var starsMax = Convert.ToDouble(StarsMaxInput.Text);
            var ppMin = Convert.ToDouble(PPMinInput.Text);
            var ppMax = Convert.ToDouble(PPMaxInput.Text);
            var fromDate = DatepickerFrom.SelectedDate;
            var toDate = DatePickerTo.SelectedDate;

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://cdn.wes.cloud/beatstar/bssb/v2-all.json");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //Collection with hashcode as key and mapMainDetails as value
                var allBeatSaberMaps = JsonConvert.DeserializeObject<Dictionary<string, Models.Map>>(responseBody);
                var hashCodes = allBeatSaberMaps.Keys.ToArray();

                //Collection with only mapMainDetails 
                var allBeatSaberMapsValues = allBeatSaberMaps.Values;

                //Collection with only mapSpecificDetails (has duplicate keyCodes)
                var AllMaps = new List<Models.DiffElement>();
                var count = 0;
                foreach (var diff in allBeatSaberMapsValues)
                {
                    foreach (var map in diff.Diffs)
                    {
                        //Add More map details.
                        map.HashCode = hashCodes[count];
                        map.KeyCode = diff.Key;
                        if (map.Length > 0 && map.Notes > 0) map.Nps = Math.Round(Convert.ToDouble(map.Notes) / Convert.ToDouble(map.Length), 2);
                        map.Pp = map.Pp;
                        map.Star = map.Star;
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

                        if (fromDate != null && toDate != null) if (!(map.UploadDate >= fromDate && map.UploadDate <= toDate)) shouldBeAdded = false;

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
                            if (!(map.Length >= durationMin && map.Length <= durationMax)) shouldBeAdded = false;                            
                        }

                        //Map Filters--------------------                      
                        if ((bool)MapEnabledCheckBox.IsChecked)
                        {
                            if (!(map.Njs >= njsMin && map.Njs <= njsMax)) shouldBeAdded = false;
                            if (!(map.Nps >= npsMin && map.Nps <= npsMax)) shouldBeAdded = false;
                            if (!(map.Notes >= ntsMin && map.Notes <= ntsMax)) shouldBeAdded = false;
                            if (!(map.Bombs >= bombsMin && map.Bombs <= bombsMax)) shouldBeAdded = false;
                            if (!(map.Obstacles >= obstaclesMin && map.Obstacles <= obstaclesMax)) shouldBeAdded = false;
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
                    count++;
                };

                //Max range to output. 
                var maxOutputCount = Convert.ToInt32(MaxOutputInput.Text);
                
                //Randomize order if user wants that 
                var random = new Random();
                if ((bool)RandomizeOrderCheckbox.IsChecked) AllMaps = AllMaps.OrderBy(a => random.Next(0, maxOutputCount)).ToList();
                //Remove deleted maps if user wants that
                if (!(bool)IncludeRemovedMapsCheckbox.IsChecked) AllMaps = AllMaps.Where(x => x.UploadDate != null).ToList();
                //limit the output to what the user wants
                if (maxOutputCount == 0) maxOutputCount = AllMaps.Count();
                var rangedMapDifficulties = AllMaps.Take(maxOutputCount);
                ResultsCountLabel.Content = $"Total Results: {AllMaps.Count()} - Shown Results: {maxOutputCount}";
                MapDataGrid.ItemsSource = rangedMapDifficulties.OrderBy(x => x.UploadDate);


                //Add colum headers into combobox from graph
                foreach (var colum in MapDataGrid.Columns)
                {
                    if (colum.Header.ToString() == "Scores" || colum.Header.ToString() == "UploadDate" || colum.Header.ToString() == "KeyCode" || colum.Header.ToString() == "Diff") continue;
                    graphCombobox.Items.Add(colum.Header);
                }
                _shownMapDataResult = rangedMapDifficulties.OrderBy(x => x.UploadDate);
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

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedPathLabel.Content = dialog.SelectedPath + "\\BeatSaberMapFilterPlaylist.json";
                }            
            }
        }

        private void SaveAsPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var playlist = new Models.PlaylistModel();
            playlist.PlaylistTitle = "BeatSaberMapFilterPlaylist";
            playlist.PlaylistAuthor = "BeatSaberMapFilterPlaylist";
            playlist.PlaylistDescription = "Playlist made from results from the BeatSaberMapFilter tool";
            playlist.Image = "data:image/png;base64,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";

            var songList = new List<Models.Song>();
            foreach (var map in _shownMapDataResult)
            {
                songList.Add(new Models.Song() { Hash = $"{map.HashCode}" });
            }

            playlist.Songs = songList;
            var json = JsonConvert.SerializeObject(playlist);
            File.WriteAllText(SelectedPathLabel.Content.ToString(), json);
        }
    }
}
