using UnityEngine;
using System.Collections.Generic;

public class PlayerLeveler : MonoBehaviour
{
    private ActionManager _actions;

    private const int XPModifier = 10;
    private const int MinAttackCooldown = 10;
    private const int MaxSpawn = 5;
    private const int MaxPlayerSpeed = 30;
    private const int MaxMaxHealth = 30;

    private const float XPScaler = 1.5f;
    private const float DifficultyModifier = 1.2f;
    private const float MaxPushRadius = 8f;
    private const float MaxPushAngle = 100f;
    private const float MinSpawnDelay = 1f;

    public int Level { get; private set; }
    public int XP { get; private set; }
    public int XPPerc { get; private set; }
    public int AttackCooldown { get; private set; }
    public int EnemyAttackCooldown { get; private set; }
    public int MaxHealth { get; private set; }

    public float PlayerSpeed { get; private set; }
    public float PushRadius { get; private set; }
    public float PushStrength { get; private set; }
    public float PushAngle { get; private set; }
    public float SpawnDelay { get; private set; }
    public float EnemySpeed { get; private set; }

    private static string[] s_allPerks = {
        Statics.Meta.AttackCooldown,
        Statics.Meta.MaxHealth,
        Statics.Meta.PlayerSpeed,
        Statics.Meta.PushRadius,
        Statics.Meta.PushStrength,
        Statics.Meta.PushAngle
    };

    public PlayerLeveler()
    {
        this.XP = 0;
        this.Level = this.CalculateLevel();
        this.AttackCooldown = 60;
        this.EnemyAttackCooldown = 60;
        this.PlayerSpeed = 10f;
        this.PushRadius = 3f;
        this.PushStrength = 8f;
        this.PushAngle = 45f;
        this.SpawnDelay = 3f;
        this.EnemySpeed = 4f;
        this.MaxHealth = 10;
    }

    public void Awake()
    {
        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);
    }

    private int CalculateLevel()
    {
        return (int)System.Math.Sqrt(this.XP / PlayerLeveler.XPScaler) + 1;
    }

    public float CalculateLevelProgress()
    {
        float currentLevel = CalculateLevel();

        float currentLevelXP = (currentLevel - 1) * (currentLevel - 1) * PlayerLeveler.XPScaler;
        float nextLevelXP = currentLevel * currentLevel * PlayerLeveler.XPScaler;

        float xpIntoLevel = this.XP - currentLevelXP;
        float xpNeededThisLevel = nextLevelXP - currentLevelXP;

        return xpIntoLevel / xpNeededThisLevel;
    }

    private void HandleActions(string message)
    {
        if (message.StartsWith(Statics.Meta.Increase))
        {
            string thing = message.Substring(message.IndexOf(" ") + 1);

            this.Increase(thing);
            this.Increase(Statics.Meta.SpawnDelay);
            this.Increase(Statics.Meta.EnemyAttackCooldown);
        }

        switch (message)
        {
            case Statics.Enemy.Expire: this.AddXP(); break;
            default: break;
        }
    }

    public int GetSpawnAmount()
    {
        int nextInt = new System.Random().Next(1, ((this.Level + 1) / 2) + 1);
        return nextInt > PlayerLeveler.MaxSpawn ? PlayerLeveler.MaxSpawn : nextInt;
    }

    private void Increase(string message)
    {
        float scaler = this.GetScaler();

        switch (message)
        {
            case Statics.Meta.AttackCooldown:
                this.AttackCooldown = this.AttackCooldown - 20 < PlayerLeveler.MinAttackCooldown
                    ? PlayerLeveler.MinAttackCooldown
                    : this.AttackCooldown - 20;
                this.EnemyAttackCooldown = this.EnemyAttackCooldown - 10 < PlayerLeveler.MinAttackCooldown + 10
                    ? PlayerLeveler.MinAttackCooldown + 10
                    : this.EnemyAttackCooldown - 10;
                break;
            case Statics.Meta.EnemyAttackCooldown:
                this.EnemyAttackCooldown = this.EnemyAttackCooldown - 1 < PlayerLeveler.MinAttackCooldown + 10
                    ? PlayerLeveler.MinAttackCooldown + 10
                    : this.EnemyAttackCooldown - 1;
                break;
            case Statics.Meta.MaxHealth:
                this.MaxHealth = this.MaxHealth + 10 > PlayerLeveler.MaxMaxHealth
                    ? PlayerLeveler.MaxMaxHealth
                    : this.MaxHealth + 10;
                    this._actions.Notify(Statics.Player.Heal);
                break;
            case Statics.Meta.PlayerSpeed:
                this.PlayerSpeed += 2f;
                this.EnemySpeed = this.PlayerSpeed * 0.7f;
                break;
            case Statics.Meta.PushRadius:
                this.PushRadius = this.PushRadius + scaler > PlayerLeveler.MaxPushRadius
                    ? PlayerLeveler.MaxPushRadius
                    : this.PushRadius + scaler;
                break;
            case Statics.Meta.PushStrength:
                this.PushStrength += 2;
                break;
            case Statics.Meta.PushAngle:
                this.PushAngle = this.PushAngle + 1f > PlayerLeveler.MaxPushAngle
                    ? PlayerLeveler.MaxPushAngle
                    : this.PushAngle + 1f;
                break;
            case Statics.Meta.SpawnDelay:
                this.SpawnDelay = this.SpawnDelay - (scaler / 4) < PlayerLeveler.MinSpawnDelay
                    ? PlayerLeveler.MinSpawnDelay
                    : this.SpawnDelay - (scaler / 4);
                break;
        }
    }

    private float GetScaler()
    {
        return (float)(this.Level / (PlayerLeveler.XPModifier));
    }

    public string[] GetThreePerks()
    {
        List<string> perks = new List<string>();

        foreach (string perk in PlayerLeveler.s_allPerks)
        {
            if (!IsMaxedOut(perk))
            {
                perks.Add(perk);
            }
        }

        string[] returner = new string[3];

        for (int i = 0; i < 3; i++)
        {
            int perkIndex = Random.Range(0, perks.Count);
            returner[i] = perks[perkIndex];
            perks.RemoveAt(perkIndex);
        }

        return returner;
    }

    private bool IsMaxedOut(string perk)
    {
        switch (perk)
        {
            case Statics.Meta.AttackCooldown:
                return this.AttackCooldown <= PlayerLeveler.MinAttackCooldown;
            case Statics.Meta.MaxHealth:
                return this.MaxHealth >= PlayerLeveler.MaxMaxHealth;
            case Statics.Meta.PlayerSpeed:
                return this.PlayerSpeed >= PlayerLeveler.MaxPlayerSpeed;
            case Statics.Meta.PushRadius:
                return this.PushRadius >= PlayerLeveler.MaxPushRadius;
            case Statics.Meta.PushAngle:
                return this.PushAngle >= PlayerLeveler.MaxPushAngle;
            default:
                return false;
        }
    }

    private void AddXP()
    {
        int oldLevel = this.Level;
        this.XP++;
        this.Level = this.CalculateLevel();

        if (oldLevel < this.Level)
        {
            this._actions.Notify(Statics.Meta.LevelUp);
        }
    }
}

