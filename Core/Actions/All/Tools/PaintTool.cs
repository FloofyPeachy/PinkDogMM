using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PinkDogMM_Gd._3D.Tools;
using PinkDogMM_Gd.Core.Actions.All.Tools.Textures;
using PinkDogMM_Gd.Core.Commands;

namespace PinkDogMM_Gd.Core.Actions.All.Tools;

public class PaintTool : Tool
{
     
    private Color _primaryColor = Colors.White;
    private Color _secondaryColor = Colors.Black;
    
    public PaintTool()
    {
       
    }

    public override void Tick(Dictionary arguments)
    {
        
    }
    
   
    public static string Path = "Tools/PaintTool";
    public override string Icon => "icon_pinsel";
    
    public override bool AddToStack => true;
    
    public override bool Sticky() => true;
    public override Type Tool3D() => typeof(PaintTool3D);
}