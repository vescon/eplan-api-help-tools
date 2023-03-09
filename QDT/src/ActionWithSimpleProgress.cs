using System;
using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.MasterData;

namespace Vescon.EplAddin.Qdt
{
    public class ActionWithSimpleProgress
    {
        private readonly Project _project;
        private readonly string _title = Addin.Title;
        private readonly bool _immediate;

        private Action<Exception> _errorHandler;

        public ActionWithSimpleProgress(
            Project project,
            bool immediate = false)
        {
            _project = project;
            _immediate = immediate;
        }

        public ActionWithSimpleProgress(
            Project project, 
            string title, 
            bool immediate = false)
        : this(
            project, 
            immediate)
        {
            _title = title;
        }

        public ActionWithSimpleProgress OnError(Action<Exception> errorHandler)
        {
            _errorHandler = errorHandler;
            return this;
        }

        public bool RunSteps(params Action<Progress>[] actions)
        {
            using (var prgrss = new Progress("SimpleProgress"))
            {
                prgrss.SetTitle(_title);
                prgrss.SetAllowCancel(true);
                prgrss.SetAskOnCancel(true);

                if (_immediate)
                    prgrss.ShowImmediately();

                prgrss.SetNeededSteps(actions.Length);

                try
                {
                    foreach (var a in actions)
                    {
                        a(prgrss);

                        prgrss.Step(1);
                        if (prgrss.Canceled())
                            return false;
                    }
                }
                catch (Exception e)
                {
                    _errorHandler?.Invoke(e);
                    throw;
                }

                return true;
            }
        }

        public bool IterateOverProjectSymbols(Action<Symbol> action)
        {
            var symbols = _project.SymbolLibraries
                .SelectMany(x => x.Symbols.Select(y => new
                {
                    Library = x, 
                    Symbol = y
                }));

            var steps = symbols
                .Select(x => new Action<Progress>(prgrss =>
                {
                    prgrss.SetActionText($"Symbol: {x.Library.Name} / {x.Symbol.Name}");

                    action(x.Symbol);
                }))
                .ToArray();

            return RunSteps(steps);
        }

        public bool IterateOverPages(Action<Page> action)
        {
            return IterateOverPages(
                _project.Pages.ToList(),
                action);
        }

        public bool IterateOverPages(IList<Page> pages, Action<Page> action)
        {
            var steps = pages
                .Select(x => new Action<Progress>(prgrss =>
                {
                    prgrss.SetActionText($"Page '{x.IdentifyingName}'");

                    action(x);
                }))
                .ToArray();

            return RunSteps(steps);
        }
    }
}