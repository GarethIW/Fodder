namespace Fodder.Core.UX
{
    using System;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Common human control interface to abstract away player interaction implementation
    /// from the core Fodder game engine.
    /// </summary>
    /// <remarks>
    /// Fodder.Windows will implement <see cref="IHumanPlayerControls"/> with a Keyboard 
    /// and mouse and the Fodder.WindowsPhone will implemen with the touch device controls.
    /// </remarks>
    public interface IHumanPlayerControls
    {
        /// <summary>
        /// Indicates a direction, if any - in which the player would like to zoom on the map.
        /// </summary>
        ZoomDirection Zoom { get; }
        
        /// <summary>
        /// Indicates a direction, if any - in which the player would like to scroll on the map.
        /// </summary>
        ScrollDirection Scroll { get; }

        /// <summary>
        /// Indicates the last known (potentially current) x-axis location of the player's
        /// main pointer.
        /// </summary>
        Int32 X { get; }

        /// <summary>
        /// Indicates the last known (potentially current) y-axis location of the player's
        /// main pointer.
        /// </summary>
        Int32 Y { get; }

        /// <summary>
        /// Indicates that the player is trying to select a game object at the current location.
        /// </summary>
        Boolean Select { get; }

        /// <summary>
        /// Indicates that the player is trying to reset the game.
        /// </summary>
        Boolean Reset { get; }

        /// <summary>
        /// Method to determine if a UI button is being pressed by use of a shortcut key.
        /// </summary>
        /// <param name="key">The key to check the pressed status of.</param>
        /// <returns>True if the key is pressed.</returns>
        Boolean IsButtonShortcutKeyPressed(Keys key);

        /// <summary>
        /// 
        /// </summary>
        Boolean IsPhone { get; }
    }
}
