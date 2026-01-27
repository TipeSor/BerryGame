namespace BerryEngine
{
    public enum EventType
    {
        Used = 0,       // Already processed event.
        MouseDown,	    // Mouse button was pressed.
        MouseUp,	    // Mouse button was released.
        MouseMove,	    // Mouse was moved (Editor views only).
        MouseDrag,	    // Mouse was dragged.
        KeyDown,	    // A keyboard key was pressed.
        KeyUp,	        // A keyboard key was released.
        ScrollWheel,	// The scroll wheel was moved.
        Repaint,	    // A repaint event. One is sent every frame.
        Layout,	        // A layout event.
        Ignore,	        // Event should be ignored.
        ContextClick, 	// User has right-clicked (or control-clicked on the mac).
    };
}
