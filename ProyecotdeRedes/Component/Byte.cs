using System;
using System.Text;

namespace ProyecotdeRedes.Component
{
  public class Byte
  {
    Bit[] bits { get; set; }

    public Byte(Bit[] bits)
    {
      if (bits.Length != 8)
      {
        throw new Exception("Un byte tiene que tener exactamente 8 bits");
      }
      this.bits = bits;
    }

    public Bit[] GiveMeBits
    {
      get => bits;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(bits.Length);

      foreach (var item in bits)
      {
        stringBuilder.Append(((int)item).ToString()); 
      }

      StringBuilder builder = new StringBuilder();

      builder.Append(Convert.ToString(Convert.ToInt32(stringBuilder.ToString(), 2), 16));

      return builder.ToString(); 
    }
  }
}
