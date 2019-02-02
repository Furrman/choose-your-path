using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace Choose_Your_Path
{
    public partial class Instruction : PhoneApplicationPage
    {
        public Instruction()
        {
            InitializeComponent();
            BitmapImage bi = new BitmapImage();
            WelcomeText.Text = "Within the framework an application was devised for Windows Phone, which searches for optimal connection between points. The software has the possibility of choosing the way of determining connections or converting it to the amount of gathered data. API was employed in the programming to display maps and to determine the location based on GPS readings.";
            PointsText.Text = "Application allow user to add points by holding finger on the map screen. All points are gathered on Points list, where user could freely manipulate their position or remove them from the list. All operation on the points are invoked by buttons on the left side of Points page. Before running operation user should check one point from the list by simple tapping. The are only one exception in the last button, which clear all points from the list. Application does not need to select any points to remove all points, but this requires additional user confirmation. User can also go to selected point by double tapping at this point";
            FunctionsText.Text = "User can start main functionality of this application from virtually any views of main page. Most of the function are located in applicationbar buttons. This buttons can invoke to get current phone position on the map, track continuously it position, shuffle points to get best queue to all of them and show it on the map with tips. There are also additional functionality placed on the map screen. To this control user can put address to localize position of written address. All those operations can operate for a long period of time, so here come new button to stop them. This button shows up only during running process to stop it.";
            SettingsText.Text = "This function are located on new page and could be showed up after choosing the corresponding menu item. Item redirect user to new page with application settings and allow him to set different elements of map to change it form. In current version there are placed controls to set map color and style and also turn on or off landmarks and pedestrian objects. Second page contains two buttons to administer map downloader and updater";
            if (Visibility.Visible == (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"])
            {
                bi.UriSource = new Uri("/Images/dark.logo.png", UriKind.Relative);
                WelcomeImage.Source = bi;
            }
            else
            {
                bi.UriSource = new Uri("/Images/light.logo.png", UriKind.Relative);
                WelcomeImage.Source = bi;
            }
        }
    }
}