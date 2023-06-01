using System;

namespace Engine
{
    /// <summary>
    /// Provides data for the message event.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an extra new line should be added.
        /// </summary>
        public bool AddExtraNewLine { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class with the specified message and addExtraNewLine flag.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="addExtraNewLine">A value indicating whether an extra new line should be added.</param>
        public MessageEventArgs(string message, bool addExtraNewLine)
        {
            Message = message;
            AddExtraNewLine = addExtraNewLine;
        }

    }
}