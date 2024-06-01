using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class TournamentReportScreen : UIContainer
{
    [Export] private Label textLabel;

    private List<Label> discardableLabels = new List<Label>();

    public override void _Ready()
    {
        base._Ready();

        MainViewController.OnTournamentSelected += OnTournamentSelected;
    }

    private void OnTournamentSelected(string date, Godot.Collections.Array data)
    {
        textLabel.Text = $"Tournament held on: {date}";
        foreach (var item in data) {
            var newLabel = textLabel.Duplicate() as Label;
            newLabel.Text = FormatRound(item.AsGodotDictionary());
            AddChild(newLabel);
            discardableLabels.Add(newLabel);

            // Back button should always be the last control
            MoveChild(newLabel, newLabel.GetIndex() - 1);
        }
    }

    public override void OnCancel()
    {
        stateMachine.SwitchState(ContainerType.HistoryScreen);

        foreach (var item in discardableLabels)
        {
            RemoveChild(item);
            item.QueueFree();
        }

        discardableLabels.Clear();
    }

    private string FormatRound(Godot.Collections.Dictionary round)
    {
        string result = "";
        if (round.ContainsKey("matches"))
        {
            result += $"Round {round.Count - (int)round["round"]}:\n";

            var matches = round["matches"].AsGodotArray();

            // Low-level search to accomodate for Godot structures
            Godot.Collections.Dictionary match = null;
            for (int i = 0; i < matches.Count; i++)
            {
                var currMatch = matches[i].AsGodotDictionary();
                var currPlayers = currMatch["players"].AsGodotArray();
                if (currPlayers[0].AsGodotDictionary()["id"].ToString() == stateMachine.GetUserId()
                || currPlayers[1].AsGodotDictionary()["id"].ToString() == stateMachine.GetUserId())
                {
                    match = currMatch;
                    break;
                }
            }

            var players = match["players"].AsGodotArray();
            var p1 = players[0].AsGodotDictionary();
            var p2 = players[1].AsGodotDictionary();
            var winner = match["winner"].AsGodotDictionary();
            result += $"\"{p1["userName"]}\" of clan {Utils.GetClan((int)p1["clan"])}\nfought\n\"{p2["userName"]}\" of clan {Utils.GetClan((int)p2["clan"])}\n";
            result += $"\n\"{winner["userName"]}\" of clan {Utils.GetClan((int)winner["clan"])} won its bout";

        }
        else 
        {
            Dictionary winner = round["winner"].AsGodotDictionary();
            result += $"\"{winner["userName"]}\" of clan {Utils.GetClan((int)winner["clan"])} was declared winner of the tournament!";
        }
        return result;
    }
}
