using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
		private readonly SpotifyClient spotify = new SpotifyClient();
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

		public BlotifyViewModel()
		{
			PlayCommand = new ReactiveCommand(spotify.CanPlay);
			PreviousCommand = new ReactiveCommand(spotify.CanPrevious);
			NextCommand = new ReactiveCommand(spotify.CanSkip);
			ToggleDetails = new ReactiveCommand(Observable.Return(true));
			disposable = new CompositeDisposable(
				spotify.TrackChanged.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(track => CurrentTrack = new CurrentTrack(track)),
				spotify.TimeChanged.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(time => CurrentTime = time),
				spotify.IsPlaying.ObserveOn(Dispatcher.CurrentDispatcher).Subscribe(v => IsPlaying = v),
				PlayCommand.Subscribe(_ => PlayPause()),
				PreviousCommand.Subscribe(_ => spotify.Previous()),
				NextCommand.Subscribe(_ => spotify.Next()),
				ToggleDetails.Subscribe(_ => ShowDetails = !ShowDetails),
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