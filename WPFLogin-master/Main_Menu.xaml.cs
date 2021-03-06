﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Main_Menu.xaml
    /// </summary>
    /// 

    public partial class Main_Menu : Window
    {
        Stopwatch stopwatch = new Stopwatch();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public delegate void NextPrimeDelegate();

        bool isSearching = false;
        string myImage = "";

        //(put in every function that uses it) 

        private static UI_Network comms = new UI_Network();


        // Constructor
        public Main_Menu()
        {
            InitializeComponent();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += backgroundWorker_DoWork;

            

            // Initializes and starts the thread.
            var th1 = new Thread(Threaded_Network);
            //th1.IsBackground = true;
            th1.Start();


            if (comms.isConnected())
            {
                status_box.Foreground = Brushes.Green;
                status_box.Content = "Connection Successful! ";
            }


            // var th2 = new Thread(Safe_to_Fly);
            // th2.Start();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            FTPImageTransfer transfer = null; //new FTPImageTransfer("ftp://192.168.168.1", "drone", "NEVERAGAIN");
            UI_Network comms = new UI_Network();
            //begins the drone search process by having the user select an image of
            //the target
            if (!isSearching)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    status_box.Foreground = Brushes.Yellow;
                    status_box.Content = "User Selecting Target. ";

                    status_label.Foreground = Brushes.Yellow;
                    status_label.Content = "Standby";
                }));

                // Makes filepicker window object.
                var openFileDialog = new OpenFileDialog();
                // Only allows Images.
                openFileDialog.Filter = "Image Files (*.png, *.jpg,*.bmp)|*.png;*.jpg;*.bmp";
                openFileDialog.FilterIndex = 1;
                // Sets default directory when opening.
                openFileDialog.InitialDirectory = @"C:\";
                // Title of image picker window.
                openFileDialog.Title = "Select Image of person to be searched for";

                bool? click_ok = openFileDialog.ShowDialog();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    // Sets variable myImage to the uploaded image.
                    myImage = openFileDialog.FileName;
                    // Sets image box to the image path in myImage.
                    search_for_image.Source = setImage();

                }));

                // Attempts to send user-selected image to server for drone search.
                try
                {
                    if (click_ok == true)
                    {
                        // Makes a new FTP connection to the server.
                        transfer = new FTPImageTransfer("ftp://192.168.168.1", "drone", "NEVERAGAIN");
                        // Uploads image with name of ui_image.png
                        string name = "ui_image.png";

                        // Sends image file to server.
                        transfer.Upload(openFileDialog.FileName, name);

                        for (int i = 0; i <= 101; i++)
                        {
                            //Reports the progress of the upload.
                            // Thread.Sleep(100); Testing the upload button
                            backgroundWorker.ReportProgress(i);
                        }

                        comms.SendImage(name);
                        comms.SendStart();

                        // Sets Dialog box's color and text with respect to the image upload result
                        status_box.Foreground = Brushes.Green;
                        if (comms.getUploadStatus())
                            status_box.Content = "Image succesfully uploaded, Searching for new target. ";
                        else
                        {
                            status_box.Foreground = Brushes.Red;
                            status_box.Content = "Image failed to upload, Drone not started. ";
                        }

                        status_label.Foreground = Brushes.Green;
                        status_label.Content = "Active";

                        // Starts the timer, if user picks new target timer does not reset
                        //if (!(stopwatch.Elapsed.Seconds > 0))
                          //  stopwatch.Start();
                        // stopwatch.Stop();
                        //timer_label.Content = stopwatch.Elapsed;

                    }
                    else if (click_ok == false)
                    {
                        // Resets the upload fail/pass dialog box
                        //status_box.Foreground = reset

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            status_box.Content = " ";

                            status_label.Foreground = Brushes.Black;
                            status_label.Content = "N/A";
                        }));
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.TargetSite);

                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        status_label.Foreground = Brushes.Red;
                        status_label.Content = "Inactive";
                    }));
                }

            }
            else
            {

            }


        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            for (int i = 0; i <= theprogressbar.Value; i++)
            {
                percentageprogress.Content = i + " %";
            }

            theprogressbar.Value = e.ProgressPercentage;
        }

        // Attemps to establish connection to server, if successful status box displays
        // a successful connection message, otherwise states the connection failed
        public void Threaded_Network()
        {
            
            while (true)
            {
                //int temp = 0;
                this.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        voltage_label.Content = comms.GetVoltage() + "V";
                        safe_to_fly_status.Content = comms.GetSafe().ToString();

                    }
                    catch (TimeoutException ex)
                    {
                        status_box.Content = ex;
                    }
                    
                   //temp++;
                   //Console.WriteLine(temp);

               }));

               Thread.Sleep(1000);
            }
        }

        /*
        // Threaded network connection.
        public partial class ThreadedWorker : Main_Menu
        {
            int ID;
            Thread t;
            public ThreadedWorker(int ID)
            {
                this.ID = ID;
                t = new Thread(new ThreadStart(Network_initialize));
                t.Start();
            }
            void Network_initialize()
            {
                try
                {
                    status_box.Foreground = Brushes.Yellow;
                    status_box.Content = "Attempting to Connect to Server... ";
                    UI_Network Comm = new UI_Network();
                }
                catch (Exception ex)
                {
                    status_box.Foreground = Brushes.Red;
                    status_box.Content = "Could not Connect to the Server. ";
                }
                status_box.Foreground = Brushes.Green;
                status_box.Content = "Connection Successful! ";
            }
        }
        */

        // Returns to the main menu page.
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri("test.xaml", UriKind.Relative));
        }

        //mouse hover - home
        //TODO - remove and put in designer properties/XAML
        private void Home_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        // Closes the application window, logs current user out, 
        // then re-opens login window.
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var newW = new login();
            newW.Show();

            this.Close();
        }

        // Send signal to drone to commence searching for the selected target.
        //TODO - handle new search while already searching 
        private void Start_Search_Click(object sender, RoutedEventArgs e)
        {
            // Resets the upload fail/pass dialog box

            if (!isSearching)
            {
                Continue_Searching.Visibility = Visibility.Visible;
                Start_Search.Content = "New Search";
                try
                {
                    // Bulk of the work is here so user can interact with the UI while uploading a picture
                    backgroundWorker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }

            }

        }

        // Sends signal to drone for the search to cease.
        private void Stop_Button1_Click(object sender, RoutedEventArgs e)
        {
            isSearching = false;

            // Reset ImageBox images.
            search_for_image.Source = null;
            person_found.Source = null;

            // Sets ouput log to respective color and text.
            status_box.Foreground = Brushes.Red;
            status_box.Content = "Drone Stopped. ";

            // Sets Drone Activity Label to respective color and status.
            status_label.Foreground = Brushes.Red;
            status_label.Content = "Inactive";

            // RetrieveImage();

            try
            {
                comms.SendStop();
            }
            catch
            {
                Console.Write("");
            }
        }

        // Sends signal for drone to continue it's search and find a new target.
        private void Continue_Searching_Click(object sender, RoutedEventArgs e)
        {
            //TODO - make drone continue searching

            status_box.Foreground = Brushes.Green;
            status_box.Content = "continuing Search. ";
        }

        // Forces the drone to stop.
        private void KillSwitch_Click(object sender, RoutedEventArgs e)
        {
            comms.SendKill();
        }

        // When drone finds target, function sets ImageBox to UI.
        void RetrieveImage()
        {
            var Network = new UI_Network();

            myImage = "/*set to the incoming image**/";

            person_found.Source = setImage();
        }

        // Returns a usable image converted from OpenFileDialog sourcestream.
        BitmapImage setImage()
        {
            var b = new BitmapImage();
            b.BeginInit();
            try
            {
                b.UriSource = new Uri(myImage);
                b.EndInit();

            }
            catch
            {

            }
            return b;
        }
    }
}
