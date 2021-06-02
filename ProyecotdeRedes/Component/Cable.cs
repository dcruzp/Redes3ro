using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    enum Data
    { 
        cero = 0,
        uno = 1,
        nil = 2 
    }

    class Cable
    {
        public Cable(Dispositivo disp1 , Dispositivo disp2)
        {
            this.dispositivo1 = disp1;
            this.dispositivo2 = disp2; 
        }

        public Cable (): this(null , null )
        {
        }

        Dispositivo dispositivo1;
        Dispositivo dispositivo2;

        Dispositivo Disp1
        {
            get => this.dispositivo1;
            set => this.dispositivo1 = value;
        }

        Dispositivo Disp2
        {
            get => this.dispositivo2;
            set => this.dispositivo2 = value;
        }

    }
}
