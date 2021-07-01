using System;
using System.Collections.Generic;

namespace ProyecotdeRedes.Component
{
  public class IPPacket
  {
    List<byte> data;

    Tuple<int, int> index_lenght_dest_ip;
    Tuple<int, int> index_lenght_orig_ip;
    Tuple<int, int> index_lenght_ttl;
    Tuple<int, int> index_lenght_protocol;
    Tuple<int, int> index_lenght_payload_size;
    Tuple<int, int> index_lenght_data;

    public IPPacket()
    {
      this.data = new List<byte>(); 
    }

    public IPPacket(List<byte> bytes)
    {
      data = new List<byte>();
      data.AddRange(bytes);
    }

    public void InsertNewByte (byte @byte)
    {
      data.Add(@byte);
    }
  }
}
