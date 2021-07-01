using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes.Auxiliaries
{
  public class AuxiliaryFunctions
  {
    public static int FromByteDataToInt(List<Bit> data)
    {
      StringBuilder stringBuilder = new StringBuilder(data.Count);

      foreach (var item in data)
      {
        stringBuilder.Append(((int)item).ToString());
      }

      int lenghtData = Convert.ToInt32(stringBuilder.ToString(), 2);

      return lenghtData;
    }

    public static string FromByteDataToHexadecimal(List<Bit> data)
    {
      Queue<Bit> queue = new Queue<Bit>(data);

      StringBuilder stringBuilder = new StringBuilder(data.Count / 4);

      StringBuilder stringBuilderAux = new StringBuilder(4);

      while (queue.Count > 0)
      {
        stringBuilderAux = new StringBuilder(4);

        int count = 4;

        while (count-- > 0 && queue.Count > 0)
        {
          Bit bit = queue.Dequeue();
          stringBuilderAux.Append(((int)bit).ToString());
        }

        if (stringBuilderAux.Length != 4)
          throw new InvalidCastException($"No se puede transformar de " +
$"Binario a Hexadecimal porque no hay una 4 bits para determinar un digito en Hexadecimal");

        stringBuilder.Append(FromBinaryCadenaToHexadecimalDigit(stringBuilderAux
            .ToString()));
      }

      return stringBuilder.ToString();
    }

    public static char FromBinaryCadenaToHexadecimalDigit(string str)
    {
      switch (str)
      {
        case "0000":
          return '0';
        case "0001":
          return '1';
        case "0010":
          return '2';
        case "0011":
          return '3';
        case "0100":
          return '4';
        case "0101":
          return '5';
        case "0110":
          return '6';
        case "0111":
          return '7';
        case "1000":
          return '8';
        case "1001":
          return '9';
        case "1010":
          return 'A';
        case "1011":
          return 'B';
        case "1100":
          return 'C';
        case "1101":
          return 'D';
        case "1110":
          return 'E';
        case "1111":
          return 'F';
        default:
          throw new InvalidCastException("Can't  convert");
      }

    }

    public static Bit[] convertFromHexStrToBitArray(string hexString)
    {
      List<Bit> bitArray = new List<Bit>();

      foreach (var item in hexString)
      {
        string strtobit = convertHexDigitToBitSequence(item);

        if (string.IsNullOrEmpty(strtobit))
          throw new InvalidCastException("can't convert");

        foreach (var item1 in strtobit)
        {
          bitArray.Add(item1 == '0' ? Bit.cero : Bit.uno);
        }
      }

      return bitArray.ToArray();
    }

    private static string convertHexDigitToBitSequence(char c)
    {
      switch (c)
      {
        case '0':
          return "0000";
        case '1':
          return "0001";
        case '2':
          return "0010";
        case '3':
          return "0011";
        case '4':
          return "0100";
        case '5':
          return "0101";
        case '6':
          return "0110";
        case '7':
          return "0111";
        case '8':
          return "1000";
        case '9':
          return "1001";
        case 'A':
          return "1010";
        case 'B':
          return "1011";
        case 'C':
          return "1100";
        case 'D':
          return "1101";
        case 'E':
          return "1110";
        case 'F':
          return "1111";

        default:
          throw new InvalidCastException("Can't convert");

      }

    }

    public static InstructionType GiveMeTheInstruction(string instruction)
    {
      switch (instruction)
      {
        case "create":
          return InstructionType.create;
        case "connect":
          return InstructionType.connect;
        case "send":
          return InstructionType.send;
        case "disconnect":
          return InstructionType.disconnect;
        case "mac":
          return InstructionType.mac;
        case "send_frame":
          return InstructionType.send_frame;
        case "ip":
          return InstructionType.ip;
        case "send_packet":
          return InstructionType.send_packet;
        default:
          throw new InvalidCastException($" '{instruction}' no ese un tipo de instrucción valida");
      }
    }

    /// <summary>
    /// Esta funcion retorna una representacion en binarios 
    /// de los datos que se pasan con parametro en un array de
    /// Bits 
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    public static string ConvertToStringPackage(List<Bit> package)
    {
      StringBuilder stringBuilder = new StringBuilder(package.Count);

      foreach (var item in package)
      {
        stringBuilder.Append(((int)item).ToString());
      }

      return stringBuilder.ToString();
    }

    public static int ConvertFronHexadecimalToInteger(string hexadecimal)
    {
      int number = Convert.ToInt32(hexadecimal, 16);
      return number;
    }

    public static string GiveMeLenghtInHexadecimal(string data)
    {
      StringBuilder stringBuilder = new StringBuilder((data.Length / 2).ToString("X"));

      stringBuilder.Insert(0, "0", stringBuilder.Length % 2);

      return stringBuilder.ToString();
    }

    public static IEnumerable<String> SplitStrInSubStrWithLength(string hexadecimal)
    {
      if (hexadecimal.Length % 2 != 0)
        throw new IndexOutOfRangeException($"To Split de str Hex in number Hex the length of " +
          $"{hexadecimal} must be divisible by 2"); 

      StringBuilder stringBuilder = new StringBuilder(2);
      for (int i = 0; i < hexadecimal.Length; i += 2)
      {
        stringBuilder.Clear();
        stringBuilder.Append(hexadecimal.Substring(i,2));

        yield return stringBuilder.ToString();
      }
    }

    public static uint SumOfDataInInteger ( string data )
    {
      uint sumdata = 0;
      foreach (var item in SplitStrInSubStrWithLength(data))
      {
        var integer = Convert.ToUInt32(item, 16);

        sumdata += integer;
      }

      return sumdata; 
    }

    public static string SumOfDataInHex(string data)
    {
      uint sumdata = 0;
      foreach (var item in SplitStrInSubStrWithLength(data))
      {
        var integer = Convert.ToUInt32(item, 16);

        sumdata += integer;
      }

      var sumdatahexadecimal = new StringBuilder(sumdata.ToString("X"));

      sumdatahexadecimal.Insert(0, "0", sumdatahexadecimal.Length % 2);

      return sumdatahexadecimal.ToString();
    }

    /// <summary>
    /// Esta funcion recibe una string que representa los datos que 
    /// se van a transformar en Hexadecimal. Lo que hace basicamente es
    /// coger cada uno de los caracteres y obtener su representacion en 
    /// hexadecimal y con esta informacion conformar el string de salida 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string FromCharDataToHexadecimalData (string data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      
      foreach (var item in data)
      {
        var aux = new StringBuilder(Convert.ToString((int)item, 16));
        aux.Insert(0, "0", aux.Length % 2);
        stringBuilder.Append(aux);
      }
      return stringBuilder.ToString(); 
    }


    /// <summary>
    /// Esto retorna una lista de Bits que tiene la secuencia de 
    /// bits que representan los datos que se brindan en hexadecimal 
    /// por el parámetro datainHex. (todo concatenado) 
    /// </summary>
    /// <param name="datainHex"></param>
    /// <returns></returns>
    public static List<Bit> ConvertToListOfBitHexadecimalSequence(params string[] datainHex)
    {
      var package = new List<Bit>();

      foreach (var item in datainHex)
      {
        package.AddRange(convertFromHexStrToBitArray(item));
      }

      return package;
    }
  }
}
