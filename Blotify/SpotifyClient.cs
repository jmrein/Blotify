using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;

namespace Blotify
{
	public class SpotifyClient : IDisposable
	{
		private readonly CompositeDisposable waitForPlayableDisposable = new CompositeDisposable();
		private readonly SpotifyLocalAPI api = new SpotifyLocalAPI();
		private readonly Subject<Unit> updateRequested = new Subject<Unit>();

		public SpotifyClient()
		{
			var stateChange = Observable.FromEventPattern<PlayStateEventArgs>(h => api.OnPlayStateChange += h, h => api.OnPlayStateChange -= h)
				.Select(s => Unit.Default)
				.Merge(updateRequested)
				.Select(playing => api.GetStatus());
			TrackChanged = Observable.FromEventPattern<TrackChangeEventArgs>(h => api.OnTrackChange += h, h => api.OnTrackChange -= h)
				.Select(t => t.EventArgs.NewTrack)
				.Merge(stateChange.Select(p => p.Track));
			TimeChanged = Observable.FromEventPattern<TrackTimeChangeEventArgs>(h => api.OnTrackTimeChange += h, h => api.OnTrackTimeChange -= h)
				.Select(t => t.EventArgs.TrackTime);
			IsPlaying = stateChange.Select(s => s.Playing);
			CanPlay = stateChange.Select(s => s.PlayEnabled);
			CanSkip = stateChange.Select(s => s.NextEnabled);
			CanPrevious = stateChange.Select(s => s.PrevEnabled);
		}

		public void Connect()
		{
			Start(SpotifyLocalAPI.IsSpotifyRunning, SpotifyLocalAPI.RunSpotify, "Spotify is not running!");
			Start(SpotifyLocalAPI.IsSpotifyWebHelperRunning, SpotifyLocalAPI.RunSpotifyWebHelper, "Web helper is not running!");
			Start(api.Connect, null, "Could not connect.");
			api.ListenForEvents = true;
			WaitForPlay();
			updateRequested.OnNext(Unit.Default);
		}

		/// <summary>
		/// Right after Spotify has launched, properties like CanPlay will be disabled.
		/// Give it a few tries to sort itself out.
		/// It may also be disabled for reasons, in which case, give up after a bit.
		/// </summary>
		private void WaitForPlay()
		{
			waitForPlayableDisposable.Add(CanPlay.Where(canPlay => canPlay).Subscribe(canPlay =>
			{
				waitForPlayableDisposable.Dispose();
			}));
			waitForPlayableDisposable.Add(Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(500)).Subscribe(i =>
			{
				updateRequested.OnNext(Unit.Default);
				if (i > 3)
				{
					waitForPlayableDisposable.Dispose();
				}
			}));
		}

		private static void Start(Func<bool> isRunning, Action starter, string errorMessage)
		{
			if (isRunning())
			{
				return;
			}
			starter?.Invoke();
			if (!isRunning())
			{
				throw new Exception(errorMessage);
			}
		}

		public IObservable<Track> TrackChanged { get; }
		public IObservable<double> TimeChanged { get; }
		public IObservable<bool> IsPlaying { get; }
		public IObservable<bool> CanPlay { get; }
		public IObservable<bool> CanSkip { get; }
		public IObservable<bool> CanPrevious { get; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				waitForPlayableDisposable.Dispose();
				api.Dispose();
			}
		}

		public void Previous()
		{
			api.Previous();
		}

		public async void Play()
		{
			await api.Play().ContinueWith(_ => updateRequested.OnNext(Unit.Default));
		}

		public async void Pause()
		{
			await api.Pause().ContinueWith(_ => updateRequested.OnNext(Unit.Default));
		}

		public void Next()
		{
			api.Skip();
		}
	}
}
