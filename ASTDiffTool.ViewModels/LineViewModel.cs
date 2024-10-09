using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.ViewModels
{
    public enum LineState
    {
        Normal,
        Highlighted
    }

    public class LineViewModel : ViewModelBase
    {
        private string _line;
        private LineState _state;

        public string Line
        {
            get { return _line; }
            set
            {
                _line = value;
                OnPropertyChanged(nameof(Line));
            }
        }

        public LineState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }
    }
}
