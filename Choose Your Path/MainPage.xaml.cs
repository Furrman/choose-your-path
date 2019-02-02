using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Choose_Your_Path.Resources;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Reflection;
using Microsoft.Phone.Tasks;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Maps.Toolkit;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Choose_Your_Path
{

    public partial class MainPage : PhoneApplicationPage
    {

        private ProgressIndicator Progress = new ProgressIndicator
        {
            IsVisible = false,
            IsIndeterminate = true,
            Text = "Calculating..."
        };
        private List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        private RouteQuery MyQuery = null;
        private MapRoute MyMapRoute;
        private Color CurrentColor;
        private Popup popup;
        private CancellationTokenSource Cts;
        private Thread thread;
        private GeocodeQuery query;
        private IAsyncOperation<Geoposition> locationTask;
        private BackgroundWorker backroungWorker;
        private Geolocator geolocator = null;
        private MapLayer userLocationLayer;
        private ApplicationBarIconButton getLocation;
        private ApplicationBarIconButton track;
        private ApplicationBarIconButton calculate;
        private ApplicationBarIconButton start;
        private ApplicationBarMenuItem settings;
        private ApplicationBarMenuItem instruction;
        private bool shuffle = false;
        private bool tracking = false;
        private int dot = 1;
        private bool Enabled = true;

        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            SystemTray.SetProgressIndicator(this, Progress);
            ShowSplash();
            CurrentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
            Cts = new CancellationTokenSource();
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            getLocation = new ApplicationBarIconButton();
            getLocation.IconUri = new Uri("/Images/dark.cross.png", UriKind.Relative);
            getLocation.Text = "Get Location";
            getLocation.Click += GetLocation;
            ApplicationBar.Buttons.Add(getLocation);

            track = new ApplicationBarIconButton();
            track.IconUri = new Uri("/Images/dark.transport.play.png", UriKind.Relative);
            track.Text = "Start tracking";
            track.Click += Track;
            ApplicationBar.Buttons.Add(track);

            calculate = new ApplicationBarIconButton();
            calculate.IconUri = new Uri("/Images/dark.shuffle.png", UriKind.Relative);
            calculate.Text = "Find cycle";
            calculate.Click += ShufflePoints;
            ApplicationBar.Buttons.Add(calculate);

            start = new ApplicationBarIconButton();
            start.IconUri = new Uri("/Images/dark.map.treasure.png", UriKind.Relative);
            start.Text = "Find path";
            start.Click += FindRoute;
            ApplicationBar.Buttons.Add(start);

            instruction = new ApplicationBarMenuItem();
            instruction.Text = "Instruction";
            instruction.Click += GoToInstruction;
            ApplicationBar.MenuItems.Add(instruction);
            
            settings = new ApplicationBarMenuItem();
            settings.Text = "Settings";
            settings.Click += GoToSettings;
            ApplicationBar.MenuItems.Add(settings);

            if (Visibility.Visible == (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"])
            {
                UpButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/light.arrow.up.png", UriKind.Relative)) };
                DownButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/light.arrow.down.png", UriKind.Relative)) };
                DeleteButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/light.delete.png", UriKind.Relative)) };
                ClearButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/light.clean.png", UriKind.Relative)) };
            }
            else
            {
                UpButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/dark.arrow.up.png", UriKind.Relative)) };
                DownButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/dark.arrow.down.png", UriKind.Relative)) };
                DeleteButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/dark.delete.png", UriKind.Relative)) };
                ClearButton.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("/Images/dark.clean.png", UriKind.Relative)) };
            }
        }

        private void ShowSplash()
        {
            this.popup = new Popup();
            if (Visibility.Visible == (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"])
            {
                this.popup.Child = new SplashScreenControl(false);
            }
            else
            {
                this.popup.Child = new SplashScreenControl(true);
            }
            this.popup.IsOpen = true;
            StartLoadingData();
        }

        private void StartLoadingData()
        {
            backroungWorker = new BackgroundWorker();
            backroungWorker.DoWork += new DoWorkEventHandler(backroungWorker_DoWork);
            backroungWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backroungWorker_RunWorkerCompleted);
            backroungWorker.RunWorkerAsync();
        }

        private void backroungWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(4000);
        }

        private void backroungWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.popup.IsOpen = false;
                CreateAppBar();
            }
            );
        }

        private void CheckTheme()
        {
            Visibility darkBackgroundVisibility = (Visibility)Resources["PhoneDarkThemeVisibility"];
            if (Visibility.Visible == darkBackgroundVisibility)
            {
                ThemeManager.ToDarkTheme();
            }
            else
            {
                ThemeManager.ToLightTheme();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            else
            {
                base.OnNavigatedTo(e);

                String msg = "";

                if (NavigationContext.QueryString.TryGetValue("msg", out msg))
                {
                    if (msg[0].Equals('1'))
                    {
                        MyMap.CartographicMode = Microsoft.Phone.Maps.Controls.MapCartographicMode.Road;
                    }
                    else
                    {
                        if (msg[0].Equals('2'))
                        {
                            MyMap.CartographicMode = Microsoft.Phone.Maps.Controls.MapCartographicMode.Aerial;
                        }
                        else
                        {
                            if (msg[0].Equals('3'))
                            {
                                MyMap.CartographicMode = Microsoft.Phone.Maps.Controls.MapCartographicMode.Hybrid;
                            }
                            else
                            {
                                if (msg[0].Equals('4'))
                                {
                                    MyMap.CartographicMode = Microsoft.Phone.Maps.Controls.MapCartographicMode.Terrain;
                                }
                            }
                        }
                    }

                    if (msg[1].Equals('1'))
                    {
                        MyMap.ColorMode = Microsoft.Phone.Maps.Controls.MapColorMode.Light;
                    }
                    else
                    {
                        if (msg[1].Equals('2'))
                        {
                            MyMap.ColorMode = Microsoft.Phone.Maps.Controls.MapColorMode.Dark;
                        }
                    }

                    if (msg[2].Equals('1'))
                    {
                        MyMap.LandmarksEnabled = true;
                    }
                    else
                    {
                        if (msg[2].Equals('2'))
                        {
                            MyMap.LandmarksEnabled = false;
                        }
                    }

                    if (msg[3].Equals('1'))
                    {
                        MyMap.PedestrianFeaturesEnabled = true;
                    }
                    else
                    {
                        if (msg[3].Equals('2'))
                        {
                            MyMap.PedestrianFeaturesEnabled = false;
                        }
                    }
                }

            }
        }

        private void Navigate(List<GeoCoordinate> geolist)
        {
            Geolocator MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            try
            {
                MyQuery = new RouteQuery();
                MyQuery.Waypoints = geolist;
                MyQuery.QueryCompleted += QueryCompleted;
                //MyQuery.QueryCompleted += ((sender, e) =>
                //    {
                //        if (e.Error == null)
                //        {
                //            MyQuery = new RouteQuery();
                //            MyQuery.Waypoints = geolist;
                //            MyQuery.QueryCompleted += QueryCompleted;
                //        }
                //    });
                MyQuery.QueryAsync();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings or capabilities are not checked.");
            }
        }

        private void QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                try
                {
                    Route MyRoute = e.Result;
                    MyMapRoute = new MapRoute(MyRoute);
                    MyMapRoute.Color = CurrentColor;
                    MyMap.AddRoute(MyMapRoute);
                    Route.Text = "";

                    List<string> RouteList = new List<string>();
                    foreach (RouteLeg leg in MyRoute.Legs)
                    {
                        foreach (RouteManeuver maneuver in leg.Maneuvers)
                        {
                            RouteList.Add(maneuver.InstructionText);
                        }
                    }

                    foreach (string line in RouteList)
                    {
                        Route.Text += dot + ". " + line + '\n';
                        dot++;
                    }

                    calculate.Text = "Find cycle";
                    calculate.IconUri = new Uri("/Images/dark.shuffle.png", UriKind.Relative);
                    start.Text = "Find path";
                    start.IconUri = new Uri("/Images/dark.map.treasure.png", UriKind.Relative);
                    EnableAppButtons(true);

                    MyQuery.Dispose();
                }
                catch (InvalidOperationException ex)
                {

                }
            }
        }

        private async void MapTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Enabled == true)
            {
                GeoCoordinate myGeoCoordinate = this.MyMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.MyMap));
                if (!myGeoCoordinate.IsUnknown)
                {
                        MyCoordinates.Add(myGeoCoordinate);
                        PointList.Items.Add(MyCoordinates.Count + ". " + await GetAddress(myGeoCoordinate.Latitude, myGeoCoordinate.Longitude));

                        AddPushpin(myGeoCoordinate, MyCoordinates.Count);
                }
                else
                {
                    MessageBox.Show("Unkown position");
                }
            }
        }

        private void AddPushpin(GeoCoordinate position, int content)
        {
            Pushpin Pushpin = new Pushpin();
            Pushpin.Content = content;
            Pushpin.GeoCoordinate = position;

            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = Pushpin;
            myLocationOverlay.PositionOrigin = new System.Windows.Point(0.0, 1.0);
            myLocationOverlay.GeoCoordinate = position;

            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);
            MyMap.Layers.Add(myLocationLayer);
        }

        private void AddUserPosition(GeoCoordinate position)
        {
            UserLocationMarker Marker = new UserLocationMarker();
            Marker.GeoCoordinate = position;

            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = Marker;
            myLocationOverlay.PositionOrigin = new System.Windows.Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = position;

            userLocationLayer = new MapLayer();
            userLocationLayer.Add(myLocationOverlay);
            MyMap.Layers.Add(userLocationLayer);
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            if (MyMap.ZoomLevel < 20)
            {
                try
                {
                    MyMap.ZoomLevel += 1;
                }
                catch (ArgumentOutOfRangeException ex)
                {

                }
            }
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            if (MyMap.ZoomLevel > 1)
            {
                try
                {
                    MyMap.ZoomLevel -= 1;
                }
                catch (ArgumentOutOfRangeException ex)
                {

                }
            }
        }

        private void TurnLeft(object sender, RoutedEventArgs e)
        {
            MyMap.Heading -= 45;
        }

        private void TurnRight(object sender, RoutedEventArgs e)
        {
            MyMap.Heading += 45;
        }

        private async void GetLocation(object sender, EventArgs e)
        {
            if (Enabled)
            {
                EnableAppButtons(false);
                getLocation.Text = "Stop";
                getLocation.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                getLocation.IsEnabled = true;

                Geolocator myGeolocator = new Geolocator();
                locationTask = myGeolocator.GetGeopositionAsync();
                Geoposition myGeoposition = await locationTask;
                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                GeoCoordinate myGeoCoordinate = CoordinateConverter.ConvertGeocoordinate(myGeocoordinate);
                MyMap.Center = myGeoCoordinate;
                MyMap.ZoomLevel = 13;

                getLocation.IconUri = new Uri("/Images/dark.cross.png", UriKind.Relative);
                getLocation.Text = "Get Location";
                EnableAppButtons(true);
            }
            else
            {
                if (query != null)
                {
                    query.CancelAsync();
                    query = null;
                }
                else
                {
                    locationTask.Cancel();
                }
                getLocation.IconUri = new Uri("/Images/dark.cross.png", UriKind.Relative);
                getLocation.Text = "Get Location";
                EnableAppButtons(true);
            }
        }

        private List<GeoCoordinate> DeepCopy(List<GeoCoordinate> list)
        {
            List<GeoCoordinate> tmp = new List<GeoCoordinate>();
            for (int i = 0; i < list.Count; i++)
            {
                tmp.Add(list[i]);
            }
            return tmp;
        }

        private async void ShufflePoints(object sender, EventArgs e)
        {
            if (Enabled)
            {
                MessageBoxResult m;
                if (MyCoordinates.Count < 2)
                {
                    MessageBox.Show("You add too small number of map points!");
                }
                else
                {
                    if (MyCoordinates.Count < 3)
                    {
                        dot = 1;
                        EnableAppButtons(false);
                        calculate.Text = "Stop";
                        calculate.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                        calculate.IsEnabled = true;

                        if (MyMapRoute != null)
                        {
                            MyMap.RemoveRoute(MyMapRoute);
                        }
                        List<GeoCoordinate> tmp = DeepCopy(MyCoordinates);
                        tmp.Add(MyCoordinates[0]);
                        Navigate(tmp);
                    }
                    else
                    {
                        m = MessageBox.Show("Do you want find best optimal path?", "Starting shuffle", MessageBoxButton.OKCancel);
                        if (m == MessageBoxResult.OK)
                        {
                            EnableAppButtons(false);
                            calculate.IsEnabled = true;
                            calculate.Text = "Stop";
                            calculate.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                            shuffle = true;
                            Algorithm A = new Algorithm(MyCoordinates);
                            List<int> Result = new List<int>();
                            try
                            {
                                Result = await Task.Run(() =>
                                {
                                    thread = Thread.CurrentThread;
                                    return A.Start();
                                }, Cts.Token);

                                string Message = "Found route:\n";
                                for (int i = 0; i < Result.Count; i++)
                                {
                                    Message += (Result[i] + 1) + " ";
                                }
                                Message += "\nDo You want accept changes?";
                                m = MessageBox.Show(Message, "Do You want accept new route?", MessageBoxButton.OKCancel);
                                if (m == MessageBoxResult.OK)
                                {
                                    for (int i = 0; i < MyCoordinates.Count; i++)
                                    {
                                        if (i != Result[i])
                                        {
                                            SwapGeoPositions(i, Result[i]);
                                            int k = -1;
                                            for (int j = 0; j < Result.Count; j++)
                                            {
                                                if (i == Result[j])
                                                {
                                                    k = j;
                                                    break;
                                                }
                                            }
                                            int tmp2 = Result[i];
                                            Result[i] = Result[k];
                                            Result[k] = tmp2;
                                        }
                                    }
                                    if (MyMapRoute != null)
                                    {
                                        MyMap.RemoveRoute(MyMapRoute);
                                    }
                                    MyMap.Layers.Clear();
                                    AddAllPushpins();
                                }
                                shuffle = false;

                                dot = 1;
                                List<GeoCoordinate> tmp = DeepCopy(MyCoordinates);
                                tmp.Add(MyCoordinates[0]);
                                Navigate(tmp);
                            }
                            catch (ThreadAbortException ex)
                            {

                            }
                        }
                        else
                        {
                            EnableAppButtons(false);
                            calculate.Text = "Stop";
                            calculate.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                            calculate.IsEnabled = true;

                            if (MyMapRoute != null)
                            {
                                MyMap.RemoveRoute(MyMapRoute);
                            }
                            List<GeoCoordinate> tmp = DeepCopy(MyCoordinates);
                            tmp.Add(MyCoordinates[0]);
                            Navigate(tmp);
                        }
                    }
                }
            }
            else
            {
                if (shuffle)
                {
                    Cts.Cancel();
                    thread.Abort();
                    calculate.Text = "Find cycle";
                    calculate.IconUri = new Uri("/Images/dark.shuffle.png", UriKind.Relative);
                    EnableAppButtons(true);
                }
                else
                {
                    MyQuery.CancelAsync();
                    calculate.Text = "Find cycle";
                    calculate.IconUri = new Uri("/Images/dark.shuffle.png", UriKind.Relative);
                    EnableAppButtons(true);
                }
            }
        }

        private void FindRoute(object sender, EventArgs e)
        {
            if (MyCoordinates.Count == 0 || MyCoordinates.Count == 1)
            {
                MessageBox.Show("You add too small number of map points!");
            }
            else
            {
                if (Enabled)
                {
                    EnableAppButtons(false);
                    start.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                    start.Text = "Stop";
                    start.IsEnabled = true;
                    dot = 1;
                    if (MyMapRoute != null)
                    {
                        MyMap.RemoveRoute(MyMapRoute);
                    }
                    Navigate(MyCoordinates);
                }
                else
                {
                    MyQuery.CancelAsync();
                    EnableAppButtons(true);
                    start.IconUri = new Uri("/Images/dark.map.treasure.png", UriKind.Relative);
                    start.Text = "Find path";
                }
            }
        }

        private void Track(object sender, EventArgs e)
        {
            if (!tracking)
            {
                EnableAppButtons(false);
                Progress.IsVisible = false;
                track.IsEnabled = true;
                
                MyMap.ZoomLevel = 16;
                geolocator = new Geolocator();
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 100;

                geolocator.PositionChanged += PositionChanged;

                tracking = true;
                track.Text = "Stop";
                track.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
            }
            else
            {
                EnableAppButtons(true);
                if (userLocationLayer != null)
                {
                    MyMap.Layers.Remove(userLocationLayer);
                }

                geolocator.PositionChanged -= PositionChanged;
                geolocator = null;

                tracking = false;
                track.Text = "Start";
                track.IconUri = new Uri("/Images/dark.transport.play.png", UriKind.Relative);
            }
        }

        void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MyMap.Layers.Remove(userLocationLayer);
                MyMap.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                AddUserPosition(new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude));
            });
        }

        private void GoToInstruction(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Instruction.xaml", UriKind.Relative));
        }

        private void GoToSettings(object sender, EventArgs e)
        {
            String msg = "";

            if (MyMap.CartographicMode.ToString() == "Road")
            {
                msg += "1";
            }
            else
            {
                if (MyMap.CartographicMode.ToString() == "Aerial")
                {
                    msg += "2";
                }
                else
                {
                    if (MyMap.CartographicMode.ToString() == "Hybrid")
                    {
                        msg += "3";
                    }
                    else
                    {
                        if (MyMap.CartographicMode.ToString() == "Terrain")
                        {
                            msg += "4";
                        }
                    }
                }
            }

            if (MyMap.ColorMode.ToString() == "Light")
            {
                msg += "1";
            }
            else
            {
                if (MyMap.ColorMode.ToString() == "Dark")
                {
                    msg += "2";
                }
            }

            if (MyMap.LandmarksEnabled)
            {
                msg += "1";
            }
            else
            {
                msg += "2";
            }

            if (MyMap.PedestrianFeaturesEnabled)
            {
                msg += "1";
            }
            else
            {
                msg += "2";
            }

            NavigationService.Navigate(new Uri("/Settings.xaml?msg=" + msg, UriKind.Relative));
        }

        private void Find(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Enabled)
                {
                    EnableAppButtons(false);
                    getLocation.Text = "Stop";
                    getLocation.IconUri = new Uri("/Images/dark.transport.stop.png", UriKind.Relative);
                    getLocation.IsEnabled = true;
                    MyMap.Focus();
                    query = new GeocodeQuery()
                    {
                        GeoCoordinate = new GeoCoordinate(0, 0),
                        SearchTerm = Search.Text
                    };
                    query.QueryCompleted += GetGeoCoordinate;
                    query.QueryAsync();
                }
            }
        }

        private void EnableAppButtons(bool value)
        {
            Enabled = value;

            Progress.IsVisible = !value;
            getLocation.IsEnabled = value;
            track.IsEnabled = value;
            calculate.IsEnabled = value;
            start.IsEnabled = value;
            settings.IsEnabled = value;
            instruction.IsEnabled = value;

            UpButton.IsEnabled = value;
            DownButton.IsEnabled = value;
            DeleteButton.IsEnabled = value;
            ClearButton.IsEnabled = value;
        }

        public Task<string> GetAddress(double latitude, double longitude)
        {
            string Address = "";
            var tcs = new TaskCompletionSource<string>();
            System.EventHandler<Microsoft.Phone.Maps.Services.QueryCompletedEventArgs<System.Collections.Generic.IList<Microsoft.Phone.Maps.Services.MapLocation>>> handler = null;
            var reverseGeocode = new ReverseGeocodeQuery();
            handler = (sender, args) =>
            {
                foreach (var address in args.Result.Select(adrInfo => adrInfo.Information.Address))
                {;
                    if (address.Street != "" && address.HouseNumber != "")
                    {
                        Address += address.Street + " ";
                        Address += address.HouseNumber + ",\n";
                    }
                    else
                    {
                        if (address.Street != "" && address.HouseNumber == "")
                        {
                            Address += address.Street + " ";
                        }
                    }
                    Address += address.City + ", ";
                    Address += address.Country;
                }
                if (Address == "")
                {
                    Address = latitude + ",\n" + longitude;
                }
                reverseGeocode.QueryCompleted -= handler;
                tcs.SetResult(Address);
            };

            reverseGeocode.GeoCoordinate = new GeoCoordinate(latitude, longitude);
            reverseGeocode.QueryCompleted += handler;
            reverseGeocode.QueryAsync();
            return tcs.Task;
        }

        private void GetGeoCoordinate(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in e.Result)
            {
                MyMap.Center = item.GeoCoordinate;
                MyMap.ZoomLevel = 16;
            }
            EnableAppButtons(true);
            getLocation.IconUri = new Uri("/Images/dark.cross.png", UriKind.Relative);
            getLocation.Text = "Get Location";
            query = null;
        }

        private void PitchUp(object sender, RoutedEventArgs e)
        {
            if (MyMap.Pitch > 0)
            {
                MyMap.Pitch -= 5;
            }
        }

        private void PitchDown(object sender, RoutedEventArgs e)
        {
            if (MyMap.Pitch < 75)
            {
                MyMap.Pitch += 5;
            }
        }

        private void SwapGeoPositions(int pos1, int pos2)
        {
            GeoCoordinate Swap = MyCoordinates[pos2];
            string Str = PointList.Items[pos2].ToString();

            int PointIndex1 = PointList.Items[pos1].ToString().IndexOf(".");
            int PointIndex2 = PointList.Items[pos2].ToString().IndexOf(".");
            int Number1 = Convert.ToInt32(PointList.Items[pos1].ToString().Substring(0, PointIndex1));
            int Number2 = Convert.ToInt32(PointList.Items[pos2].ToString().Substring(0, PointIndex2));

            PointList.Items[pos2] = pos2+1 + ". " + PointList.Items[pos1].ToString().Substring(PointIndex1 + 2);
            MyCoordinates[pos2] = MyCoordinates[pos1];

            PointList.Items[pos1] = pos1+1 + ". " + Str.Substring(PointIndex2 + 2);
            MyCoordinates[pos1] = Swap;
        }

        private void UpPoint(object sender, RoutedEventArgs e)
        {
            if (PointList.SelectedIndex > 0)
            {
                SwapGeoPositions(PointList.SelectedIndex - 1, PointList.SelectedIndex);
                if (MyMapRoute != null)
                {
                    MyMap.RemoveRoute(MyMapRoute);
                }
                MyMap.Layers.Clear();
                AddAllPushpins();
            }
        }

        private void DownPoint(object sender, RoutedEventArgs e)
        {
            if (PointList.SelectedIndex < MyCoordinates.Count - 1 && PointList.SelectedIndex > -1)
            {
                SwapGeoPositions(PointList.SelectedIndex, PointList.SelectedIndex + 1);
                GeoCoordinate tmp = MyCoordinates[PointList.SelectedIndex + 1];
                if (MyMapRoute != null)
                {
                    MyMap.RemoveRoute(MyMapRoute);
                }
                MyMap.Layers.Clear();
                AddAllPushpins();
            }
        }

        private void DeletePoint(object sender, RoutedEventArgs e)
        {
            if (PointList.SelectedIndex > -1)
            {
                int PointIndex;
                int index = PointList.SelectedIndex;
                PointList.Items.RemoveAt(index);
                MyCoordinates.Remove(MyCoordinates[index]);
                MyMap.Layers.Clear();
                for (int i = index; i < MyCoordinates.Count; i++)
                {
                    PointIndex = PointList.Items[i].ToString().IndexOf(".");
                    PointList.Items[i] = i + 1 + ". " + PointList.Items[i].ToString().Substring(PointIndex + 2);
                }
                AddAllPushpins();
                if (MyMapRoute != null)
                {
                    MyMap.RemoveRoute(MyMapRoute);
                }
            }
        }
        
        private void ClearPoints(object sender, RoutedEventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("All points will be removed. Are You sure?", "Clean Poinst", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.OK)
            {
                MyCoordinates.Clear();
                PointList.Items.Clear();
                MyMap.Layers.Clear();
                if (MyMapRoute != null)
                {
                    MyMap.RemoveRoute(MyMapRoute);
                }
            }
        }

        private void AddAllPushpins()
        {
            for (int i = 0; i < MyCoordinates.Count; i++)
            {
                AddPushpin(MyCoordinates[i], i + 1);
            }
        }

        private void ChangeViewPoint(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                MyMap.Center = MyCoordinates[PointList.SelectedIndex];
                Pivot.SelectedIndex = 0;
            }
            catch (System.ArgumentOutOfRangeException)
            {

            }
        }

    }

    public static class CoordinateConverter
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }
    }

}