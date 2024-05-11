using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public partial class MainViewController : PanelContainer
{
    private Dictionary<ContainerType, UIContainer> containers;

    private string userId = "";

    [Export] private Node manager;

    public override void _Ready()
    {
        base._Ready();

        containers = GetChildren()
            .Where(element => element is UIContainer)
            .Cast<UIContainer>()
            .ToDictionary(element => element.container);
    }
 
    public override void _Process(double delta)
    {
        base._Process(delta);

        var userId = manager.Get("player_id").ToString();

        containers[ContainerType.Login].Visible = userId.Length == 0;
        containers[ContainerType.MainMenu].Visible = userId.Length > 0;
    }
}
