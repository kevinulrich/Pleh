using System;
using System.Collections.Generic;
using System.Text;
using Pleh.Models;
using NAudio.Wave;

namespace Pleh.Services
{
    class ClipService
    {
        public Clip CreateClipFromSourceFile(string source)
        {
            Clip clip = new Clip(source);

            GetTimes(clip);

            return clip;
        }

        private void GetTimes(Clip clip)
        {
            WaveOutEvent wavePlayer = new WaveOutEvent();
            AudioFileReader sampleProvider = new AudioFileReader(clip.Source);

            clip.FadeOutStart = sampleProvider.TotalTime.TotalSeconds - 2;

            wavePlayer.Dispose();
            sampleProvider.Dispose();
        }
    }
}
