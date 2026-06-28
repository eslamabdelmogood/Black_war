namespace BlackWar;

public enum GamePhase { Citadel, VehicleCombat, GroundCombat, FinalShowdown, Rewards }
public enum VehicleType { Scout, Assault, Tank, Interceptor, Support }
public enum VehicleWeaponType { MachineGun, GuidedMissile, Mine, SmokeBomb, ElectromagneticShield, PlasmaCannon }
public enum GroundWeaponCategory { Pistol, Smg, Shotgun, Sniper, Explosive, Melee }
public enum ContractType { Assassination, Destruction, Protection, Control, Special, CounterAssassination }
public enum RankTrack { Assassin, Driver, Reputation, Trader }
public enum ReputationLevel { Unknown, Known, Trusted, Feared, Legendary }
public enum AllianceType { Temporary, Clan, ProtectionContract }
public enum MapZone { IndustrialDesert, DestroyedCity, BridgesAndTunnels, DarkForest, MilitaryBase, Harbor }
public enum RandomEventType { Sandstorm, Explosion, AirDrop, None }

public sealed record Weapon(string Name, GroundWeaponCategory Category, int Damage, int Range, int Ammo, bool IsStolen = false, DateTime? StolenAt = null)
{
    public bool CanSell(DateTime now) => !IsStolen || (StolenAt.HasValue && now - StolenAt.Value >= TimeSpan.FromHours(24));
}

public sealed record VehicleWeapon(VehicleWeaponType Type, int Damage, int Ammo, int CooldownSeconds);

public sealed class Vehicle
{
    public string Name { get; }
    public VehicleType Type { get; }
    public int MaxArmor { get; }
    public int Armor { get; private set; }
    public int Speed { get; private set; }
    public int Nitro { get; private set; }
    public List<VehicleWeapon> Weapons { get; } = new();
    public bool Destroyed => Armor <= 0;

    public Vehicle(string name, VehicleType type, int speed, int armor, int nitro, IEnumerable<VehicleWeapon> weapons)
    {
        Name = name;
        Type = type;
        Speed = speed;
        MaxArmor = armor;
        Armor = armor;
        Nitro = nitro;
        Weapons.AddRange(weapons);
    }

    public int UseNitro()
    {
        if (Nitro <= 0) return Speed;
        Nitro -= 10;
        return (int)(Speed * 1.35);
    }

    public int Fire(Random rng)
    {
        var ready = Weapons.Where(w => w.Ammo != 0).ToList();
        if (ready.Count == 0) return 5;
        var weapon = ready[rng.Next(ready.Count)];
        return rng.Next(Math.Max(1, weapon.Damage - 10), weapon.Damage + 11);
    }

    public void TakeDamage(int amount) => Armor = Math.Max(0, Armor - amount);
    public void Repair(int amount) => Armor = Math.Min(MaxArmor, Armor + amount);
}

public sealed class Player
{
    public Guid Id { get; }
    public string Name { get; }
    public int GoldCoins { get; private set; }
    public int ReputationPoints { get; private set; }
    public int Health { get; private set; } = 100;
    public Vehicle Vehicle { get; set; }
    public List<Weapon> Inventory { get; } = new();
    public List<Contract> Contracts { get; } = new();
    public Dictionary<RankTrack, int> RankXp { get; } = Enum.GetValues<RankTrack>().ToDictionary(t => t, _ => 0);
    public bool Alive => Health > 0;
    public DateTime? TraitorUntil { get; private set; }
    public Alliance? Alliance { get; set; }

    public Player(string name, int coins, int reputation, Vehicle vehicle, IEnumerable<Weapon> weapons, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        GoldCoins = coins;
        ReputationPoints = reputation;
        Vehicle = vehicle;
        Inventory.AddRange(weapons);
    }

    public ReputationLevel ReputationLevel => ReputationPoints switch
    {
        >= 5000 => ReputationLevel.Legendary,
        >= 2500 => ReputationLevel.Feared,
        >= 1200 => ReputationLevel.Trusted,
        >= 400 => ReputationLevel.Known,
        _ => ReputationLevel.Unknown
    };

    public void AddCoins(int amount) => GoldCoins = Math.Max(0, GoldCoins + amount);
    public bool SpendCoins(int amount)
    {
        if (GoldCoins < amount) return false;
        GoldCoins -= amount;
        return true;
    }
    public void AddReputation(int amount) => ReputationPoints = Math.Max(0, ReputationPoints + amount);
    public void AddXp(RankTrack track, int amount) => RankXp[track] += amount;
    public void TakeGroundDamage(int amount) => Health = Math.Max(0, Health - amount);
    public void Heal(int amount) => Health = Math.Min(100, Health + amount);
    public void MarkTraitor(DateTime now) { TraitorUntil = now.AddHours(48); AddReputation(-350); }
}

public sealed record Contract(ContractType Type, string Description, int ActivationFee, int RewardCoins, int RewardReputation, ReputationLevel RequiredLevel, Guid? TargetPlayerId = null)
{
    public bool IsSpecial => Type is ContractType.Special;
};

public sealed class Alliance
{
    public string Name { get; }
    public AllianceType Type { get; }
    public List<Player> Members { get; } = new();
    public int Treasury { get; private set; }

    public Alliance(string name, AllianceType type) { Name = name; Type = type; }
    public void AddMember(Player player) { if (!Members.Contains(player)) { Members.Add(player); player.Alliance = this; } }
    public void Deposit(int coins) => Treasury += Math.Max(0, coins);
    public void Betray(Player traitor, DateTime now)
    {
        if (!Members.Contains(traitor)) return;
        traitor.MarkTraitor(now);
        Members.Remove(traitor);
        traitor.Alliance = null;
        traitor.AddCoins(150);
    }
}

public sealed record MarketListing(Guid Id, Player Seller, Weapon Item, int Price, DateTime ListedAt);

public sealed class BlackMarket
{
    private readonly List<MarketListing> _listings = new();
    public IReadOnlyList<MarketListing> Listings => _listings;
    public int SystemCommissionVault { get; private set; }

    public bool ListWeapon(Player seller, Weapon weapon, int price, DateTime now)
    {
        if (!seller.Inventory.Contains(weapon) || !weapon.CanSell(now) || price <= 0) return false;
        _listings.Add(new MarketListing(Guid.NewGuid(), seller, weapon, price, now));
        return true;
    }

    public bool Buy(Guid listingId, Player buyer)
    {
        var listing = _listings.FirstOrDefault(l => l.Id == listingId);
        if (listing is null || buyer.GoldCoins < listing.Price) return false;
        buyer.SpendCoins(listing.Price);
        var commission = (int)Math.Ceiling(listing.Price * 0.05);
        SystemCommissionVault += commission;
        listing.Seller.AddCoins(listing.Price - commission);
        listing.Seller.Inventory.Remove(listing.Item);
        buyer.Inventory.Add(listing.Item with { IsStolen = false, StolenAt = null });
        listing.Seller.AddXp(RankTrack.Trader, listing.Price);
        _listings.Remove(listing);
        return true;
    }
}
