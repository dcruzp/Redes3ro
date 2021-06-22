using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Component
{
    public class OneBitPackage
    {
        uint _time;
        Action _action;
        Bit _bit;
        ActionResult _actionResult;

        public OneBitPackage (
            uint time, 
            Action action, 
            Bit bit , 
            ActionResult actionResult = ActionResult.None )
        {
            _time = time;
            _action = action;
            _bit = bit;
            _actionResult = actionResult; 
        }

        public Bit Bit
        {
            get => this._bit;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(this._time.ToString());
            stringBuilder.Append(" "); 
            stringBuilder.Append(this._action.ToString());
            stringBuilder.Append(" ");
            stringBuilder.Append((int)this._bit);
            stringBuilder.Append(" ");
            stringBuilder.Append(this._actionResult == ActionResult.None ? "" : this._actionResult.ToString());
            return stringBuilder.ToString();
        }
    }
}
