using ProyecotdeRedes.Devices;

namespace ProyecotdeRedes.Component
{
  public class CableDuplex : ICable
  {
    Port _puerto1;
    Port _puerto2;

    Bit _bit1;
    Bit _bit2;

    public CableDuplex()
    {
      _puerto1 = null;
      _puerto2 = null;
      _bit1 = Bit.none;
      _bit2 = Bit.none;
    }

    public Port puerto1 { get => _puerto1; set => _puerto1 = value; }
    public Port puerto2 { get => _puerto2; set => _puerto2 = value; }
    public Bit bit1 { get => _bit1; set => _bit1 = value; }
    public Bit bit2 { get => _bit2; set => _bit2 = value; }
  }
}
