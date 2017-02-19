using System;
using System.Windows;

namespace Blotify
{
    public partial class App : Application
    {
	    private void App_OnStartup(object sender, StartupEventArgs e)
	    {
		    var spotify = new SpotifyClient();
			try
			{
				spotify.Connect();
				new MainWindow {DataContext = new BlotifyViewModel(spotify)}.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				Shutdown();
			}
		}
    }
}
