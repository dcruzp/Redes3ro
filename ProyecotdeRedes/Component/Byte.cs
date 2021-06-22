﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Component
{
    public class Byte
    {
        Bit [] bits { get; set; }

        public Byte (Bit [] bits)
        {
            if(bits.Length != 8 )
            {
                throw new Exception("Un byte tiene que tener exactamente 8 bits"); 
            }
            this.bits = bits;
        }

        public Bit [] GiveMeBits
        {
            get => this.bits;
        }
    }
}
