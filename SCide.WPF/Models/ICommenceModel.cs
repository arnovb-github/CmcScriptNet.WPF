using System.Collections.Generic;
using System.ComponentModel;

namespace SCide.WPF.Models
{
    public interface ICommenceModel : INotifyPropertyChanged
    {
        IList<string> Categories { get; set; }
        IList<string> Connections { get; set; }
        IList<string> Fields { get; set;  }
        IList<string> Forms { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        bool CanSave(string path);
        bool CheckInFormScript(Commence.CommenceScript scriptData);
        string CheckOutFormScript(string categoryName, string formName);
        void OnPropertyChanged(string propertyName);
        void GetFormNames(string categoryName);
        void Focus();
    }
}