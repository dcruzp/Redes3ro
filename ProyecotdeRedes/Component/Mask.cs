using ProyecotdeRedes.Auxiliaries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProyecotdeRedes.Component
{
  public class Mask:IComparable
  {
    byte[] _mask;

    public Mask(byte[] ip)
    {
      if (ip.Length != 4)
        throw new Exception("wrong ip direction");
      this._mask = ip;
    }

    public Mask(string mask, NumberStyles numberStyles)    {
      List<byte> mask_dir = new List<byte>(4);
      if (numberStyles == NumberStyles.None)
      {
        foreach (var item in mask.Split('.'))
        {
          var @byte = System.Byte.Parse(item, NumberStyles.None);
          mask_dir.Add(@byte);
        }
      }
      else if (numberStyles == NumberStyles.HexNumber)
      {
        foreach (var item in AuxiliaryFunctions.SplitStrInSubStrWithLength(mask))
        {
          var @byte = System.Byte.Parse(item, NumberStyles.HexNumber);
          mask_dir.Add(@byte);
        }
      }


      if (mask_dir.Count != 4)
      {
        throw new InvalidCastException($"can't cast {mask} address . This must have four bytes exactly");
      }
      _mask = mask_dir.ToArray();
    }

    public int this[int i]
    {
      get
      {
        if (i < 0 || i >= 4)
          throw new IndexOutOfRangeException("a Ip direction has only 4 bytes of data ");
        return this._mask[i];
      }
    }

    public int CompareTo(object obj)
    {
      IP ip = obj as IP;

      if (ip == null)
        return -1;

      var ip1 = this.GiveMeStringFormat("X2");
      var ip2 = ip.GiveMeStringFormat("X2");

      return ip1.CompareTo(ip2);
    }

    public override bool Equals(object obj)
    {
      IP ip_dir = obj as IP;
      if (ip_dir is null)
      {
        return false;
      }

      for (int i = 0; i < this._mask.Length; i++)
      {
        if (_mask[i] != ip_dir[i])
          return false;
      }
      return true;
    }

    public string GiveMeStringFormat(string format = null)
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _mask)
      {
        if (stringBuilder.Length != 0 && format == null)
          stringBuilder.Append('.');
        stringBuilder.Append(item.ToString(format));
      }
      return stringBuilder.ToString();
    }

    public string GiveMeStringFormat(int @base)
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _mask)
      {
        if (stringBuilder.Length != 0 && @base == 10)
          stringBuilder.Append('.');
        stringBuilder.Append(Convert.ToString(item, @base));
      }
      return stringBuilder.ToString();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _mask)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append('.');
        stringBuilder.Append(item.ToString());
      }
      return stringBuilder.ToString();
    }
  }
}
