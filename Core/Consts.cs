using System.Collections.Generic;
using Godot;

namespace PinkDogMM_Gd.Core;

public class Consts
{
    public static Dictionary<int, Color> cornerToColor = new Dictionary<int, Color>
    {
        { 0, Color.FromHtml("#FF0000") },
        { 1, Color.FromHtml("#00FF00") },
        { 2, Color.FromHtml("#0000FF") },
        { 3, Color.FromHtml("#8000FF") },
        { 4, Color.FromHtml("#FF00FF") },
        { 5, Color.FromHtml("#00FFFF") },
        { 6, Color.FromHtml("#FF7F00") },
        { 7, Color.FromHtml("#FFFFFF") },
    };
}