using System;
using System.Collections.Generic;
using System.Text;

namespace Pleh.Models
{
    public class Player
    {
        public Clip Clip;
        public int ID { get; private set; }

        public Player(int id)
        {
            ID = id;
        }
    }
}
