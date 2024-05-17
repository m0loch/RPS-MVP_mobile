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
    private ContainerType currentState = ContainerType.None;

    public static event Action<ContainerType> OnStateChanged;
    public static event Action OnBackButtonPressed;

    public static void RaiseBackButtonPressed() => OnBackButtonPressed?.Invoke();

    public override void _Ready()
    {
        base._Ready();

        containers = GetChildren()
            .Where(element => element is UIContainer)
            .Cast<UIContainer>()
            .ToDictionary(element => element.container);

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
            Debug.Print("connecting");

            // Connection has just been performed
            foreach (var container in containers.Values) {
                container.OnProfileLoaded(userId);
            }

            SwitchState(ContainerType.MainMenu);
        }
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

    private void NotifyBackButtonPressed()
    {
        containers[currentState].OnCancel();
    }
}
