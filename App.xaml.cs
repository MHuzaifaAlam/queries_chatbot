using System;
using System.Windows;
using System.Threading.Tasks;

namespace quries_chatbot
{
    public partial class App : Application
    {
        // The Startup event handler
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // Show splash screen
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            // Simulate a loading time (10 seconds delay)
            await Task.Delay(10000); // Simulate 10 seconds loading

            // Show the main window after splash screen
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Close the splash screen
            splashScreen.Close();
        }
    }
}
