using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes
{
    public class CkeckMetods
    {
        public static bool CheckStrContainOnlyHexadecimalCharacters(string str)
        {
            char[] caracteresvalidos = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            foreach (var item in caracteresvalidos)
                if (caracteresvalidos.Contains(item)) return false;
            return true;
        }
    }

    
}
