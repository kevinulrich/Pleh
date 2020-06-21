using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using Pleh.Models;
using System.Timers;
using System.Diagnostics;

namespace Pleh.Services.AudioService
{
    enum FadeDirection
    {
        In,
        Out
    }

    /*
     * TODO: Decouple ticket from player implementation by using a service to create tickets
     */
    public class AudioTicket : IDisposable
    {
        private readonly IWavePlayer WavePlayer;
        private readonly WaveChannel32 SampleProvider;
        private readonly Clip Clip;

        private const int FadeResolution = 10;

        public AudioTicket(Clip clip)
        {
            Clip = clip;

            WavePlayer = new WaveOutEvent();
            SampleProvider = new WaveChannel32(new AudioFileReader(Clip.Source),1f,0f);

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

        private void Fade(FadeDirection direction, double duration)
        {
            
            Timer fadeTimer = new Timer(FadeResolution);
            decimal volume = direction == FadeDirection.In ? 0.00m : 1.00m;
            decimal volumeSteps = 1 / (Convert.ToDecimal(duration) / (Convert.ToDecimal(FadeResolution) / 1000));

            if(direction == FadeDirection.Out)
            {
                volumeSteps = -1 * volumeSteps;
            }

            SampleProvider.Volume = (float)volume;

            Play();

            fadeTimer.Elapsed += (s, e) => {
                fadeTimer.Stop();
                volume += volumeSteps;

                if (direction == FadeDirection.In && volume <= 1.0m)
                {
                    SampleProvider.Volume = (float)volume;
                    fadeTimer.Start();
                    return;
                }

                if (direction == FadeDirection.Out && volume >= 0.0m)
                {
                    SampleProvider.Volume = (float)volume;
                    fadeTimer.Start();
                    return;
                }

                if (direction == FadeDirection.Out)
                {
                    Pause();
                    return;
                }

                fadeTimer.Dispose();
            };

            fadeTimer.Start();
        }

        public void FadeIn()
        {
            if(Clip.FadeInLength <= 0)
            {
                Play();
                return;
            }

            Fade(FadeDirection.In, Clip.FadeInLength);
           
        }

        public void FadeOut()
        {
            if (Clip.FadeOutLength <= 0)
            {
                Pause();
                return;
            }

            Fade(FadeDirection.Out, Clip.FadeOutLength);
        }

        public void Reset()
        {
            WavePlayer.Stop();
            SetProgress(Clip.FadeInStart);
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
            if(SampleProvider == null)
            {
                return 0;
            }

            return SampleProvider.CurrentTime.TotalSeconds;
        }

        public void SetProgress(double progress)
        {
            SampleProvider.CurrentTime = new TimeSpan(0, 0, Convert.ToInt32(progress));
        }

        public void Dispose()
        {
            WavePlayer.Dispose();
            SampleProvider.Dispose();
        }
    }
}
