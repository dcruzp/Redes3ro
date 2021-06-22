﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Auxiliaries
{
    public class AuxiliaryFunctions
    {
        public static int FromByteDataToInt (List<Bit> data)
        {
            StringBuilder stringBuilder = new StringBuilder(data.Count);

            foreach (var item in data)
            {
                stringBuilder.Append(((int)item).ToString());
            }

            int lenghtData = Convert.ToInt32(stringBuilder.ToString(), 2);

            return lenghtData;
        }



        public static Bit[] convertFromHexStrToBitArray(string hexString)
        {
            List<Bit> bitArray = new List<Bit>();

            foreach (var item in hexString)
            {
                string strtobit = convertHexDigitToBitSequence(item);

                if (string.IsNullOrEmpty(strtobit))
                    throw new InvalidCastException("can't convert");

                foreach (var item1 in strtobit)
                {
                    bitArray.Add(item1 == '0' ? Bit.cero : Bit.uno);
                }
            }

            return bitArray.ToArray();
        }

        private static string convertHexDigitToBitSequence(char c)
        {
            switch (c)
            {
                case '0':
                    return "0000";
                case '1':
                    return "0001";
                case '2':
                    return "0010";
                case '3':
                    return "0011";
                case '4':
                    return "0100";
                case '5':
                    return "0101";
                case '6':
                    return "0110";
                case '7':
                    return "0111";
                case '8':
                    return "1000";
                case '9':
                    return "1001";
                case 'A':
                    return "1010";
                case 'B':
                    return "1011";
                case 'C':
                    return "1100";
                case 'D':
                    return "1101";
                case 'E':
                    return "1110";
                case 'F':
                    return "1111";

                default:
                    throw new InvalidCastException("Can't convert");

            }

        }

        public static TipodeInstruccion GiveMeTheInstruction(string instruction)
        {           
            switch (instruction)
            {
                case "create":
                    return TipodeInstruccion.create;
                case "connect":
                    return TipodeInstruccion.connect;
                case "send":
                    return TipodeInstruccion.send;
                case "disconnect":
                    return TipodeInstruccion.disconnect;                    
                case "mac":
                    return TipodeInstruccion.mac;                   
                case "send_frame":
                    return TipodeInstruccion.send_frame;                    
                default:
                    throw new InvalidCastException($" '{instruction}' no ese un tipo de instrucción valida");
            }
        }

        public static string ConvertToStringPackage(List<Bit> package)
        {
            StringBuilder stringBuilder = new StringBuilder(package.Count);

            foreach (var item in package)
            {
                stringBuilder.Append(((int)item).ToString());
            }

            return stringBuilder.ToString(); 
        }


        /// <summary>
        /// Esto retorna una lista de Bits que tiene la secuencia de 
        /// bits que representan los datos que se brindan en hexadecimal 
        /// por el parámetro datainHex. (todo concatenado) 
        /// </summary>
        /// <param name="datainHex"></param>
        /// <returns></returns>
        public static List<Bit> ConvertToListOfBitHexadecimalSequence(params string[] datainHex)
        {
            var package = new List<Bit>();

            foreach (var item in datainHex)
            {
                package.AddRange(convertFromHexStrToBitArray(item));
            }

            return package;
        }
    }
}
