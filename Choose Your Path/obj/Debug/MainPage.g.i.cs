﻿#pragma checksum "D:\Inżynier\Choose Your Path\Choose Your Path\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4BC133D168561FE86A4DFF4D3025D5BD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Choose_Your_Path {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot Pivot;
        
        internal Microsoft.Phone.Maps.Controls.Map MyMap;
        
        internal System.Windows.Controls.TextBox Search;
        
        internal System.Windows.Controls.ListBox PointList;
        
        internal System.Windows.Controls.Button UpButton;
        
        internal System.Windows.Controls.Button DownButton;
        
        internal System.Windows.Controls.Button DeleteButton;
        
        internal System.Windows.Controls.Button ClearButton;
        
        internal System.Windows.Controls.TextBlock Route;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Choose%20Your%20Path;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Pivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("Pivot")));
            this.MyMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("MyMap")));
            this.Search = ((System.Windows.Controls.TextBox)(this.FindName("Search")));
            this.PointList = ((System.Windows.Controls.ListBox)(this.FindName("PointList")));
            this.UpButton = ((System.Windows.Controls.Button)(this.FindName("UpButton")));
            this.DownButton = ((System.Windows.Controls.Button)(this.FindName("DownButton")));
            this.DeleteButton = ((System.Windows.Controls.Button)(this.FindName("DeleteButton")));
            this.ClearButton = ((System.Windows.Controls.Button)(this.FindName("ClearButton")));
            this.Route = ((System.Windows.Controls.TextBlock)(this.FindName("Route")));
        }
    }
}

