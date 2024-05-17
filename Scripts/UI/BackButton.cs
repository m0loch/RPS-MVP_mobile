using Godot;
using System;
using System.Reflection.Metadata;

public partial class BackButton : Button
{
    public override void _Ready()
    {
        MainViewController.OnStateChanged += HandleStateChanged;
        Visible = false;
    }

    private void HandleStateChanged(ContainerType state)
    {
        Visible = state != ContainerType.Login && state != ContainerType.MainMenu;
    }

    public void OnPressed()
    {
        MainViewController.RaiseBackButtonPressed();
    }
}
