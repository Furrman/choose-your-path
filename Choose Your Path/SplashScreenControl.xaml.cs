using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Choose_Your_Path
{
    public partial class SplashScreenControl : UserControl
    {
        public SplashScreenControl(bool dark)
        {
            InitializeComponent();
            this.progressBar1.IsIndeterminate = true;
            if (dark)
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);
                textBlock1.Foreground = new SolidColorBrush(Colors.White);
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri("/Images/light.logo.png", UriKind.Relative);
                Image.Source = bi;
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.White);
                textBlock1.Foreground = new SolidColorBrush(Colors.Black);
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri("/Images/dark.logo.png", UriKind.Relative);
                Image.Source = bi;
            }
        }
    }
}
