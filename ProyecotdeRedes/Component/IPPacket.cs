using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyecotdeRedes.Component
{
  public class IPPacket
  {
    List<byte> data;

    public Tuple<int, int> index_lenght_dest_ip { get; }
    public Tuple<int, int> index_lenght_orig_ip { get; }
    public Tuple<int, int> index_lenght_ttl { get; }
    public Tuple<int, int> index_lenght_protocol { get; }
    public Tuple<int, int> index_lenght_payload_size {get; }
    public Tuple<int, int> index_lenght_data { get; set; }

    public IPPacket()
    {
      index_lenght_dest_ip = new Tuple<int, int>(0, 4);
      index_lenght_orig_ip = new Tuple<int, int>(4, 4);
      index_lenght_ttl = new Tuple<int, int>(8, 1);
      index_lenght_protocol = new Tuple<int, int>(9, 1);
      index_lenght_payload_size = new Tuple<int, int>(10, 1);
      index_lenght_data = new Tuple<int, int>(11, 255);

      this.data = new List<byte>(); 
    }

    public IPPacket(List<byte> bytes):base()
    {
      data.AddRange(bytes);
    }

    public bool CheckDataIsOk()
    {
      if (data.Count < 11)
        return false;

      byte length_data = data.GetRange(index_lenght_payload_size.Item1,
                                       index_lenght_payload_size.Item2).FirstOrDefault();

      var _data = data.GetRange(index_lenght_data.Item1, index_lenght_data.Item2);

      if (length_data > _data.Count)
        return false;

      return true; 
    }

    public bool InsertNewByte (byte @byte)
    {
      if (data.Count == 10)
      {
        index_lenght_data  = new Tuple<int, int>(10,@byte); 
      }
      else if (data.Count > 11)
      {
        if (data.Count > 11 + index_lenght_data.Item2)
          return false;
      }

      data.Add(@byte);

      return true; 
    }
  }
}
