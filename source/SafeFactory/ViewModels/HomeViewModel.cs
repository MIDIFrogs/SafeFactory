using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace SafeFactory.ViewModels
{
    internal class HomeViewModel
    {
        // Unit это типа void
        public ReactiveCommand<Unit, Unit> ChooseProject { get; set; }
    }
}
