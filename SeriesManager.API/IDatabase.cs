using GUI.Enums;
using GUI.MVVM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GUI
{
    public interface IDatabase
    {
        Task<IEnumerable<SeriesModel>> Search(string searchQuery);
        Task<SeriesModel> Load(SeriesModel seriesModel);
    }
}
