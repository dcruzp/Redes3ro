using ProyecotdeRedes.Auxiliaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyecotdeRedes.Component
{
  public class DataFramePackage
  {
    List<Byte> _dirMacIn { get; set; }
    List<Byte> _dirMacOut { get; set; }
    List<Byte> _dataLength { get; set; }
    List<Byte> _lengthCheckData { get; set; }
    List<Byte> _data { get; set; }
    List<Byte> _checkData { get; set; }

    int _currentCount = 0;
    int _length = 0;
    int _lengthCheck = 0;
    bool _fullData = false;
    uint _timeReceived;
    bool _IsOkData; 

    public DataFramePackage()
    {
      _dirMacIn = new List<Byte>();
      _dirMacOut = new List<Byte>();
      _dataLength = new List<Byte>();
      _lengthCheckData = new List<Byte>();
      _data = new List<Byte>();
      _checkData = new List<Byte>();
      _currentCount = 0;
      _length = 0;
      _timeReceived = 0;
      _fullData = false;
      _lengthCheck = 0;
    }

  

    public uint TimeReceived
    {
      get => _timeReceived;
    }

    public bool FullData { get => _fullData; }

    public void InsertNextByte(Byte @byte)
    {
      if (_currentCount < 2)
      {
        _dirMacIn.Add(@byte);
      }
      else if (_currentCount < 4)
      {
        _dirMacOut.Add(@byte);
      }
      else if (_currentCount < 5)
      {
        _dataLength.Add(@byte);
        _length = Auxiliaries.AuxiliaryFunctions.FromByteDataToInt(@byte.GiveMeBits.ToList());
      }
      else if (_currentCount < 6)
      {
        _lengthCheckData.Add(@byte);
        _lengthCheck = AuxiliaryFunctions.FromByteDataToInt(@byte.GiveMeBits.ToList());
      }
      else if (_currentCount < _length + 6)
      {
        _data.Add(@byte);
      }
      else
      {
        _checkData.Add(@byte);
      }

      _currentCount++;
      if (_currentCount == _length + _lengthCheck + 6)
      {
        _timeReceived = Program.current_time;
        _fullData = true;
        _IsOkData = CheckIsOkData(); 
      }
    }

    public List<Bit> CheckData
    {
      get
      {
        if (!_fullData)
        {
          return null; 
        }

        List<Bit> bits = new List<Bit>();

        foreach (var item in _checkData)
        {
          bits.AddRange(item.GiveMeBits); 
        }

        return bits; 
      }
    }

    public List<Bit> Data
    {
      get
      {
        if (!_fullData)
          return null;

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
        if (!_fullData)
          return null;

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
        if (!_fullData)
          return null;

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

      foreach (var item in _lengthCheckData)
      {
        frame.AddRange(item.GiveMeBits);
      }

      foreach (var item in _data)
      {
        frame.AddRange(item.GiveMeBits);
      }
      foreach (var item in _checkData)
      {
        frame.AddRange(item.GiveMeBits);
      }

      return frame;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append(_timeReceived.ToString());
      stringBuilder.Append(" ");
      stringBuilder.Append(AuxiliaryFunctions.FromByteDataToHexadecimal(MacOut));
      stringBuilder.Append(" ");
      stringBuilder.Append(AuxiliaryFunctions.FromByteDataToHexadecimal(Data));

      return stringBuilder.ToString();
    }

    

    public bool CheckIsOkData ()
    {
      var datatoCheck = AuxiliaryFunctions.ConvertToStringPackage(CheckData);

      var datacheckInteger = Convert.ToUInt32(datatoCheck, 2);

      var sumdata = AuxiliaryFunctions.SumOfDataInInteger(AuxiliaryFunctions.FromByteDataToHexadecimal(Data));

      if (datacheckInteger != sumdata)
        return false; 

      return true; 
    }
  }
}
