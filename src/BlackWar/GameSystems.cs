namespace BlackWar;

public sealed class Citadel
{
    public List<Contract> ContractBoard { get; } = new();
    public BlackMarket Market { get; } = new();

    public void PreparePlayer(Player player, Random rng)
    {
        if (player.GoldCoins >= 100 && rng.NextDouble() < 0.55)
        {
            player.SpendCoins(100);
            player.Inventory.Add(new Weapon("Auto-Pistol", GroundWeaponCategory.Pistol, 18, 25, 60));
        }
        if (player.GoldCoins >= 150 && rng.NextDouble() < 0.35)
        {
            player.SpendCoins(150);
            player.Vehicle.Repair(35);
        }
    }

    public bool AcceptContract(Player player, Contract contract)
    {
        if (player.ReputationLevel < contract.RequiredLevel || !player.SpendCoins(contract.ActivationFee)) return false;
        player.Contracts.Add(contract);
        return true;
    }
}

public sealed class BattleMap
{
    public const double SizeKm = 8.0;
    public double SafeZoneRadiusKm { get; private set; } = 4.0;
    public int ElapsedMinutes { get; private set; }
    public IReadOnlyList<MapZone> Zones { get; } = Enum.GetValues<MapZone>();

    public RandomEventType Advance(Random rng)
    {
        ElapsedMinutes++;
        if (ElapsedMinutes % 3 == 0) SafeZoneRadiusKm = Math.Max(0.4, SafeZoneRadiusKm - 0.45);
        return rng.NextDouble() switch
        {
            < 0.08 => RandomEventType.Sandstorm,
            < 0.14 => RandomEventType.Explosion,
            < 0.20 => RandomEventType.AirDrop,
            _ => RandomEventType.None
        };
    }
}

public sealed class MatchResult
{
    public Player Winner { get; init; } = null!;
    public List<string> Log { get; } = new();
    public Dictionary<Player, int> Kills { get; } = new();
}

public sealed class MatchEngine
{
    private readonly Random _rng;
    public MatchEngine(Random rng) => _rng = rng;

    public MatchResult Run(List<Player> players)
    {
        var result = new MatchResult();
        var map = new BattleMap();
        var alive = players.ToList();
        var phase = GamePhase.VehicleCombat;
        result.Log.Add($"Tournament started with {alive.Count} mercenaries.");

        while (alive.Count > 1 && map.ElapsedMinutes < 45)
        {
            var ev = map.Advance(_rng);
            if (alive.Count <= 8 && phase != GamePhase.FinalShowdown)
            {
                phase = GamePhase.FinalShowdown;
                result.Log.Add("Final Showdown triggered: rare weapons and special contracts are live.");
                foreach (var p in alive.Where(p => p.ReputationLevel >= ReputationLevel.Feared))
                    p.Contracts.Add(new Contract(ContractType.Special, "Win by extracting Order data", 0, 1200, 250, ReputationLevel.Feared));
            }

            if (ev != RandomEventType.None) result.Log.Add($"Dynamic event: {ev} at minute {map.ElapsedMinutes}.");
            ResolveEncounter(alive, result, phase);
            alive = alive.Where(p => p.Alive).ToList();
        }

        result.Winner = alive.OrderByDescending(p => p.Health + p.Vehicle.Armor).First();
        GrantRewards(players, result.Winner, result);
        return result;
    }

    private void ResolveEncounter(List<Player> alive, MatchResult result, GamePhase phase)
    {
        if (alive.Count < 2) return;
        var attacker = alive[_rng.Next(alive.Count)];
        var target = alive.Where(p => p != attacker).OrderBy(_ => _rng.Next()).First();

        if (!attacker.Vehicle.Destroyed && !target.Vehicle.Destroyed)
        {
            var damage = attacker.Vehicle.Fire(_rng) + (phase == GamePhase.FinalShowdown ? 8 : 0);
            if (_rng.NextDouble() < 0.2) damage += attacker.Vehicle.UseNitro() / 15;
            target.Vehicle.TakeDamage(damage);
            attacker.AddXp(RankTrack.Driver, damage);
            result.Log.Add($"{attacker.Name} hit {target.Name}'s {target.Vehicle.Type} for {damage} vehicle damage.");
            if (target.Vehicle.Destroyed)
            {
                attacker.AddCoins(_rng.Next(30, 81));
                attacker.AddXp(RankTrack.Driver, 100);
                result.Log.Add($"{target.Name}'s vehicle exploded; ground combat begins for them.");
            }
            return;
        }

        var weapon = attacker.Inventory.OrderByDescending(w => w.Damage).FirstOrDefault() ?? new Weapon("Knife", GroundWeaponCategory.Melee, 25, 2, 1);
        var groundDamage = _rng.Next(Math.Max(1, weapon.Damage - 8), weapon.Damage + 9);
        if (target.Vehicle.Destroyed) groundDamage += 10;
        target.TakeGroundDamage(groundDamage);
        attacker.AddXp(RankTrack.Assassin, groundDamage);
        result.Log.Add($"{attacker.Name} used {weapon.Name} against {target.Name} for {groundDamage} ground damage.");
        if (!target.Alive)
        {
            result.Kills[attacker] = result.Kills.GetValueOrDefault(attacker) + 1;
            attacker.AddCoins(_rng.Next(10, 51));
            CompleteRelevantContracts(attacker, target, result);
        }
    }

    private static void CompleteRelevantContracts(Player attacker, Player target, MatchResult result)
    {
        foreach (var contract in attacker.Contracts.ToList())
        {
            var success = contract.Type switch
            {
                ContractType.Assassination or ContractType.CounterAssassination => contract.TargetPlayerId == target.Id,
                ContractType.Destruction => target.Vehicle.Destroyed,
                ContractType.Special => true,
                _ => false
            };
            if (!success) continue;
            attacker.AddCoins(contract.RewardCoins);
            attacker.AddReputation(contract.RewardReputation);
            attacker.AddXp(RankTrack.Reputation, contract.RewardReputation);
            attacker.Contracts.Remove(contract);
            result.Log.Add($"Contract completed by {attacker.Name}: {contract.Description}.");
        }
    }

    private static void GrantRewards(IEnumerable<Player> players, Player winner, MatchResult result)
    {
        winner.AddCoins(1000);
        winner.AddReputation(200);
        winner.AddXp(RankTrack.Reputation, 200);
        result.Log.Add($"Winner: {winner.Name}. Rewards paid: 1000 coins + 200 reputation.");
        foreach (var p in players.Where(p => p != winner && p.Alive).Take(2)) p.AddCoins(300);
    }
}

public sealed class BlackWarGame
{
    private readonly Random _rng;
    public Citadel Citadel { get; } = new();
    public List<Player> Players { get; } = new();

    public BlackWarGame(Random rng) => _rng = rng;

    public void RunDemoSeason(int matchCount)
    {
        Console.WriteLine("BLACK WAR — playable C# systems prototype");
        for (var i = 1; i <= matchCount; i++)
        {
            Console.WriteLine($"\n=== Citadel preparation for match {i} ===");
            foreach (var p in Players) Citadel.PreparePlayer(p, _rng);
            AssignContracts();
            var result = new MatchEngine(_rng).Run(Players.Select(CloneForMatch).ToList());
            foreach (var line in result.Log.Take(18)) Console.WriteLine(line);
            Console.WriteLine($"Match {i} complete. Winner: {result.Winner.Name}");
        }
    }

    private void AssignContracts()
    {
        foreach (var player in Players)
        {
            var target = Players.Where(p => p != player).OrderBy(_ => _rng.Next()).First();
            var contract = new Contract(ContractType.Assassination, $"Eliminate {target.Name}", 50, 250, 80, ReputationLevel.Unknown, target.Id);
            Citadel.AcceptContract(player, contract);
        }
    }

    private static Player CloneForMatch(Player p)
    {
        var vehicle = new Vehicle(p.Vehicle.Name, p.Vehicle.Type, p.Vehicle.Speed, p.Vehicle.MaxArmor, 80, p.Vehicle.Weapons);
        var clone = new Player(p.Name, p.GoldCoins, p.ReputationPoints, vehicle, p.Inventory, p.Id) { Alliance = p.Alliance };
        foreach (var contract in p.Contracts) clone.Contracts.Add(contract);
        return clone;
    }
}

public static class GameBootstrap
{
    public static BlackWarGame CreateDemoGame(int seed)
    {
        var rng = new Random(seed);
        var game = new BlackWarGame(rng);
        for (var i = 1; i <= 40; i++)
        {
            var vehicle = CreateVehicle((VehicleType)(i % 5));
            var weapon = new Weapon(i % 4 == 0 ? "Sniper" : "Assault Rifle", i % 4 == 0 ? GroundWeaponCategory.Sniper : GroundWeaponCategory.Smg, i % 4 == 0 ? 70 : 34, i % 4 == 0 ? 120 : 60, 90);
            game.Players.Add(new Player($"Merc-{i:00}", 800, rng.Next(0, 900), vehicle, new[] { weapon }));
        }
        var clan = new Alliance("Neon Vultures", AllianceType.Clan);
        foreach (var p in game.Players.Take(5)) clan.AddMember(p);
        return game;
    }

    private static Vehicle CreateVehicle(VehicleType type) => type switch
    {
        VehicleType.Scout => new Vehicle("Wraith Scout", type, 130, 120, 100, new[] { new VehicleWeapon(VehicleWeaponType.MachineGun, 24, 500, 1), new VehicleWeapon(VehicleWeaponType.SmokeBomb, 0, 3, 20) }),
        VehicleType.Assault => new Vehicle("Ravager", type, 95, 220, 70, new[] { new VehicleWeapon(VehicleWeaponType.MachineGun, 35, 600, 1), new VehicleWeapon(VehicleWeaponType.GuidedMissile, 85, 8, 8) }),
        VehicleType.Tank => new Vehicle("Iron Grave", type, 55, 420, 30, new[] { new VehicleWeapon(VehicleWeaponType.PlasmaCannon, 110, 4, 14), new VehicleWeapon(VehicleWeaponType.Mine, 60, 6, 5) }),
        VehicleType.Interceptor => new Vehicle("Night Fang", type, 120, 190, 90, new[] { new VehicleWeapon(VehicleWeaponType.GuidedMissile, 75, 10, 7), new VehicleWeapon(VehicleWeaponType.MachineGun, 28, 500, 1) }),
        _ => new Vehicle("Mercy Rig", type, 85, 210, 60, new[] { new VehicleWeapon(VehicleWeaponType.ElectromagneticShield, 0, 4, 25), new VehicleWeapon(VehicleWeaponType.MachineGun, 20, 400, 1) })
    };
}
