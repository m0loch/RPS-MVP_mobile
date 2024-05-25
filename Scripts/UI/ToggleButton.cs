using Godot;

public partial class ToggleButton : Button
{
    private StyleBox ActiveTheme;

    public override void _Ready()
    {
        ActiveTheme = ResourceLoader.Load<StyleBox>("res://Resources/active_button.tres");
    }
    public void SetPressed(bool pressed)
    {
        if (pressed)
        {
            AddThemeStyleboxOverride("normal", ActiveTheme);
        }
        else
        {
            RemoveThemeStyleboxOverride("normal");
        }
    }
}
