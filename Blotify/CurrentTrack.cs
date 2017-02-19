using System;
using System.Windows;
using System.Windows.Media.Imaging;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;

namespace Blotify
{
	internal class CurrentTrack
	{
		public BitmapSource AlbumArt { get; set; }
		public SpotifyResource Song { get; set; }
		public SpotifyResource Album { get; set; }
		public SpotifyResource Artist { get; set; }

		public CurrentTrack(Track track)
		{
			AlbumArt = LoadBitmap(track);
			Song = track.TrackResource;
			Album = track.AlbumResource;
			Artist = track.ArtistResource;
		}

		private static BitmapSource LoadBitmap(Track track)
		{
			var ptr = IntPtr.Zero;
			try
			{
				var source = track.GetAlbumArt(AlbumArtSize.Size160);
				ptr = source.GetHbitmap();
				return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr,
					IntPtr.Zero, Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				NativeMethods.DeleteObject(ptr);
			}
		}
	}
}