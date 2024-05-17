using Godot;
using System;
using System.Diagnostics;

public partial class UIContainer : Container
{
    [Export] public ContainerType container { get; private set; }

    protected MainViewController stateMachine;

    public virtual void OnProfileLoaded(string userId) { }
    public virtual void OnCancel()
    {
        stateMachine.SwitchState(ContainerType.MainMenu);
    }

    public override void _Ready()
    {
        base._Ready();

        stateMachine = GetParent<MainViewController>();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == StateConstants.ENABLED)
        {
            EnterState();
        }
        else if (what == StateConstants.DISABLED)
        {
            ExitState();
        }
    }

    private void EnterState()
    {
        Debug.Print($"{Name}: ON!");
        Visible = true;
    }

    private void ExitState()
    {
        Debug.Print($"{Name}: OFF...");
        Visible = false;
    }
}
