using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Config.Net;
using Godot;
using PinkDogMM_Gd.Core.Actions;

namespace PinkDogMM_Gd.Core.Configuration;

public class Settings
{
    private readonly IAppSettings _config;
    private readonly Dictionary<int, string> _keybindings;

    public Settings()
    {
        _config = new ConfigurationBuilder<IAppSettings>()
            .UseJsonFile("pdmm.json")
            .Build();
        
        PL.I.Info("Loaded settings!!");
        //_keybindings = new KeyBindings();
    }

    private static readonly Settings Singleton = new Settings();

    private void LoadBindings()
    {
        
    }
    public static Settings GetThem()
    {
        return Singleton;
    }
    
    public IGeneralSettings General => _config.General;
    //public IKeybindSettings Keybinds => _keybindings;
}