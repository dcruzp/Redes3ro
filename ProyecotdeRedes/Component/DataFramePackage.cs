using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyecotdeRedes.Auxiliaries;

namespace ProyecotdeRedes.Component
{
    public class DataFramePackage
    {
        List<Byte> _dirMacIn { get; set; }
        List<Byte> _dirMacOut { get; set; }
        List<Byte> _dataLength { get; set; }
        List<Byte> _extra { get; set; }
        List<Byte> _data { get; set; }

        uint _timeReceived; 

        public DataFramePackage()
        {
            _dirMacIn = new List<Byte>();
            _dirMacOut = new List<Byte>();
            _dataLength = new List<Byte>();
            _extra = new List<Byte>();
            _data = new List<Byte>();
            _currentCount = 0;
            _length = 0;
            _timeReceived = 0;
            _fullData = false; 
        }

        int _currentCount = 0;
        int _length = 0;

        bool _fullData = false; 

        public uint TimeReceived 
        {
            get => _timeReceived; 
        }

        public bool FullData { get => this._fullData; }

        public void InsertNextByte (Byte @byte)
        {
            if (_currentCount < 2)
            {
                _dirMacIn.Add(@byte);
            }
            else if(_currentCount <4)
            {
                _dirMacOut.Add(@byte); 
            }
            else if(_currentCount < 5)
            {
                _dataLength.Add(@byte);
                _length = Auxiliaries.AuxiliaryFunctions.FromByteDataToInt(@byte.GiveMeBits.ToList());
            }
            else if(_currentCount < 6 )
            {
                _extra.Add(@byte);
            }
            else
            {                
                _data.Add(@byte);
            }
            _currentCount++;
            if (_currentCount == _length + 6)
            {
                this._timeReceived = Program.current_time;
                this._fullData = true;
            }
        }

        public List<Bit> Data { 
            get
            {
                if (!_fullData) return null;

                List<Bit> bits = new List<Bit>();

                foreach (var item in _data.Select(x => x.GiveMeBits))
                {
                    bits.AddRange(item);
                }
                return bits;
            }
        }

        public List<Bit> MacIn
        {
            get
            {
                if (!_fullData) return null;

                List<Bit> bits = new List<Bit>();

                foreach (var item in _dirMacIn.Select(x => x.GiveMeBits))
                {
                    bits.AddRange(item);
                }
                return bits;
            }
        }

        public List<Bit> MacOut
        {
            get
            {
                if (!_fullData) return null;

                List<Bit> bits = new List<Bit>();

                foreach (var item in _dirMacOut.Select(x => x.GiveMeBits))
                {
                    bits.AddRange(item); 
                }
                return bits; 
            }
            
        }

        public List<Bit> GetAllDataFrame()
        {
            List<Bit> frame = new List<Bit>(); 

            foreach (var item in _dirMacIn)
            {
                frame.AddRange(item.GiveMeBits); 
            }

            foreach (var item in _dirMacOut)
            {
                frame.AddRange(item.GiveMeBits);
            }

            foreach (var item in _dataLength)
            {
                frame.AddRange(item.GiveMeBits);
            }

            foreach (var item in _extra)
            {
                frame.AddRange(item.GiveMeBits);
            }

            foreach (var item in _data)
            {
                frame.AddRange(item.GiveMeBits);
            }

            return frame; 
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(this._timeReceived.ToString());
            stringBuilder.Append(" "); 
            stringBuilder.Append(AuxiliaryFunctions.FromByteDataToHexadecimal(this.MacOut));
            stringBuilder.Append(" "); 
            stringBuilder.Append(AuxiliaryFunctions.FromByteDataToHexadecimal(this.Data));

            return stringBuilder.ToString(); 
        }
    }
}
