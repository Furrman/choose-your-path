﻿#pragma checksum "D:\Choose Your Path\Choose Your Path\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C758CBE49F4674CFB256DC0C0EEF06C0"
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
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.ListPicker Style;
        
        internal Microsoft.Phone.Controls.ListPicker Color;
        
        internal Microsoft.Phone.Controls.ToggleSwitch Landmarks;
        
        internal Microsoft.Phone.Controls.ToggleSwitch Pedestrian;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Choose%20Your%20Path;component/Settings.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Style = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("Style")));
            this.Color = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("Color")));
            this.Landmarks = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("Landmarks")));
            this.Pedestrian = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("Pedestrian")));
        }
    }
}

