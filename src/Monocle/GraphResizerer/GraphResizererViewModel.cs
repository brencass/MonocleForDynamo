﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynamo.Extensions;
using Dynamo.Models;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;

namespace MonocleViewExtension.GraphResizerer
{
    internal class GraphResizererViewModel : ViewModelBase
    {
        public GraphResizererModel Model { get; set; }
        private ReadyParams _readyParams;
        public DelegateCommand ResizeGraph { get; set; }
        public DelegateCommand Link { get; set; }
        public DelegateCommand Close { get; set; }

        private double _xScaleFactor;
        public double XScaleFactor
        {
            get { return _xScaleFactor; }
            set { _xScaleFactor = value; RaisePropertyChanged(() => XScaleFactor); }
        }
        private double _yScaleFactor;
        public double YScaleFactor
        {
            get { return _yScaleFactor; }
            set { _yScaleFactor = value; RaisePropertyChanged(() => YScaleFactor); }
        }
        private string _results;
        public string Results
        {
            get { return _results; }
            set { _results = value; RaisePropertyChanged(() => Results); }
        }
        private bool _resultsVisibility;
        public bool ResultsVisibility
        {
            get { return _resultsVisibility; }
            set { _resultsVisibility = value; RaisePropertyChanged(() => ResultsVisibility); }
        }
        private int _runCount;
        public int RunCount
        {
            get { return _runCount; }
            set { _runCount = value; RaisePropertyChanged(() => RunCount); }
        }
        public GraphResizererViewModel(GraphResizererModel m)
        {
            Model = m;
            _readyParams = m.LoadedParams;

            XScaleFactor = 1.5;
            YScaleFactor = 2.25;

            RunCount = 0;

            ResizeGraph = new DelegateCommand(OnResizeGraph);
            Link = new DelegateCommand(OnLink);
            Close = new DelegateCommand(OnClose);

            ResultsVisibility = false;

            Model.SetRunStatus();
        }
        private void OnResizeGraph(object o)
        {
            //if it is the first run, then store the original locations
            if (RunCount == 0)
            {
                Model.GetNodes();
            }
            var changeCount = Model.ResizeGraph(XScaleFactor,YScaleFactor);
            RunCount++;

            Results = $"{changeCount} nodes and notes changed, please review your results before saving.";
            ResultsVisibility = true;
        }

        private void OnLink(object o)
        {
            Process.Start("https://forum.dynamobim.com/t/graph-resizer-for-dynamo-2-13/75612");
        }
        private void OnClose(object o)
        {
            ResultsVisibility = false;
            RunCount = 0;
            GraphResizererView win = o as GraphResizererView;
           win.Close();
        }
    }
}
