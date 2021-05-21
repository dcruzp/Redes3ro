using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Auxiliaries
{
    public class CheckMetods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirMac"></param>
        /// <returns></returns>
        public static Boolean CheckIsOkDirMac (string dirMac)
        {
            return dirMac.Length == 4 && CheckStrContainOnlyHexadecimalCharacters(dirMac); 
        }
        public static bool CheckStrContainOnlyHexadecimalCharacters(string str)
        {
            char[] caracteresvalidos = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            foreach (var item in caracteresvalidos)
                if (caracteresvalidos.Contains(item)) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cad"></param>
        /// <returns></returns>
        public static bool esBinariaLaCadena(string cad)
        {
            foreach (var item in cad)
            {
                if (item != '0' && item != '1')
                {
                    return false;
                }
            }
            return true;
        }


        public static bool sonvalidoslacantidaddepuertosdeunhub(int a, int b)
        {
            if (a < 4)
            {
                throw new InvalidCastException("Un hub no puede tener menos de 4 puertos ");
            }
            if (b <= a)
            {
                throw new InvalidCastException("La cantidad máxima de puertos no puede ser menor o igual que la cantidad mínima de puertos");
            }
            return true;
        }

    }
}
