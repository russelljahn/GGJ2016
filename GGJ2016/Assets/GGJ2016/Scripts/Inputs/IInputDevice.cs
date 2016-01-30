using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sense.Inputs
{
    public interface IInputDevice
    {
        string Name { get; }

        float HorizontalAxis1 { get; }
        float VerticalAxis1 { get; }
        float HorizontalAxis2 { get; }
        float VerticalAxis2 { get; }

        bool IsPressingConfirm { get; }
        bool IsPressingCancel { get; }
        bool IsPressingMenu { get; }
        bool IsPressingMisc { get; }

        event Action PressedConfirm;
        event Action PressedCancel;
        event Action PressedMenu;
        event Action PressedMisc;
    }
}
