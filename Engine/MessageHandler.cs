using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class MessageHandler
    {
        public static event EventHandler<MessageEventArgs> OnMessage;

        public static void RaiseMessage(string message, bool addExtraNewLine = false)
        {
            OnMessage?.Invoke(null, new MessageEventArgs(message, addExtraNewLine));
        }
    }

}
