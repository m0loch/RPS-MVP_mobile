using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class MainViewController : PanelContainer
{
    private Dictionary<ContainerType, UIContainer> containers;

    private string userId = "";
    private float baseHeight = 0;

    [Export] private Control parent;
    [Export] private Node manager;
    private ContainerType currentState = ContainerType.None;

    public static event Action<ContainerType> OnStateChanged;
    public static event Action<string, Godot.Collections.Array> OnTournamentSelected;
    public static event Action OnBackButtonPressed;

    public static void RaiseBackButtonPressed() => OnBackButtonPressed?.Invoke();
    public static void RaiseTournamentSelected(
        string date,
        Godot.Collections.Array tournament
    ) => OnTournamentSelected?.Invoke(date, tournament);
    public static void RaiseFirebaseError(string error) => Debug.Print(error);

    public override void _Ready()
    {
        base._Ready();

        containers = GetChildren()
            .Where(element => element is UIContainer)
            .Cast<UIContainer>()
            .ToDictionary(element => element.container);

        baseHeight = parent.GetRect().Size.Y;

        OnBackButtonPressed += NotifyBackButtonPressed;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        userId = manager.Get("player_id").ToString();

        if (userId.Length == 0)
        {
            SwitchState(ContainerType.Login);
        }
        else if (currentState == ContainerType.None || currentState == ContainerType.Login)
        {
            // Connection has just been performed
            foreach (var container in containers.Values) {
                container.OnProfileLoaded();
            }

            SwitchState(ContainerType.MainMenu);
        }

        AccomodateKeyboard();
    }

    private void AccomodateKeyboard()
    {
        if (!DisplayServer.HasFeature(DisplayServer.Feature.VirtualKeyboard)) return;

        int keyboardHeight = DisplayServer.VirtualKeyboardGetHeight(); // > 0 equals keyboard shown
        parent.SetSize(new Vector2(parent.GetRect().Size.X, baseHeight - keyboardHeight), true);
    }

    public void SwitchState(ContainerType newState)
    {
        // If this is the target state there's no need to go on 
        if (currentState == newState) { return; }

        Debug.Print($"Switching from {currentState} to {newState}");
    
        if (containers.ContainsKey(currentState)) { containers[currentState].Notification(StateConstants.DISABLED); }
        currentState = newState;
        if (containers.ContainsKey(currentState)) { containers[currentState].Notification(StateConstants.ENABLED); }

        OnStateChanged?.Invoke(newState);
    }

    public string GetUserId() { return userId; }

    private void NotifyBackButtonPressed()
    {
        containers[currentState].OnCancel();
    }
}
