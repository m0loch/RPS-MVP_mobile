using Godot;
using System;
using System.Diagnostics;

public partial class FightScreen : UIContainer
{
    [Export] private Label nextTournamentLbl;

    [Export] private ToggleButton participateBtn;

    [Export] private Button backBtn;

    [Export] private string SignUpText;
    [Export] private string WithdrawText;

    private string userId;
    private bool isParticipating = false;
    private DateTime? nextDate = null;

    public override void _Ready()
    {
        base._Ready();

        backBtn.Pressed += OnBackBtnPressed;
        participateBtn.Pressed += OnParticipateBtnPressed;
    }

    public override void OnProfileLoaded(string userId)
    {
        this.userId = userId;

        // Loads data for the next tournament
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_NextTournament");
        httpRequest.Request($"{APICfg.nextTournament}");

        // Loads registration, if any
        HttpRequest httpRequest2 = GetNode<HttpRequest>("HTTPRequest_Registration");
        httpRequest2.Request($"{APICfg.registration}/?id={userId}");
    }

    private void OnNextTournamentLoaded(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: couldn't load next tournament data");
            return;
        }

        Debug.Print("Check data on new tournament");

        var json = new Json();
        json.Parse(body.GetStringFromUtf8());

        var response = json.Data.AsGodotDictionary();

        if ((int)response["result"] == 0)
        {
            Debug.Print("No new tournament");
            nextTournamentLbl.Text = "No data on future tournaments yet";
            return;
        }

        nextDate = Utils.UnixTimeStampToDateTime((double)response["date"]);
        nextTournamentLbl.Text = $"Next tournament at {Utils.GetDateLabel(nextDate)}";

        participateBtn.Visible = true;
    }

    private void OnRegistrationLoaded(long result, long responseCode, string[] headers, byte[] body)
    {
        Debug.Print($"Registration: responseCode is {responseCode}");
        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: error handling registration data");
            return;
        }

        var json = new Json();
        json.Parse(body.GetStringFromUtf8());
        
        var response = json.Data.AsGodotDictionary();

        isParticipating = (int)response["result"] != 0;
        participateBtn.SetPressed(isParticipating);
        participateBtn.Text = isParticipating ? WithdrawText : SignUpText;
    }

    private void OnParticipateBtnPressed()
    {
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_Registration");
        var obj = new Godot.Collections.Dictionary();
        obj["id"] = userId;

        string payload = Json.Stringify(obj);
        string[] hdrs = new string[] { "Content-Type: application/json" };

        // If the user is participating, delete the registration, otherwise post a new one
        httpRequest.Request(
            $"{APICfg.registration}",
            hdrs,
            isParticipating ? HttpClient.Method.Delete : HttpClient.Method.Post,
            payload
        );
    }

    private void OnBackBtnPressed()
    {
        stateMachine.SwitchState(ContainerType.MainMenu);
    }
}
