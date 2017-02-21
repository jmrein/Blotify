using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using ReactiveUI;

namespace Blotify
{
	internal class BlotifyViewModel : ReactiveObject, IDisposable
	{
		private CurrentTrack currentTrack;
		private double currentTime;
		private bool isPlaying;
		private bool showDetails = true;
		private readonly SpotifyClient spotify;
		private readonly IDisposable disposable;

		public CurrentTrack CurrentTrack
		{
			get { return currentTrack; }
			private set { this.RaiseAndSetIfChanged(ref currentTrack, value); }
		}

		public double CurrentTime
		{
			get { return currentTime; }
			private set { this.RaiseAndSetIfChanged(ref currentTime, value); }
		}

		public bool IsPlaying
		{
			get { return isPlaying; }
			private set { this.RaiseAndSetIfChanged(ref isPlaying, value); }
		}

		public bool ShowDetails
		{
			get { return showDetails; }
			private set { this.RaiseAndSetIfChanged(ref showDetails, value); }
		}

		public ReactiveCommand PlayCommand { get; }
		public ReactiveCommand PreviousCommand { get; }
		public ReactiveCommand NextCommand { get; }
		public ReactiveCommand ToggleDetails { get; }

		public BlotifyViewModel(SpotifyClient spotify)
		{
			this.spotify = spotify;
			PlayCommand = ReactiveCommand.Create(PlayPause, spotify.CanPlay);
			PreviousCommand = ReactiveCommand.Create(spotify.Previous, spotify.CanPrevious);
			NextCommand = ReactiveCommand.Create(spotify.Next, spotify.CanSkip);
			ToggleDetails = ReactiveCommand.Create(() => ShowDetails = !ShowDetails, Observable.Return(true));
			disposable = new CompositeDisposable(
				spotify.TrackChanged.ObserveOn(Dispatcher.CurrentDispatcher).Where(track => track != null).Subscribe(track => CurrentTrack = new CurrentTrack(track)),
				spotify.TimeChanged.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(time => CurrentTime = time),
				spotify.IsPlaying.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(v => IsPlaying = v),
				PlayCommand, PreviousCommand, NextCommand, ToggleDetails,
				spotify, PlayCommand, PreviousCommand, NextCommand);
			spotify.Connect();
		}

		public void PlayPause()
		{
			if (IsPlaying)
			{
				spotify.Pause();
			}
			else
			{
				spotify.Play();
			}
		}

		public void Dispose()
		{
			disposable.Dispose();
		}
	}
}