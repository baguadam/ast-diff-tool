using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels.Interfaces
{
    public interface IEventAggregator
    {
        void Publish<TMessage>(TMessage message);
        void Subscribe<TMessage>(Action<TMessage> action);
    }
}
