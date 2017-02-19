using System;
using System.Reactive.Subjects;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;

namespace Blotify
{
	internal class SpotifyClient : IDisposable
	{
		private readonly SpotifyLocalAPI api = new SpotifyLocalAPI();
		private readonly ISubject<Track> trackChanged = new ReplaySubject<Track>();
		private readonly ISubject<double> timeChanged = new ReplaySubject<double>();
		private readonly ISubject<bool> isPlaying = new BehaviorSubject<bool>(false);
		private readonly ISubject<bool> canPlay = new BehaviorSubject<bool>(false);
		private readonly ISubject<bool> canSkip = new BehaviorSubject<bool>(false);
		private readonly ISubject<bool> canPrevious = new BehaviorSubject<bool>(false); 

		public void Connect()
		{
			if (!SpotifyLocalAPI.IsSpotifyRunning())
			{
				throw new Exception("Spotify is not running!");
			}
			if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
			{
				throw new Exception("SpotifyWebHelper is not running!");
			}
			if (!api.Connect())
			{
				throw new Exception("Could not connect to Spotify!");
			}
			api.ListenForEvents = true;
			UpdateStatus(api.GetStatus());
			api.OnTrackChange += (o, e) => trackChanged.OnNext(e.NewTrack);
			api.OnTrackTimeChange += (o, e) => timeChanged.OnNext(e.TrackTime);
			api.OnPlayStateChange += (o, e) => UpdateStatus(api.GetStatus());
		}

		public IObservable<Track> TrackChanged => trackChanged;
		public IObservable<double> TimeChanged => timeChanged;
		public IObservable<bool> IsPlaying => isPlaying;
		public IObservable<bool> CanPlay => canPlay;
		public IObservable<bool> CanSkip => canSkip;
		public IObservable<bool> CanPrevious => canPrevious;

		public void Dispose()
		{
			api.Dispose();
		}

		private void UpdateStatus(StatusResponse status)
		{
			isPlaying.OnNext(status.Playing);
			canPlay.OnNext(status.PlayEnabled);
			canPrevious.OnNext(status.PrevEnabled);
			canSkip.OnNext(status.NextEnabled);
			if (status.Playing)
			{
				trackChanged.OnNext(status.Track);
				timeChanged.OnNext(status.PlayingPosition);
			}
		}

		public void Previous()
		{
			api.Previous();
		}

		public async void Play()
		{
			await api.Play();
		}

		public async void Pause()
		{
			await api.Pause();
		}

		public void Next()
		{
			api.Skip();
		}
	}
}
