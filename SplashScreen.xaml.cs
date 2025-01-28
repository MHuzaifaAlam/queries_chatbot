using System;
using System.Windows;
using System.Windows.Threading;

namespace quries_chatbot
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();

            // Simulate a loading time with a delay
            System.Threading.Tasks.Task.Delay(10000).ContinueWith(t =>
            {
                // Once the delay is over, close splash screen and show main window
                Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                });
            });
        }
    }
}
