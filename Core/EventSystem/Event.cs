using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    // Event system
    public struct Event
    {
        internal static IEventHandler? mouseOwner = default;
        public static Event current = default;

        public MouseButton Button;          // Which mouse button was pressed.
        public char Character;              // The character typed.
        public Vector2 Delta;               // The relative movement of the mouse compared to last event.
        public KeyboardKey KeyCode;         // The raw key code for keyboard events.
        public EventModifiers Modifiers;    // 	Which modifier keys are held down.
        public Vector2 MousePosition;       // 	The mouse position.
        public EventType Type;              // The type of event.

        public readonly bool Alt => Modifiers.HasFlag(EventModifiers.Alt);                          // Is Alt/Option key held down? 
        public readonly bool CapsLock => Modifiers.HasFlag(EventModifiers.CapsLock);                // Is Caps Lock on?
        public readonly bool Command => Modifiers.HasFlag(EventModifiers.Command);                  // Is Command/Windows key held down?
        public readonly bool Control => Modifiers.HasFlag(EventModifiers.Control);                  // Is Control key held down?
        public readonly bool FunctionKey => Modifiers.HasFlag(EventModifiers.FunctionKey);          // Is the current keypress a function key?
        public readonly bool IsKey => Type is EventType.KeyDown or EventType.KeyUp;                 // Is this event a keyboard event?
        public readonly bool IsMouse => Type is >= EventType.MouseDown and <= EventType.MouseDrag;  // Is this event a mouse event? 
        public readonly bool Numeric => KeyCode is >= KeyboardKey.Kp0 and <= KeyboardKey.KpEqual;   // Is the current keypress on the numeric keyboard?

        public void Use()
        {
            if (Type is EventType.Layout or EventType.Repaint)
                return;

            Type = EventType.Used;
            current = this;
        }
    }
}
