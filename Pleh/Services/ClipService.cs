using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Pleh.Models;
using NAudio.Wave;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Pleh.Services
{
    class ClipService
    {
        private readonly string MetaDirectory;
        private readonly string TrackMetaDirectory;

        public ClipService()
        {
            MetaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pleh\\";
            TrackMetaDirectory = Path.Combine(MetaDirectory, "TrackMeta");
        }

        public Clip CreateClipFromSourceFile(string source)
        {
            Clip clip = new Clip(source);

            clip.Artist = "Unnamed";
            clip.Title = Path.GetFileName(source);

            PrepareFolders();

            if (!GetMeta(ref clip))
            {
                SetDefaultMeta(clip);
                GetTimes(clip);
                WriteMeta(clip);
            }
            
            return clip;
        }

        private void SetDefaultMeta(Clip clip)
        {
            clip.Type = ClipType.Music;

            TagLib.File file = TagLib.File.Create(clip.Source);
            clip.Title = file.Tag.Title;
            clip.Artist = String.Join(", ", file.Tag.Performers);
        }

        private void PrepareFolders()
        {
            if (!Directory.Exists(TrackMetaDirectory))
            {
                Directory.CreateDirectory(TrackMetaDirectory);
            }
        }

        public void GetTimes(Clip clip)
        {
            WaveOutEvent wavePlayer = new WaveOutEvent();
            AudioFileReader sampleProvider = new AudioFileReader(clip.Source);

            int bytesPerSample = (sampleProvider.WaveFormat.BitsPerSample / 8);
            var samples = sampleProvider.Length / (bytesPerSample);

            var samplesPerSecond = samples / sampleProvider.TotalTime.TotalSeconds;

            int blockSize = 12;
            float[] readBuffer = new float[samples];

            var samplesRead = sampleProvider.Read(readBuffer, 0, readBuffer.Length);

            bool setIn = false;

            for (int x = 0; x < samplesRead; x += blockSize)
            {
                double total = 0.0;

                for (int y = 0; y < blockSize && x + y < samplesRead; y++)
                {
                    total += readBuffer[x + y] * readBuffer[x + y];
                }

                float rms = (float)Math.Sqrt(total / blockSize);

                if(rms > 0.1)
                {
                    double secondsFromStart = x / samplesPerSecond;

                    if(!setIn)
                    {
                        clip.FadeInStart = secondsFromStart;
                        setIn = true;
                    }
                    

                    clip.FadeOutStart = secondsFromStart;
                }
            }

            wavePlayer.Dispose();
            sampleProvider.Dispose();
        }

        private string GetClipHash(Clip clip)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(clip.Source))
                {
                    byte[] hash = md5.ComputeHash(stream);

                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                        sb.Append(b.ToString("X2"));

                    return sb.ToString();
                }
            }
        }

        public void WriteMeta(Clip clip)
        {
            string hash = GetClipHash(clip);

            string jsonString = JsonSerializer.Serialize(clip);
            File.WriteAllText(Path.Combine(TrackMetaDirectory, hash), jsonString);
        }

        public bool GetMeta(ref Clip clip)
        {
            string hash = GetClipHash(clip);
            string source = clip.Source;

            try
            {
                string jsonString = File.ReadAllText(Path.Combine(TrackMetaDirectory, hash));
                clip = JsonSerializer.Deserialize<Clip>(jsonString);
                clip.Source = source;

                return true;
            }
            catch(Exception e)
            {
                Debug.Write(e);
                return false;
            }
        }
    }
}
