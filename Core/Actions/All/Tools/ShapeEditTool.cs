using System;
using PinkDogMM_Gd._3D.Gear;
using PinkDogMM_Gd._3D.Tools;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class ShapeEditTool : Tool
{
    public static string Path = "Tools/PointerTool";
    public override string Icon => "icon_mod_edit";
    
    public override bool AddToStack => false;
    
    public override bool Sticky() => false;
    public override Type Tool3D() => typeof(ShapeEditTool3D);
}