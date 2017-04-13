using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Utils.Asynchronous
{
    public interface IMediator
    {
        /// <summary>
        /// Registers a specific recipient for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to register.</param>
        /// <param name="handler">The handler method.</param>
        /// <param name="returnHandler">The handler with register result</param>
        /// <param name="context">Optional, if assign recepent will receive only message with this context, in other case will get all T type Messages</param>
        Task RegisterAsync<TMessage>(object recipient, Action<TMessage> handler, Action<bool> returnHandler = null, object context = null);

        /// <summary>
        /// Unregisters a specific recipient for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="context">Optional, if assign unregister only for this context, if null unregister for all contexts</param>
        Task UnregisterAsync<TMessage>(object recipient, object context = null) where TMessage : IMessage;

        /// <summary>
        /// Unregisters a specific recipient for all messages
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        Task UnregisterRecipientAsync(object recipient);

        /// <summary>
        /// Multicast a message to all message targets for a given message.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="sender">The message sender</param>
        /// <param name="message">The message parameter.</param>
        /// <param name="context">Optional, if assign message will be sent to all recipents registred with <c>this context</c> all with <c>null context</c></param>
        Task SendMessageAsync<TMessage>(object sender, TMessage message, object context = null);
    }
}
