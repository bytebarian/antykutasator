using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Utils.Asynchronous
{
    /// <summary>
    /// Class with implemented Mediator pattern.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<Type, List<ActionInfo>> _registeredHandlers = new Dictionary<Type, List<ActionInfo>>();
        private readonly TaskFactory _taskFactory;

        public Mediator()
        {
            _taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(1));
        }


        /// <summary>
        /// Registers a specific recipient for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to register.</param>
        /// <param name="handler">The handler method.</param>
        /// <param name="returnHandler">The handler with register result</param>
        /// <param name="context">Optional, if assign recepent will receive only message with this context, in other case will get all T type Messages</param>
        public Task RegisterAsync<TMessage>(object recipient, Action<TMessage> handler, Action<bool> returnHandler = null, object context = null)
        {
            return RunAction(() =>
            {
                RegisterInternal(recipient, handler, context);
                if (returnHandler != null)
                {
                    returnHandler.Invoke(true);
                }
            });
        }

        /// <summary>
        /// Unregisters a specific recipient for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="context">Optional, if assign unregister only for this context, if null unregister for all contexts</param>
        public Task UnregisterAsync<TMessage>(object recipient, object context = null)
            where TMessage : IMessage
        {
            return RunAction(() =>
            {
                UnregisterInternal<TMessage>(recipient, context);
            });
        }

        /// <summary>
        /// Unregisters a specific recipient for all messages
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        public Task UnregisterRecipientAsync(object recipient)
        {
            return RunAction(() =>
            {
                UnregisterRecipientIntenral(recipient);
            });
        }

        /// <summary>
        /// Multicast a message to all message targets for a given message.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="sender">The message sender</param>
        /// <param name="message">The message parameter.</param>
        /// <param name="context">Optional, if assign message will be sent to all recipents registred with <c>this context</c> all with <c>null context</c></param>
        public Task SendMessageAsync<TMessage>(object sender, TMessage message, object context = null)
        {
            return RunAction(() =>
            {
                SendMessageInternal(sender, message, context);
            });
        }

        private void RegisterInternal<TMessage>(
            object recipient,
            Action<TMessage> handler,
            object context = null)
        {
            var actionInfo = new ActionInfo
            {
                Recipient = recipient,
                ReceiveAction = handler,
                Context = context
            };

            lock (_lockObject)
            {

                var type = typeof(TMessage);
                List<ActionInfo> actions;
                if (!_registeredHandlers.TryGetValue(type, out actions))
                {
                    actions = new List<ActionInfo>();
                    _registeredHandlers.Add(type, actions);
                }
                actions.Add(actionInfo);
            }
        }

        private void UnregisterInternal<TMessage>(object recipient, object context = null)
            where TMessage : IMessage
        {
            lock (_lockObject)
            {
                var type = typeof(TMessage);
                List<ActionInfo> handlers;
                if (_registeredHandlers.TryGetValue(type, out handlers))
                {
                    handlers.RemoveAll(ai => Equals(ai.Context, context) && recipient == ai.Recipient);
                }
            }
        }

        private void UnregisterRecipientIntenral(object recipient)
        {
            lock (_lockObject)
            {
                foreach (var t in _registeredHandlers.Where(t => t.Value.RemoveAll(ai => recipient == ai.Recipient) > 0))
                {
                }
            }
        }

        private void SendMessageInternal<TMessage>(object sender, TMessage message, object context = null)
        {
            lock (_lockObject)
            {
                var type = typeof(TMessage);
                List<ActionInfo> handlers;
                if (_registeredHandlers.TryGetValue(type, out handlers))
                {
                    // New instance is required because of recurent calls
                    var machedActions = handlers
                        .FindAll(ai => ai.Recipient != sender && (ai.Context == null || Equals(ai.Context, context)));

                    foreach (var ai in machedActions)
                    {
                        ai.ReceiveAction.DynamicInvoke(message);
                    }
                }
            }
        }

        private Task RunAction(Action action)
        {
            return _taskFactory.StartNew(action);
        }

        private class ActionInfo
        {
            public object Recipient { get; set; }
            public Delegate ReceiveAction { get; set; }
            public object Context { get; set; }
        }
    }
}
