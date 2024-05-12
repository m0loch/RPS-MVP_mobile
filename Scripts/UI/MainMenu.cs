using Godot;
using System;
using System.Diagnostics;

public partial class MainMenu : UIContainer
{
    [Export] private Button characterBtn;
    [Export] private Button fightBtn;
    [Export] private Button historyBtn;
    [Export] private Button trainingBtn;
    [Export] private Button settingsBtn;

    public override void _Ready()
    {
        base._Ready();

        characterBtn.Pressed += OnCharacterBtnPressed;
        fightBtn.Pressed += OnFightBtnPressed;
        historyBtn.Pressed += OnHistoryBtnPressed;
        trainingBtn.Pressed += OnTrainingBtnPressed;
        settingsBtn.Pressed += OnSettingsBtnPressed;
    }

    private void OnCharacterBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.CharacterScreen);
    }

    private void OnFightBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.FightScreen);
    }

    private void OnHistoryBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.HistoryScreen);
    }

    private void OnTrainingBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.TrainingScreen);
    }

    private void OnSettingsBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.SettingsMenu);
    }
}