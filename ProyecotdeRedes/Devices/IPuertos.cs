namespace ProyecotdeRedes.Devices
{
  public interface IPuertos
  {
    public void UpdateOutBit();
    public void UpdateInBit();
    public Bit GiveMeOutBit();
    public Bit GiveMeInBit();
  }
}
