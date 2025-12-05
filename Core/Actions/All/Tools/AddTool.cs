using System;
using PinkDogMM_Gd._3D.Tools;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class AddTool : Tool
{
    public static string Path = "Tools/AddTool";
    public override string Icon => "icon_autotexture";
    
    public override bool AddToStack => false;
    
    public override bool Sticky() => false;
    public override Type Tool3D() => typeof(AddTool3D);
}