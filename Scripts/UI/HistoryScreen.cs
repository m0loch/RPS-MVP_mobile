using Godot;
using System.Collections.Generic;

public partial class HistoryScreen : UIContainer
{
    [Export] private Label noHistoryLabel;
    [Export] private Button tournamentPrefab;

    private List<Button> discardableButtons = new List<Button>();
    private Dictionary<string, Godot.Collections.Array> tournaments = new();

    public override void OnProfileLoaded()
    {
        HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest_Tournaments");

        httpRequest.Request($"{APICfg.tournaments}/?playerId={stateMachine.GetUserId()}");
    }

    public override void OnProfileUnloaded()
    {
        foreach (var item in discardableButtons)
        {
            RemoveChild(item);
            item.QueueFree();
        }

        discardableButtons.Clear();
    }

    private void OnLoadCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode != 200)
        {
            MainViewController.RaiseFirebaseError("ERROR: couldn't load user tournaments");
            return;
        }

        var json = new Json();
        json.Parse(body.GetStringFromUtf8());
        
        var response = json.Data.AsGodotDictionary();
        var data = response["data"].AsGodotArray();
        
        if (data.Count == 0)
        {
            return;
        }

        noHistoryLabel.Visible = false;

        foreach (var item in data)
        {
            var tournament = item.AsGodotDictionary();
            var tourData = tournament["data"].AsGodotDictionary();

            string buttonLabel = Utils.GetDateLabel(
                Utils.UnixTimeStampToDateTime(
                    (double)tourData["timestamp"].AsGodotDictionary()["_seconds"]
                )
            );

            tournaments[buttonLabel] = tourData["outcome"].AsGodotArray();

            var newCtrl = tournamentPrefab.Duplicate() as Button;
            newCtrl.Visible = true;
            newCtrl.Text = buttonLabel;
            newCtrl.Connect("pressed", Callable.From(() => OnTournamentBtnPressed(newCtrl)));
            AddChild(newCtrl);
            discardableButtons.Add(newCtrl);

            // Back button should always be the last control
            MoveChild(newCtrl, newCtrl.GetIndex() - 1);
        }
    }

    private void OnTournamentBtnPressed(Button sender)
    {
        MainViewController.RaiseTournamentSelected(sender.Text, tournaments[sender.Text]);

        stateMachine.SwitchState(ContainerType.TournamentReportScreen);
    }
}
