using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using System.ComponentModel;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();
        listener.Start(29294);

        Thread.Sleep(-1);
    }
}
class MyPlayer : Player<MyPlayer>
{
    public int TotalKills;
    public List<MyPlayer> players;
}
class MyGameServer : GameServer<MyPlayer>
{
    private List<MyPlayer> players;

    public override async Task OnRoundStarted()
    {
        await Console.Out.WriteLineAsync("Round Started"); 

        AnnounceShort("Kill enemies and steal their kill scores! GOOD LUCK!");

        // Get all players
        players = AllPlayers.ToList();

        // Reset all player kill counts
        foreach (var p in players)
        {
            p.TotalKills = 0;
        }

    }

    public override async Task OnRoundEnded()
    {
        await Console.Out.WriteLineAsync("Round Ended");

        // Displaying the player with the most kills
        var topkillplayer = players.OrderByDescending(p => p.TotalKills).FirstOrDefault();
        AnnounceLong($"KillScore Top Player {topkillplayer.Name} / {topkillplayer.TotalKills} Kills");
    }

    public override async Task OnPlayerConnected(MyPlayer player)
    {
        SayToChat($"JOIN Player: {player.Name}, Welcome!");
    }

    public override async Task OnAPlayerKilledAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
    {
        if (args.Victim.Team != args.Killer.Team)
        {
            // Add 1 kill point
            args.Killer.TotalKills += 1;

            // Steal all kill points from the victim
            args.Killer.TotalKills += args.Victim.TotalKills;
            args.Killer.Message("You got all the victim's kill points!");

            // Victim loses all kill points
            args.Victim.TotalKills = 0;
            args.Victim.Message("You lost all your kill points!");

            // Killstreak processing and its message processing
            if (args.Killer.TotalKills > 3)
            {
                string killer_mes = $"{args.Killer.TotalKills} Killstreak! Nice! [Kill Weapon: {args.KillerTool}]";
                string announce_mes = $"{args.Killer.Name}: {args.Killer.TotalKills} Killstreak! [Kill Weapon: {args.KillerTool}]";

                switch (args.Killer.TotalKills)
                {
                    case 3:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        return;
                    case 5:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        return;
                    case 10:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        return;
                    case 20:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        return;
                }
                var topkillplayer = players.OrderByDescending(p => p.TotalKills).FirstOrDefault();
                AnnounceShort($"Current Top Player {topkillplayer.Name} / {topkillplayer.TotalKills} Kills");
            }

        }

    }

    public override async Task OnConnected()
    {
        await Console.Out.WriteLineAsync("Server: " + RoundSettings.State);
    }

    public override async Task OnGameStateChanged(GameState oldState, GameState newState)
    {
        await Console.Out.WriteLineAsync("Server: " + newState);
    }
}
