using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Blotify
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			DataContext = new BlotifyViewModel();
			InitializeComponent();
		}

		private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.AbsoluteUri);
		}

		private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}