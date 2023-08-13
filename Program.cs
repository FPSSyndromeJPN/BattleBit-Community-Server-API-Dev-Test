using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();
        listener.Start(8333);

        Thread.Sleep(-1);
    }
}
class MyPlayer : Player<MyPlayer>
{
    public int TotalPoints;
    public int Streak;
    public List<MyPlayer> players;
}
class MyGameServer : GameServer<MyPlayer>
{
    private List<MyPlayer> players;

    public override async Task<bool> OnPlayerTypedMessage(MyPlayer player, ChatChannel channel, string msg)
    {
        // Sending "/end" to chat will force the round to end
        if (msg == "/end")
        {
            ForceEndGame();
        }
        return true;
    }

    public override async Task OnRoundStarted()
    {
        await Console.Out.WriteLineAsync("Round Started"); 

        AnnounceShort("Kill enemies and steal their kill scores! GOOD LUCK!");

        // Initializing variables
        // Get all players
        players = AllPlayers.ToList();

        // Reset all player kill counts
        foreach (var p in players)
        {
            p.TotalPoints = 0;
            p.Streak = 0;
        }

    }

    public override async Task OnRoundEnded()
    {
        await Console.Out.WriteLineAsync("Round Ended");

        // Displaying the player with the most kills
        var topkillplayer = players.OrderByDescending(p => p.TotalPoints).FirstOrDefault();
        AnnounceLong($"KillScore Top Player {topkillplayer.Name} / {topkillplayer.TotalPoints} Points");
    }
    public override async Task OnPlayerDied(MyPlayer player)
    {
        // Reset points and streaks when a player dies
        player.TotalPoints = 0;
        player.Streak = 0;
    }
    public override async Task OnPlayerConnected(MyPlayer player)
    {
        // Send message to chat when player joins server
        SayToChat($"JOIN Player: {player.Name}, Welcome!");
    }

    public override async Task OnAPlayerKilledAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
    {
        // Whether the victim is on another team
        if (args.Victim.Team != args.Killer.Team)
        {
            // Add 1 kill point
            args.Killer.TotalPoints += 1;
            args.Killer.Streak += 1;

            // Steal all kill points from the victim
            args.Killer.TotalPoints += args.Victim.TotalPoints;
            args.Killer.Message("You got all the victim's kill points!");

            // Victim loses all kill points
            args.Victim.TotalPoints = 0;
            args.Victim.Message("YOUR POINTS: 0\nYOUR STREAK: 0\nYou lost all your kill points!");

            // Killstreak processing and its message processing
            if (args.Killer.Streak >= 1)
            {
                // Assign message to variable
                string killer_mes = $"{args.Killer.Streak} Killstreak! [Kill Weapon: {args.KillerTool}]";
                string announce_mes = $"{args.Killer.Name}: {args.Killer.Streak} Killstreak! [Kill Weapon: {args.KillerTool}]";

                // Show total points and killstreaks on the killer screen
                args.Killer.Message($"YOUR POINTS: {args.Killer.TotalPoints}\nYOUR STREAK: {args.Killer.Streak}");

                // Show current top players in announcement popup
                var topkillplayer = players.OrderByDescending(p => p.TotalPoints).FirstOrDefault();
                AnnounceShort($"Current Top Player {topkillplayer.Name} / {topkillplayer.TotalPoints} Points");

                // Log Current Top Players
                await Console.Out.WriteLineAsync($"Current Top Player {topkillplayer.Name} / {topkillplayer.TotalPoints} Points");

                // Send a message in chat at Killstreak (3, 5, 10, 20)
                switch (args.Killer.Streak)
                {
                    case 3:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        break;
                    case 5:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        break;
                    case 10:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        break;
                    case 20:
                        args.Killer.Message(killer_mes);
                        SayToChat(announce_mes);
                        break;
                }

            }

        }

    }

    public override async Task OnConnected()
    {
        await Console.Out.WriteLineAsync("Connected");
        await Console.Out.WriteLineAsync("Server: " + RoundSettings.State);

        // Initializing variables
        // Get all players
        players = AllPlayers.ToList();

        // Reset all player kill counts
        foreach (var p in players)
        {
            p.TotalPoints = 0;
            p.Streak = 0;
        }
    }

    public override async Task OnGameStateChanged(GameState oldState, GameState newState)
    {
        // Server state
        await Console.Out.WriteLineAsync("Server: " + newState);
    }

}
