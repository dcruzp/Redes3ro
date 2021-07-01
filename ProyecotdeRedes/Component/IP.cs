using System;
using System.Text;
using System.Globalization;
using ProyecotdeRedes.Auxiliaries;
using System.Collections.Generic;

namespace ProyecotdeRedes.Component
{
  public class IP :IComparable
  {
    byte[] _ip;

    public IP(byte[] ip)
    {
      if (ip.Length != 4)
        throw new Exception("wrong ip direction"); 
      this._ip = ip; 
    }

    public IP (string ip , NumberStyles numberStyles)
    {
      List<byte> ip_dir = new List<byte>(4); 
      if (numberStyles == NumberStyles.None)
      {
        foreach (var item in ip.Split('.'))
        {
          var @byte = System.Byte.Parse(item, NumberStyles.None);
          ip_dir.Add(@byte);
        }
      }
      if (numberStyles == NumberStyles.HexNumber)
      {
        foreach (var item in AuxiliaryFunctions.SplitStrInSubStrWithLength(ip))
        {
          var @byte = System.Byte.Parse(item, NumberStyles.HexNumber);
          ip_dir.Add(@byte); 
        }
      }



      if (ip_dir.Count != 4)
      {
        throw new InvalidCastException($"can't cast de {ip} address . This must have four bytes exactly"); 
      }
      _ip = ip_dir.ToArray(); 
    }

    public int  this  [int i]
    {
      get {
        if (i < 0 || i >= 4)
          throw new IndexOutOfRangeException("a Ip direction has only 4 bytes of data ");
        return this._ip[i]; 
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

      for (int i = 0; i < this._ip.Length; i++)
      {
        if (_ip[i] != ip_dir[i])
          return false;
      }
      return true; 
    }

    public string GiveMeStringFormat (string format = null)
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _ip)
      {
        if (stringBuilder.Length != 0 && format == null)
          stringBuilder.Append('.');
        stringBuilder.Append(item.ToString(format));
      }
      return stringBuilder.ToString();
    }

    public string GiveMeStringFormat (int @base )
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _ip)
      {
        if (stringBuilder.Length != 0 && @base == 10)
          stringBuilder.Append('.');
        stringBuilder.Append(Convert.ToString(item, @base).ToUpper());
      }
      return stringBuilder.ToString();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      foreach (var item in _ip)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append('.');
        stringBuilder.Append(item.ToString()); 
      }
      return stringBuilder.ToString();
    }
  }
}
