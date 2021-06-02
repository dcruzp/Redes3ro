﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {
        /// <summary>
        /// Este es el constructor por defecto 
        /// siempre asume que el Hub va a tener 4 puertos
        /// </summary>
        /// <param name="name"></param>
        /// <param name="indice"></param>
        /// El parámetro indice es para saber que indice tiene el 
        /// dispositivo en el montículo donde se almacenan los
        /// dispositivos en general. 
        public Hub(string name ,int indice):base (name , 4,indice)
        {
        }


        /// <summary>
        /// Este es el constructor que chequea que la cantidad de
        /// puertos con que se va a crear el hub tenga una cantidad 
        /// de puertos que este en el rango de la configuración de la 
        /// cantidad de puertos establecidas
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numerodepuertos"></param>
        /// <param name="indice"></param>
        public Hub (string name , int numerodepuertos , int indice):base(name , numerodepuertos,indice)
        {
            if (numerodepuertos < Program.cantidadminimadepuertosdeunhub || numerodepuertos > Program.cantidadmaximadepuertosdeunhub)
            {
                throw new IndexOutOfRangeException("No se puede tener un hub con menos de 4 puertos o mas de 8 "); 
            }            
        }
    }
}