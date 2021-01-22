using System.Collections.Generic;
using System.ComponentModel;

namespace SCide.WPF.Models
{
    public interface ICommenceModel : INotifyPropertyChanged
    {
        IList<string> Categories { get; }
        IList<string> Connections { get; }
        IList<string> Fields { get; }
        IList<string> Forms { get; }
        IList<ICommenceItem> Items { get; }
        string Name { get; }
        string Path { get; }
        bool CanSave(string path);
        bool CheckInFormScript(Commence.ICommenceScript scriptData);
        string CheckOutFormScript(string categoryName, string formName);
        void OnPropertyChanged(string propertyName);
        void GetFormNames(string categoryName);
        void Focus();
        string GetFormXmlFile(string categoryName, string formName);
    }
}