using System;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.LoadSave;

public interface IModelFormat<T>
{
    public Model Load(String path);
    
    public void Save(String path, Model model);
}