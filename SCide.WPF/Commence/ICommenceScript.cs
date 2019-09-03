using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCide.WPF.Commence
{
    public interface ICommenceScript
    {
        string CategoryName { get; }
        IList<CommenceConnection> Connections { get; }
        IList<IDFControl> Controls { get; }
        string DatabaseName { get; }
        string DatabasePath { get; }
        IList<CommenceField> Fields { get; }
        string FilePath { get; }
        string FormName { get; }
        string FullName { get; }
        CommenceConnection SelectedConnection { get; }
        IDFControl SelectedControl { get; }
        CommenceField SelectedField { get; }
        Task GetMetaDataAsync();
    }
}