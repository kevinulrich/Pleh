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

namespace Pleh.ViewModels
{
    public class MetaWindowViewModel : ViewModelBase
    {
        private Clip Clip;
        private AudioTicket AudioTicket;
        private Timer Timer;

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

        public MetaWindowViewModel(Clip clip)
        {
            Clip = clip;
            AudioTicket = new AudioTicket(Clip);

            Title = Clip.Title;
            Artist = Clip.Artist;
            Image = GetClipVisualization();

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
