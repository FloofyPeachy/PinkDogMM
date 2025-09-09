using System.ComponentModel;

namespace PinkDogMM_Gd.Core.Configuration;

public interface IAppSettings : INotifyPropertyChanged
{
    IGeneralSettings General { get; set; }
    
}