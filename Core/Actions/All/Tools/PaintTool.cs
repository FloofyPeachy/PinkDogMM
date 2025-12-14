using System;
using PinkDogMM_Gd._3D.Tools;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class PaintTool : Tool
{
    public static string Path = "Tools/PaintTool";
    public override string Icon => "icon_pinsel";
    
    public override bool AddToStack => true;
    
    public override bool Sticky() => true;
    public override Type Tool3D() => typeof(PaintTool3D);
}