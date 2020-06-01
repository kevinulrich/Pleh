using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using Pleh.Models;

namespace Pleh.Services.AudioService
{
    public class AudioTicket : IDisposable
    {
        private readonly IWavePlayer WavePlayer;
        private readonly WaveStream SampleProvider;
        private readonly Clip Clip;

        public AudioTicket(Clip clip)
        {
            Clip = clip;

            WavePlayer = new WaveOutEvent();
            SampleProvider = new AudioFileReader(Clip.Source);

            WavePlayer.Init(SampleProvider);
        }

        public void Play()
        {
            WavePlayer.Play();
        }

        public void Pause()
        {
            WavePlayer.Pause();
        }

        public void Reset()
        {
            WavePlayer.Stop();
            SampleProvider.Position = 0;
        }

        public double GetDuration()
        {
            return SampleProvider.TotalTime.TotalSeconds;
        }

        public double GetRemaining()
        {
            return GetDuration() - GetProgress();
        }

        public double GetProgress()
        {
            return SampleProvider.CurrentTime.TotalSeconds;
        }

        public void Dispose()
        {
            WavePlayer.Dispose();
            SampleProvider.Dispose();
        }
    }
}
