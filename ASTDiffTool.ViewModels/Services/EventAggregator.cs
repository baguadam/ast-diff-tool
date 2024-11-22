using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTDiffTool.ViewModels.Interfaces;

namespace ASTDiffTool.ViewModels.Services
{
    /// <summary>
    /// Event Aggregator class that is placed above the view models as service.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Action<object>>> _subscriptions = new();

        /// <summary>
        /// Publishes an event to all subscribers that are subscribed to this event type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the event/message to publish.</typeparam>
        /// <param name="message">The event or message instance to be published.</param>
        public void Publish<TMessage>(TMessage message)
        {
            var messageType = typeof(TMessage);

            if (_subscriptions.ContainsKey(messageType))
            {
                foreach (var action in _subscriptions[messageType])
                {
                    action(message);
                }
            }
        }

        /// <summary>
        /// Subscribes to an event of the specified type with the provided action.
        /// </summary>
        /// <typeparam name="TMessage">The type of the event/message to subscribe to.</typeparam>
        /// <param name="action">The action to invoke when the event is published.</param>
        public void Subscribe<TMessage>(Action<TMessage> action)
        {
            var messageType = typeof(TMessage);

            if (!_subscriptions.ContainsKey(messageType))
            {
                _subscriptions[messageType] = new List<Action<object>>();
            }
            _subscriptions[messageType].Add(message => action((TMessage)message));
        }
    }
}
