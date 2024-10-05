using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTDiffTool.ViewModels.Interfaces;

namespace ASTDiffTool.ViewModels.Services
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Action<object>>> _subscriptions = new();

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
