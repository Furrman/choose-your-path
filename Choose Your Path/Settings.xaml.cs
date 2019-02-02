using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Text;
using Microsoft.Phone.Tasks;

namespace Choose_Your_Path
{
    public partial class Settings : PhoneApplicationPage
    {

        private String MapSettings;
        private bool flag = false;

        public Settings()
        {
            InitializeComponent();
            MapSettings = "";
            flag = true;
        }

        private void Save(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?msg=" + MapSettings, UriKind.Relative));
            NavigationService.RemoveBackEntry();
            NavigationService.RemoveBackEntry();
            MessageBox.Show("Settings saved!");
        }

        private void Cancel(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MapSettings = "";
            String msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                MapSettings = msg;

                string s = "";
                int x;
                s += MapSettings[0];
                x = System.Convert.ToInt32(s) - 1;
                Style.SelectedIndex = x;

                s = "" + MapSettings[1];
                x = System.Convert.ToInt32(s) - 1;
                Color.SelectedIndex = x;
                
                if (MapSettings[2].Equals('1'))
                {
                    Landmarks.IsChecked = true;
                }
                else
                {
                    if (MapSettings[2].Equals('2'))
                    {
                        Landmarks.IsChecked = false;
                    }
                }

                if (MapSettings[3].Equals('1'))
                {
                    Pedestrian.IsChecked = true;
                }
                else
                {
                    if (MapSettings[3].Equals('2'))
                    {
                        Pedestrian.IsChecked = false;
                    }
                }
            }
        }

        private void StyleChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                StringBuilder x = new StringBuilder(MapSettings);

                if (Style.SelectedIndex == 0)
                {
                    x[0] = '1';
                }
                else
                {
                    if (Style.SelectedIndex == 1)
                    {
                        x[0] = '2';
                    }
                    else
                    {
                        if (Style.SelectedIndex == 2)
                        {
                            x[0] = '3';
                        }
                        else
                        {
                            if (Style.SelectedIndex == 3)
                            {
                                x[0] = '4';
                            }
                        }
                    }
                }
                MapSettings = x.ToString();
            }
        }

        private void ColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                StringBuilder x = new StringBuilder(MapSettings);

                if (Color.SelectedIndex == 0)
                {
                    x[1] = '1';
                }
                else
                {
                    if (Color.SelectedIndex == 1)
                    {
                        x[1] = '2';
                    }
                }
                MapSettings = x.ToString();
            }
        }

        private void LandmarksClick(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                StringBuilder x = new StringBuilder(MapSettings);

                if (Landmarks.IsChecked == true)
                {
                    x[2] = '1';
                }
                else
                {
                    if (Landmarks.IsChecked == false)
                    {
                        x[2] = '2';
                    }
                }
                MapSettings = x.ToString();
            }

        }

        private void PedestrianClick(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                StringBuilder x = new StringBuilder(MapSettings);

                if (Pedestrian.IsChecked == true)
                {
                    x[3] = '1';
                }
                else
                {
                    if (Pedestrian.IsChecked == false)
                    {
                        x[3] = '2';
                    }
                }
                MapSettings = x.ToString();
            }

        }

        private void OpenMapDownloader(object sender, RoutedEventArgs e)
        {
            MapDownloaderTask mapDownloaderTask = new MapDownloaderTask();
            mapDownloaderTask.Show();
        }

        private void OpenMapUpdater(object sender, RoutedEventArgs e)
        {
            MapUpdaterTask mapUpdaterTask = new MapUpdaterTask();
            mapUpdaterTask.Show();
        }
    }
}