using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Component
{
    public class DataFramePackage
    {
        List<Byte> DirMacIn { get; set; }
        List<Byte> DirMacOut { get; set; }
        List<Byte> DataLength { get; set; }
        List<Byte> Extra { get; set; }
        List<Byte> Data { get; set; }

        public DataFramePackage()
        {
            DirMacIn = new List<Byte>();
            DirMacOut = new List<Byte>();
            DataLength = new List<Byte>();
            Extra = new List<Byte>();
            Data = new List<Byte>();
        }

        int _currentCount = 0;
        int lenght = 0;

        bool _fullData = false; 

        public bool FullData { get => this._fullData; }

        public void InsertNextByte (Byte @byte)
        {
            if (_currentCount < 2)
            {
                DirMacIn.Add(@byte);
            }
            else if(_currentCount <4)
            {
                DirMacOut.Add(@byte); 
            }
            else if(_currentCount < 5)
            {
                DataLength.Add(@byte);
                lenght = Auxiliaries.AuxiliaryFunctions.FromByteDataToInt(@byte.GiveMeBits.ToList());
            }
            else if(_currentCount < 6 )
            {
                Extra.Add(@byte);
            }
            else
            {
                
                Data.Add(@byte);
            }
            _currentCount++;
            if (_currentCount == lenght + 6)
            {
                _fullData = true;
            }
        }
    }
}
