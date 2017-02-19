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

		private void Info_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("http://github.com/jmrein/Blotify");
		}
	}
}