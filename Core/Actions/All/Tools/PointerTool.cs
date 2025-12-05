using System;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D.Gear;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class PointerTool : Tool
{
    public static string Path = "Tools/PointerTool";
    public override string Icon => "icon_arrow_front2";
    
    public override bool AddToStack => false;
    
    public override bool Sticky() => false;
    public override Type Tool3D() => typeof(PointerTool3D);
   
}