using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Pleh.Models
{
    public class Clip
    {
        public string Source;
        public string Title { get; set; }
        public double FadeOutStart = 245;
        public double FadeOutLength = 2;
        public double FadeInStart = 0.5;
        public double FadeInLength = 0.1;

        public Clip(string source)
        {
            Title = Path.GetFileName(source);
            Source = source;
        }
    }
}
