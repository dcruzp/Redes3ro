using System.Text;

namespace ProyecotdeRedes.Component
{
  public class OneBitPackage
  {
    uint _time;
    Action _action;
    Bit _bit;
    ActionResult _actionResult;
    string _port;

    public OneBitPackage(
        uint time,
        Action action,
        Bit bit,
        ActionResult actionResult = ActionResult.None,
        string port = "")
    {
      _time = time;
      _action = action;
      _bit = bit;
      _actionResult = actionResult;
      _port = port;
    }

    public uint Time
    {
      get => _time;
    }

    public Bit Bit
    {
      get => _bit;
    }
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      stringBuilder.Append(_port);
      stringBuilder.Append(" ");
      stringBuilder.Append(_time.ToString());
      stringBuilder.Append(" ");
      stringBuilder.Append(_action.ToString());
      stringBuilder.Append(" ");
      stringBuilder.Append((int)_bit);
      stringBuilder.Append(" ");
      stringBuilder.Append(_actionResult == ActionResult.None ? "" : _actionResult.ToString());
      return stringBuilder.ToString();
    }
  }
}
