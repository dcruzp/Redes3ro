using ProyecotdeRedes.Devices;

namespace ProyecotdeRedes.Component
{
  public interface ICable
  {
    public Port puerto1 { get; set; }
    public Port puerto2 { get; set; }


    public Bit bit1 { get; set; }

    public Bit bit2 { get; set; }

  }
}
