using System;
using System.Collections.Generic;
using System.Text;
using Pleh.Models;
using ReactiveUI;
using WaveFormRendererLib;
using System.Drawing;
using Avalonia.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using Pleh.Services;
using Pleh.Services.AudioService;
using System.Timers;
using Pleh.Views;
using System.Diagnostics;

namespace Pleh.ViewModels
{
    public class MetaWindowViewModel : ViewModelBase
    {
        private Clip Clip;
        private AudioTicket AudioTicket;
        private ClipService ClipService;
        private Timer Timer;
        private MetaWindow MetaWindow;

        private string artist;
        public string Artist
        {
            get => artist;
            set => this.RaiseAndSetIfChanged(ref artist, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private double progress;
        public double Progress
        {
            get => progress;
            set
            {
                if(value != progress)
                {
                    AudioTicket.SetProgress(Convert.ToInt64(value));
                }

                progress = value;
                this.RaisePropertyChanged();
            }
        }

        private double duration;
        public double Duration
        {
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }

        private Avalonia.Media.Imaging.Bitmap image;
        public Avalonia.Media.Imaging.Bitmap Image
        {
            get => image;
            set => this.RaiseAndSetIfChanged(ref image, value);
        }

        private double fadeInStart;
        public double FadeInStart
        {
            get => fadeInStart;
            set => this.RaiseAndSetIfChanged(ref fadeInStart, value);
        }

        private double fadeOutStart;
        public double FadeOutStart
        {
            get => fadeOutStart;
            set => this.RaiseAndSetIfChanged(ref fadeOutStart, value);
        }

        private double fadeInLength;
        public double FadeInLength
        {
            get => fadeInLength;
            set => this.RaiseAndSetIfChanged(ref fadeInLength, value);
        }

        private double fadeOutLength;
        public double FadeOutLength
        {
            get => fadeOutLength;
            set => this.RaiseAndSetIfChanged(ref fadeOutLength, value);
        }

        private double rampIn;
        public double RampIn
        {
            get => rampIn;
            set => this.RaiseAndSetIfChanged(ref rampIn, value);
        }

        private double rampOut;
        public double RampOut
        {
            get => rampOut;
            set => this.RaiseAndSetIfChanged(ref rampOut, value);
        }

        public MetaWindowViewModel(Clip clip, MetaWindow window)
        {
            Clip = clip;
            MetaWindow = window;

            AudioTicket = new AudioTicket(Clip);
            ClipService = new ClipService();

            Title = Clip.Title;
            Artist = Clip.Artist;
            FadeInStart = Clip.FadeInStart;
            FadeOutStart = Clip.FadeOutStart;
            FadeInLength = Clip.FadeInLength;
            FadeOutLength = Clip.FadeOutLength;
            RampIn = Clip.RampIn;
            RampOut = Clip.RampOut;

            Image = GetClipVisualization();

            Timer = new Timer(100);

            Timer.Elapsed += (s, e) => {
                Timer.Stop();
                UpdateProgress();
                Timer.Start();
            };

            Timer.Start();

            MetaWindow.Closing += (s, e) =>
            {
                Timer.Stop();
                AudioTicket.Dispose();
            };
        }

        private void UpdateProgress()
        {
            if (AudioTicket == null)
            {
                return;
            }

            progress = AudioTicket.GetProgress();
            Progress = progress;

            Duration = AudioTicket.GetDuration();

            if (AudioTicket.GetRemaining() == 0)
            {
                Pause();
            }
        }


        public void Play()
        {
            AudioTicket.Play();
        }

        public void Pause()
        {
            AudioTicket.Pause();
        }

        public void ApplyToClip()
        {
            Clip.Title = Title;
            Clip.Artist = Artist;
            Clip.FadeInStart = FadeInStart;
            Clip.FadeInLength = FadeInLength;
            Clip.FadeOutStart = FadeOutStart;
            Clip.FadeOutLength = FadeOutLength;
            Clip.RampIn = RampIn;
            Clip.RampOut = RampOut;
        }

        public void ApplyToClipAndSave()
        {
            ApplyToClip();
            ClipService.WriteMeta(Clip);
        }

        private Avalonia.Media.Imaging.Bitmap GetClipVisualization()
        {
            RmsPeakProvider maxPeakProvider = new RmsPeakProvider(1);

            StandardWaveFormRendererSettings myRendererSettings = new StandardWaveFormRendererSettings
            {
                Width = 600,
                TopHeight = 32,
                BottomHeight = 32
            };

            WaveFormRenderer renderer = new WaveFormRenderer();
            string audioFilePath = Clip.Source;

            Image originalDrawing = renderer.Render(audioFilePath, maxPeakProvider, myRendererSettings);

            MemoryStream stream = new MemoryStream();
            originalDrawing.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;

            Avalonia.Media.Imaging.Bitmap image = new Avalonia.Media.Imaging.Bitmap(stream);
            
            return image;
        }
    }
}
