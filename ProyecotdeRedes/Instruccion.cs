using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
     
    public class Instruccion
    {
        /// <summary>
        /// El tiempo en el que le toca ejecutarse la instrucion 
        /// </summary>
        uint time; 

        /// <summary>
        /// Esta es el string completo donde esta toda la informacion que
        /// tiene la instruccion 
        /// </summary>
        string _instruccion;


        /// <summary>
        /// Este es el constructor
        /// aquí pone automáticamente en la variable time 
        /// el valor del entero que representa el tiempo en el 
        /// que le corresponde ejecutarse la instrucción 
        /// </summary>
        /// <param name="instruccion"></param>
        public Instruccion (string instruccion )
        {
            this._instruccion = instruccion;
            uint.TryParse(instruccion.Split(' ')[0] , out time); 
        }

        public String instruccion
        {
            get => this._instruccion; 
        }

        public uint Time
        {
            get => this.time;
        }
    }
}
