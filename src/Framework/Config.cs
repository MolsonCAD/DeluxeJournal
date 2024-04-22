﻿namespace DeluxeJournal.Framework
{
    internal class Config
    {
        /// <summary>Enable to show an indicator on the journal button when a task is completed.</summary>
        public bool EnableVisualTaskCompleteIndicator { get; set; } = false;

        /// <summary>Show the "Smart Add" info box in the "Add Task" window.</summary>
        public bool ShowSmartAddTip { get; set; } = true;

        /// <summary>Show the help message when the task page is empty.</summary>
        public bool ShowAddTaskHelpMessage { get; set; } = true;

        /// <summary>Toggle between "Net Wealth" and "Total Amount to Pay/Gain" display modes.</summary>
        public bool MoneyViewNetWealth { get; set; } = false;
    }
}
