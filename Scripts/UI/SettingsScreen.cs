using Godot;
using System;

public partial class SettingsScreen : UIContainer
{
    [Export] private Button backBtn;

    public override void _Ready()
    {
        base._Ready();

        backBtn.Pressed += OnBackBtnPressed;
    }

    private void OnBackBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.MainMenu);
    }
}
