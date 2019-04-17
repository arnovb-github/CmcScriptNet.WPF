﻿using SCide.WPF.Configuration;
using SCide.WPF.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCide.WPF.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowModel()
        {
            CommenceModel = new CommenceModel();
            StatusBarModel = new StatusBarModel();
            EditorSettings = EditorSettings.Instance;
        }

        //private bool canEdit;
        //public bool CanEdit
        //{
        //    get
        //    {
        //        return canEdit;
        //    }
        //    set
        //    {
        //        canEdit = value;
        //        OnPropertyChanged();
        //    }
        //}

        public CommenceModel CommenceModel { get; set; }
        public StatusBarModel StatusBarModel { get; set; }

        private List<IdentifierMatch> list;
        public List<IdentifierMatch> IdentifierMatches
        {
            get { return list; }
            set
            {
                list = value;
                OnPropertyChanged();
            }
        }

        private IdentifierMatch selectedIdentifierMatch;
        public IdentifierMatch SelectedIdentifierMatch
        {
            get { return selectedIdentifierMatch; }
            set
            {
                selectedIdentifierMatch = value;
                OnPropertyChanged();
            }
        }

        private EditorSettings editorSettings;
        public EditorSettings EditorSettings
        {
            get
            {
                return editorSettings;
            }
            set
            {
                editorSettings = value;
                OnPropertyChanged();
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
