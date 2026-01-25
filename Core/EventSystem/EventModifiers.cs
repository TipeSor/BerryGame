namespace BerryGame
{
#pragma warning disable IDE0055
    [Flags]
    public enum EventModifiers
    {
        None        = 0,	    // No modifier key pressed during a keystroke event.
        Shift       = 1 << 0,   // Shift key.
        Control     = 1 << 1,   // Control key.
        Alt         = 1 << 2,	// Alt key.
        Command     = 1 << 3,   // Command key (Mac).
        Numeric     = 1 << 4,	// Num lock key.
        CapsLock    = 1 << 5,	// Caps lock key.
        FunctionKey = 1 << 6,	//Function key.
    }
#pragma warning restore IDE0055
}
