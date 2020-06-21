using System;
using Pleh.Models;
using Pleh.Services;
using ReactiveUI;
using Pleh.Services.AudioService;
using System.Timers;
using System.Globalization;

namespace Pleh.ViewModels
{
    public enum PlayerState
    {
        Empty,
        Paused,
        Playing
    }

    public class PlayerViewModel : ViewModelBase
    {
        public Player Player { get; private set; }
        private AudioTicket AudioTicket;
        private Timer Timer;

        public int ID { get; private set; }

        private string title = "NOT LOADED";
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private string secondsProgress = "-";
        public string SecondsProgress
        {
            get => secondsProgress;
            set => this.RaiseAndSetIfChanged(ref secondsProgress, value);
        }

        private string formattedTotal = "-:--";
        public string FormattedTotal
        {
            get => formattedTotal;
            set => this.RaiseAndSetIfChanged(ref formattedTotal, value);
        }

        private string secondsRemain = "-";
        public string SecondsRemain
        {
            get => secondsRemain;
            set => this.RaiseAndSetIfChanged(ref secondsRemain, value);
        }

        private int perthousandProgress = 0;
        public int PerthousandProgress
        {
            get => perthousandProgress;
            set => this.RaiseAndSetIfChanged(ref perthousandProgress, value);
        }

        public PlayerState State { get; private set; } = PlayerState.Empty;

        private readonly ClipService ClipService = new ClipService();

        public PlayerViewModel(Player player)
        {
            Player = player;

            ID = Player.ID;

            Timer = new Timer(100);

            Timer.Elapsed += (s, e) => {
                Timer.Stop();
                UpdateProgress();
                Timer.Start();
            };

            Timer.Start();

        }

        private void UpdateProgress()
        {
            if(AudioTicket == null)
            {
                SecondsProgress = "-";
                SecondsRemain = "-";
                return;
            }

            SecondsProgress = AudioTicket.GetProgress().ToString("+0", CultureInfo.InvariantCulture);

            double total = AudioTicket.GetDuration();
            double totalMinutes = Math.Floor(total / 60);
            FormattedTotal = totalMinutes.ToString("00") + ":" + (total - totalMinutes * 60).ToString("00");

            SecondsRemain = AudioTicket.GetRemaining().ToString("-00");

            PerthousandProgress = (int)Math.Floor(AudioTicket.GetProgress() / AudioTicket.GetDuration() * 1000);

            if(AudioTicket.GetProgress() >= Player.Clip.FadeOutStart)
            {
                FadeOut();
            }
        }

        public bool SetClip(Clip clip)
        {
            if(State == PlayerState.Playing)
            {
                return false;
            }

            if(AudioTicket != null)
            {
                Timer.Stop();
                AudioTicket.Dispose();
            }

            Player.Clip = clip;
            Title = Player.Clip.Title;

            State = PlayerState.Paused;

            AudioTicket = new AudioTicket(Player.Clip);
            Reset();
            Timer.Start();

            return true;
        }

        public double GetSecondsRemaining()
        {
            return AudioTicket.GetRemaining();
        }

        public double GetSecondsProgress()
        {
            return AudioTicket.GetProgress();
        }

        public void PlayButtonPress()
        {
            Play();
        }

        public void FadeInButtonPress()
        {
            FadeIn();
        }

        public void FadeOutButtonPress()
        {
            FadeOut();
        }

        public void PauseButtonPress()
        {
            Pause();
        }

        public void ResetButtonPress()
        {
            if(State != PlayerState.Paused)
            {
                return;
            }

            Reset();
        }

        public void Pause()
        {
            if(AudioTicket == null)
            {
                return;
            }

            AudioTicket.Pause();

            State = PlayerState.Paused;
        }

        public void Play()
        {
            if (AudioTicket == null)
            {
                return;
            }

            AudioTicket.Play();

            State = PlayerState.Playing;
        }

        public void FadeIn()
        {
            if (AudioTicket == null || State == PlayerState.Playing)
            {
                return;
            }

            AudioTicket.FadeIn();

            State = PlayerState.Playing;
        }

        public void FadeOut()
        {
            if (AudioTicket == null || State == PlayerState.Paused)
            {
                return;
            }

            AudioTicket.FadeOut();

            State = PlayerState.Paused;
        }

        public void Reset()
        {
            if (AudioTicket == null)
            {
                return;
            }

            AudioTicket.Reset();

            State = PlayerState.Paused;
        }
    }
}
