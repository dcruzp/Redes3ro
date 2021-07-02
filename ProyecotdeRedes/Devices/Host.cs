﻿using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
  class Host : Device
  {
    /// <summary>
    /// Esto es para representar la direccion ip y la 
    /// mascara del dispositivo en formato "entendible para los humanos"
    /// </summary>
    Tuple<IP, Mask> ip_mask;

    Dictionary<string, List<String>> packages_to_send; 

    /// <summary>
    /// Esta es la dirección Max representada en hexadecimal 
    /// </summary>
    string mac_direction;

    public Host(string name, int indice) : base(name, 1, indice)
    {
      mac_direction = null;
      packages_to_send = new Dictionary<string,List<string>>(); 
    }


    public void TakeIpAndMaskAddres (string ip , string mask)
    {
      IP _ip = new IP(ip, System.Globalization.NumberStyles.None);

      Mask _mask = new Mask(mask, System.Globalization.NumberStyles.None);

      this.ip_mask = new Tuple<IP, Mask>(_ip, _mask); 

    }

    /// <summary>
    /// Esto es para darle una dirección Mac a la computadora 
    /// El parámetro es un string hexadecimal de 4 dígitos
    /// </summary>
    /// <param name="dirMac"></param>
    public void PutMacDirection(string dirMac)
    {
      if (mac_direction == null && CheckMetods.CheckIsOkDirMac(dirMac))
      {
        mac_direction = dirMac;
      }
      else
      {
        throw new InvalidCastException($"No se pudo asignar la dirección Mac {dirMac} a la computadora {name}");
      }
    }


    /// <summary>
    /// send_frame procesa todos los datos necesarios 
    /// para armar el contenido de un frame yenviarlos 
    /// por el puerto.  
    /// </summary>
    /// <param name="mac">Este parametro es la data de la 
    /// direccion mac en formato hexadecimal. Es la direccion 
    /// mac de la computadora a la que se le va a enviar la data</param>
    /// <param name="data">Este parametro es la data que se va a 
    /// enviar en el frame, en formato hexadecimal </param>
    public void send_frame(string mac, string data)
    {

      var dirmacin = string.IsNullOrEmpty(mac) ? "FFFF" : mac;

      var dirmacout = string.IsNullOrEmpty(mac_direction) ? "FFFF" : mac_direction;

      var lenghtdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(data);


      var sumdatahex = AuxiliaryFunctions.SumOfDataInHex(data);

      var lenghtsumdatahex = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(sumdatahex);

      var packagetosend = AuxiliaryFunctions.ConvertToListOfBitHexadecimalSequence(dirmacin, 
                                                                                   dirmacout, 
                                                                                   lenghtdata, 
                                                                                   lenghtsumdatahex, 
                                                                                   data, 
                                                                                   sumdatahex);


      foreach (var item in ports)
      {
        item.SendData(packagetosend);
      }

      Console.WriteLine($"Enviado por la computadora '{name}' el " +
          $"paquete {AuxiliaryFunctions.FromByteDataToHexadecimal(packagetosend)}");

    }

    public void send_packet (string ip , string data )
    {
      //direction ip to send the data 
      IP _ip = new IP(ip, System.Globalization.NumberStyles.None);

      var query = createSpecialFrameQuery(_ip.GiveMeStringFormat("X2"));

      if (!packages_to_send.ContainsKey(ip))
      {
        packages_to_send.Add(ip, new List<string>()); 
      }

      var ip_packet = BuidPacketToSend(ip, data); 

      packages_to_send[ip].Add(ip_packet); 
      
      foreach (var item in this.ports)
      {
        item.SendData(query); 
      }
    }

    public string BuidPacketToSend(string ip, string data)
    {
      string destination_ip = new IP(ip, System.Globalization.NumberStyles.None).GiveMeStringFormat("X2");
      string origin_ip = this.ip_mask.Item1.GiveMeStringFormat("X2");

      string tlt = "00";
      string protocol = "00";

      string payload_size = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(data);

      if (payload_size.Length != 2)
        throw new Exception($"the lenght of data '{data}' out of range of byte ");

      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append(destination_ip);
      stringBuilder.Append(origin_ip);
      stringBuilder.Append(tlt);
      stringBuilder.Append(protocol);
      stringBuilder.Append(payload_size);
      stringBuilder.Append(data);

      return stringBuilder.ToString(); 

    }

    /// <summary>
    /// Este método se llama cuando hubo una instrucción 
    /// send para el envío de un paquete de bits
    /// </summary>
    /// <param name="paquete"></param>
    public void send(List<Bit> pakage)
    {
      foreach (var item in ports)
      {
        item.SendData(pakage);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ProcessDataReceived()
    {
      base.ProcessDataReceived();

      if (currentBuildInFrame.FullData)
      {
        
        var is_ok_data = currentBuildInFrame.CheckIsOkData(); 
        
        var is_request_query = CheckIsFrameReceivedIsSprecialFrameQuery(currentBuildInFrame);
        var is_request_response = CheckIsFrameReceivedIsSpecialFrameResponse(currentBuildInFrame);

        if (is_ok_data && ! is_request_query && ! is_request_response)
        {
          var data_ip_packet = ReadIPPacket(currentBuildInFrame);
          Console.WriteLine(data_ip_packet);

          WriteIpPacketReceivedInOutput(currentBuildInFrame); 
        }


        _history.Add(currentBuildInFrame);
        WriteDataReceivedInOutput(currentBuildInFrame);

        currentBuildInFrame = null;
      }
    }

    public string ReadIPPacket (DataFramePackage frame)
    {
      var data = frame.Data;

      var data_hexadecimal = AuxiliaryFunctions.FromByteDataToHexadecimal(data);

      IPPacket iPPacket = new IPPacket(); 

      foreach (var item in AuxiliaryFunctions.SplitStrInSubStrWithLength(data_hexadecimal))
      {
        byte aux_byte = System.Byte.Parse(item, System.Globalization.NumberStyles.HexNumber);
        iPPacket.InsertNewByte(aux_byte); 
      }

      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append(Program.current_time);
      stringBuilder.Append(" ");
      stringBuilder.Append(iPPacket.OriginIp.ToString());
      stringBuilder.Append(" "); 

      foreach (var item in iPPacket.Data)
      {
        stringBuilder.Append(item.ToString("X2")); 
      }

      return stringBuilder.ToString(); 
    }

    private bool CheckIsFrameReceivedIsSpecialFrameResponse(DataFramePackage frame)
    {
      //geting the data  in Hexadecimal 
      var data = AuxiliaryFunctions.FromByteDataToHexadecimal(frame.Data);

      //cheking that data have the correct lenght
      if (data.Length != 16)
        return false;

      //Geting the Hexadecimal representation of "ARPR" to compare with the data
      var arpr = AuxiliaryFunctions.FromCharDataToHexadecimalData("ARPR");

      //Comparing data encripted for verific that is a request ARPR
      if (String.Compare(arpr,data.Substring(0, arpr.Length)) != 0)
        return false;

      //Geting the ip representation in hexadecimal that have the frame 
      var iphexformat = data.Substring(arpr.Length);

      //Building a instance of ip
      IP ip = new IP(iphexformat, System.Globalization.NumberStyles.HexNumber);

      //Getting the Hexadecimal representation of Mac direction that send the 
      //query for now creating a new request for send the data (in this case, the data 
      //is the mac direction of this computer) 
      var mac_in = AuxiliaryFunctions.FromByteDataToHexadecimal(frame.MacOut);

      //Getting all data that computer must be send to this direcction mac

      List<String> data_to_send = new List<string>(); 

      if (packages_to_send.ContainsKey(ip.ToString()))
      {
        data_to_send = packages_to_send[ip.ToString()]; 
      }      
      
      //Sending all data to computer whit mac 'mac_in'
      foreach (var item in data_to_send) 
      {
        this.send_frame(mac_in, item); 
      }

      //updating the dictionary 
      if (packages_to_send.ContainsKey(ip.ToString()))
      {
        packages_to_send.Remove(ip.ToString());
      }

      return true; 
    }

    private bool CheckIsFrameReceivedIsSprecialFrameQuery(DataFramePackage frame)
    {
      var data = AuxiliaryFunctions.FromByteDataToHexadecimal(frame.Data);

      if (data.Length != 16)
        return false;

      var arpq = AuxiliaryFunctions.FromCharDataToHexadecimalData("ARPQ");

      if (arpq.CompareTo(data.Substring(0,arpq.Length)) != 0)
        return false;

      var iphexformat = data.Substring(arpq.Length);

      IP ip = new IP(iphexformat, System.Globalization.NumberStyles.HexNumber); 

      if (this.ip_mask.Item1.Equals(ip))
      {
        var response = createSpecialFrameResponse(AuxiliaryFunctions.FromByteDataToHexadecimal(frame.MacOut));

        foreach (var item in ports)
        {
          item.SendData(response);
        }
      }

      return true; 
    }

    private void WriteIpPacketReceivedInOutput (DataFramePackage package)
    {

      var data_ip_packet = ReadIPPacket(currentBuildInFrame);


      EscribirEnLaSalida(data_ip_packet, this.name + "_payload.txt");
    }

    private void WriteDataReceivedInOutput(DataFramePackage package)
    {
      //uint time_received = package.TimeReceived;
      //string pcout = AuxiliaryFunctions.FromByteDataToHexadecimal(package.MacOut);
      //string data = AuxiliaryFunctions.FromByteDataToHexadecimal(package.Data);

      string dataFrame = package.ToString();

      EscribirEnLaSalida(dataFrame, name + "_data.txt");
    }

    public List<Bit> createSpecialFrameQuery (string destination_ip)
    {
      //directions 
      var destination_mac = "FFFF";
      var origin_mac = this.mac_direction;

      //data
      var first4bytesofdata = AuxiliaryFunctions.FromCharDataToHexadecimalData("ARPQ");
      var last4bytesofdata = destination_ip;
           
      var data = first4bytesofdata + last4bytesofdata;
      var lenghtdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(data);

      //verification
      var sumdata = AuxiliaryFunctions.SumOfDataInHex(data);
      var lenghtsumdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(sumdata);

      //frame 
      var frame = AuxiliaryFunctions.ConvertToListOfBitHexadecimalSequence(destination_mac,
                                                                           origin_mac,
                                                                           lenghtdata,
                                                                           lenghtsumdata,
                                                                           data,
                                                                           sumdata);
      return frame; 
    }

    public List<Bit> createSpecialFrameResponse (string destination_mac_direction)
    {
      //directions
      var destination_mac = destination_mac_direction;
      var origin_mac = this.mac_direction;

      //data 
      var first4bytesofdata = AuxiliaryFunctions.FromCharDataToHexadecimalData("ARPR");
      var last4bytesofdata = this.ip_mask.Item1.GiveMeStringFormat("X2");

      var data = first4bytesofdata + last4bytesofdata;
      var lenghtdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(data);

      //verification 
      var sumdata = AuxiliaryFunctions.SumOfDataInHex(data);
      var lenghtsumdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(sumdata);

      //frame
      var frame = AuxiliaryFunctions.ConvertToListOfBitHexadecimalSequence(destination_mac,
                                                                           origin_mac,
                                                                           lenghtdata,
                                                                           lenghtsumdata,
                                                                           data,
                                                                           sumdata);
      return frame; 
    }

  }
}
