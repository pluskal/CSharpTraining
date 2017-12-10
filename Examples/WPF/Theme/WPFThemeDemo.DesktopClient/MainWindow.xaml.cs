// -----------------------------------------------------------------------------
//  <copyright file="MainWindow.xaml.cs" company="DCOM Engineering, LLC">
//      Copyright (c) DCOM Engineering, LLC.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace WPFThemeDemo.DesktopClient
{
    using System.Windows;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            this.Button1.IsEnabled = true;
        }
    }
}