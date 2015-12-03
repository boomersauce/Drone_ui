﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Main_Menu.xaml
    /// </summary>
    /// 
    public partial class Main_Menu : Window
    {

        bool isSearching = false;
        //String dir_path = @"%USERPROFILE%\\My Documents\\Downloads";

        private System.Windows.Forms.MonthCalendar monthCalendar1;

        public Main_Menu()
        {
            InitializeComponent();
            DataContext = this;
            ImageUri = "user-1-glyph-icon_MkuBPp8O.png";
            //found_image = "";
        }

        //----Imageuri-start-----\\
        public static readonly DependencyProperty ImageUriProperty =
        DependencyProperty.Register("ImageUri", typeof(string), typeof(Main_Menu));

        public string ImageUri
        {
            get { return (string)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }
        //----Imageuri-end-----\\


        private void Home_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri("test.xaml", UriKind.Relative));
        }

        //mouse hover - home
        private void Home_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var newW = new login();
            newW.Show();

            this.Close();
        }
        private void No_click(object sender, RoutedEventArgs e)
        {

        }
        private void Yes_click(object sender, RoutedEventArgs e)
        {

        }

        //TODO - make it not crash
        private void label3_Initialized(object sender, EventArgs e)
        {
         // this.Welcome_text.Content = this.monthCalendar1.SelectionRange.Start.ToShortDateString();
        }
        //
        private void Start_Search_Click(object sender, RoutedEventArgs e)
        {
            if (!isSearching)
            {
                //shows continue searching button
                Continue_Searching.Visibility = Visibility.Visible;
                Start_Search.Content = "New Search";

                //makes filepicker window object
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //Only allows Images
                openFileDialog.Filter = "Image Files (*.png, *.jpg,*.bmp)|*.png;*.jpg;*.bmp";
                //sets default directory when opening
                openFileDialog.InitialDirectory = @"C:\";
                //Title of image picker window
                openFileDialog.Title = "Select Image of person to be searched for";




                //idk
                if (openFileDialog.ShowDialog() == true)
                   // txtEditor.Text = File.ReadAllText(openFileDialog.FileName);

                ImageUri = openFileDialog.FileName;

                FTPImageTransfer transfer = new FTPImageTransfer("192.168.168.1", "drone", "NEVERAGAIN");

                //transfer.Upload_2("", name + DateTime.Now);
            }
            else
            {
                isSearching = true;

            }
        }

        private void Stop_Button1_Click(object sender, RoutedEventArgs e)
        {
            isSearching = false;
            ImageUri = "user-1-glyph-icon_MkuBPp8O.png";
            
        }
    }
}
