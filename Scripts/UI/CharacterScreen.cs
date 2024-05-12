using Godot;
using System;

public partial class CharacterScreen : UIContainer
{
    [Export] private Button saveBtn;

    public override void _Ready()
    {
        base._Ready();

        saveBtn.Pressed += OnSaveBtnPressed;
    }

    private void OnSaveBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.MainMenu);
    }
}
