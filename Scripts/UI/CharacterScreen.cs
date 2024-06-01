using Godot;
using System;
using System.Diagnostics;

public partial class CharacterScreen : UIContainer
{
    [Export] private LineEdit nameField;
    [Export] private OptionButton clanSelector;
    [Export] private Button saveBtn;

    private string currentUserName;
    private int currentClan;

    public override void _Ready()
    {
        base._Ready();

        saveBtn.Disabled = true;
        saveBtn.Pressed += OnSaveBtnPressed;
    }

    public override void OnProfileLoaded()
    {
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_Profile");

        string url = $"{APICfg.profile}/?id={stateMachine.GetUserId()}";
        httpRequest.Request(url);
    }

    private void OnLoadCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: couldn't load user profile");
            return;
        }

        var json = new Json();
        json.Parse(body.GetStringFromUtf8());
        
        var response = json.Data.AsGodotDictionary();
        if ((int)response["result"] == 0) {
            Debug.Print("New account");
            return;
        }

        var data = response["data"].AsGodotDictionary();
        nameField.Text = currentUserName = data["userName"].ToString();
        clanSelector.Selected = currentClan = (int)data["clan"];
    }

    private void OnSaveBtnPressed()
    {
        saveBtn.Disabled = true;

        if (IsUserNameChanged())
        {
            // Validate UserName
            HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_UserNameValidation");

            string url = $"{APICfg.validateProfile}/?userName={nameField.Text}";

            // Load character data from Firestore
            httpRequest.Request(url);
        }

        PerformSave();
    }

    private void OnUserNameValidationCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: Could not validate userName");
            return;
        }

        // Read call result
        var json = new Json();
        json.Parse(body.GetStringFromUtf8());
        
        var response = json.Data.AsGodotDictionary();

        if ((int)response["result"] == 0) {
            MainViewController.RaiseFirebaseError("ERROR: UserName is already in use");
            saveBtn.Disabled = false;
            return;
        }

        // No blocker: go ahead with the save process
        PerformSave();
    }

    private void PerformSave()
    {
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_Save");

        string url = $"{APICfg.profile}/?id={stateMachine.GetUserId()}";
        
        var obj = new Godot.Collections.Dictionary();
        obj["id"] = stateMachine.GetUserId();
        obj["userName"] = nameField.Text;
        obj["clan"] = clanSelector.Selected;

        string payload = Json.Stringify(obj);
        string[] hdrs = new string[] { "Content-Type: application/json" };

        // Save character on Firestore
        httpRequest.Request(url, hdrs, HttpClient.Method.Post, payload);
    }

    private void OnSaveCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        Debug.Print("OnSaveCompleted!");

        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: Could not save profile");
            saveBtn.Disabled = false;
        }

        // Update values
        currentUserName = nameField.Text;
        currentClan = clanSelector.Selected;
    }

    public override void OnCancel()
    {
        // TODO: warn user he's not saving
        stateMachine.SwitchState(ContainerType.MainMenu);
    }

#region UI Management
    private bool IsClanChanged()
    {
        return clanSelector.Selected != currentClan;
    }

    private bool IsUserNameChanged()
    {
        return nameField.Text != currentUserName;
    }

    private bool IsDataChanged()
    {
        return IsClanChanged() || IsUserNameChanged(); 
    }

    public void UpdateSaveBtnState()
    {
        saveBtn.Disabled = (nameField.Text.Length == 0) || !IsDataChanged();
    }

    public void OnClanEdited(int clan)
    {
        UpdateSaveBtnState();
    }

    public void OnUserNameEdited(string userName)
    {
        UpdateSaveBtnState();
    }
#endregion
}
