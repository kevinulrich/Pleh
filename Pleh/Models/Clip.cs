using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json.Serialization;

namespace Pleh.Models
{
    public enum ClipType
    {
        Music,
        Voicetrack
    }
    public class Clip
    {
        [JsonIgnore]
        public string Source { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public double FadeOutStart { get; set; }
        public double FadeOutLength { get; set; }
        public double FadeInStart { get; set; }
        public double FadeInLength { get; set; }
        public double RampIn { get; set; }
        public double RampOut { get; set; }
        public ClipType Type { get; set; }

        public Clip(string source)
        {
            Source = source;
        }

        public Clip()
        {
            
        }
    }
}
