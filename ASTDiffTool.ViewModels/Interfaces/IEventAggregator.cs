using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Interfaces
{
    /// <summary>
    /// Interface for an event aggregator that allows different parts of the application to communicate by publishing and subscribing to events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Publishes an event to all the subscribers that are interested in the message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message/event being published.</typeparam>
        /// <param name="message">The message or event to publish.</param>
        void Publish<TMessage>(TMessage message);

        /// <summary>
        /// Subscribes to a specific event type, allowing the provided action to be invoked whenever that event is published.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message/event being subscribed to.</typeparam>
        /// <param name="action">The action to be executed when the event is published.</param>
        void Subscribe<TMessage>(Action<TMessage> action);
    }
}
