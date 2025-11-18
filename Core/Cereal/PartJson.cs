using System.Collections.Generic;
using System.Text.Json;
using Godot;
using PinkDogMM_Gd.Core.Schema;

namespace PinkDogMM_Gd.Core.LoadSave;

public class PartJson
{
    public static string PartToJson(Part part)
    {
        return JsonSerializer.Serialize(new Dictionary<string, object>
        {
            {"name", part.Name},
            
        });
    }
}