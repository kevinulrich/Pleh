using System;
using System.Collections.Generic;
using System.Text;

namespace Pleh.Models
{
    public class Playlist
    {
        public List<Clip> List;

        public Playlist()
        {
            List = new List<Clip>();
        }
    }
}
