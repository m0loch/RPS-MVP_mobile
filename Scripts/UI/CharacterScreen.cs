using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class CharacterScreen : UIContainer
{
    [Export] private LineEdit nameField;
    [Export] private OptionButton clanSelector;
    [Export] private Button saveBtn;

    private string userId;

    public override void _Ready()
    {
        base._Ready();

        saveBtn.Disabled = true;
        saveBtn.Pressed += OnSaveBtnPressed;
    }

    public override void OnProfileLoaded(string userId)
    {
        this.userId = userId;
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");

        string url = $"{APICfg.profile}/?id={userId}";
        Debug.Print(url);

        // Load character data from Firestore
        httpRequest.RequestCompleted += OnLoadCompleted;
        httpRequest.Request(url);
    }

    private void OnLoadCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
        {
            // TODO: rise error
            return;
        }

        var json = new Json();
        json.Parse(body.GetStringFromUtf8());
        
        var response = json.Data.AsGodotDictionary();
        if ((int)response["result"] == 0) {
            Debug.Print("New account");
            return;
        }

        foreach(var key in response.Keys)
        {
            Debug.Print(key.ToString());
            Debug.Print(response[key].ToString());
        }
        var data = response["data"].AsGodotDictionary();
        nameField.Text = data["userName"].ToString();
        clanSelector.Selected = (int)data["clan"];
        saveBtn.Disabled = false;
    }

    private void OnSaveBtnPressed()
    {
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");

        string url = $"{APICfg.validateProfile}";
        Debug.Print(url);

        // Load character data from Firestore
        httpRequest.RequestCompleted += OnValidationCompleted;
        httpRequest.Request(url);
    }

    private void OnValidationCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        // LETTURA DEL RISULTATO PRIMA DI FARE COSE
        return;

        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");

        string url = $"{APICfg.profile}/?id={userId}";
        
        var obj = new Godot.Collections.Dictionary();
        obj["id"] = userId;
        obj["userName"] = nameField.Text;
        obj["clan"] = clanSelector.Selected;

        string payload = Json.Stringify(obj);
        string[] hdrs = new string[] { "Content-Type: application/json" };

        // Save character on Firestore
        httpRequest.RequestCompleted += OnSaveCompleted;
        httpRequest.Request(url, hdrs, HttpClient.Method.Post, payload);
    }

    private void OnSaveCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        Debug.Print("OnSaveCompleted!");

        if (responseCode != 200)
        {
            // TODO: rise error
            Debug.Print("Could not save...");
            return;
        }

        // Save was ok, we can leave
        stateMachine.SwitchState(ContainerType.MainMenu);
    }

    public override void OnCancel()
    {
        // TODO: warn user he's not saving
        stateMachine.SwitchState(ContainerType.MainMenu);
    }

}
