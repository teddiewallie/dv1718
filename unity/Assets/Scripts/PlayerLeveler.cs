using UnityEngine;

public class PlayerLeveler : MonoBehaviour
{
    private ActionManager _actions;

    private const int XPModifier = 10;
    private const float XPScaler = 1.5f;
    private const float DifficultyModifier = 1.2f;
    private const int MinAttackCooldown = 20;
    private const int MaxSpawn = 5;

    public int Level { get; private set; }
    public int XP { get; private set; }
    public int XPPerc { get; private set; }
    public int AttackCooldown { get; private set; }
    public int MaxHealth { get; private set; }

    public float Speed { get; private set; }

    public float PushRadius { get; private set; }
    public float PushStrength { get; private set; }
    public float PushAngle { get; private set; }
    public float SpawnDelay { get; private set; }
    public float EnemySpeed { get; private set; }

    public PlayerLeveler()
    {
        this.XP = 0;
        this.Level = this.CalculateLevel();
        this.AttackCooldown = 60;
        this.Speed = 6f;
        this.PushRadius = 2f;
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
        return (int)(this.XP / (PlayerLeveler.XPModifier * PlayerLeveler.XPScaler * PlayerLeveler.DifficultyModifier)) + 1;
    }

    private void HandleActions(string message)
    {
        if (message.StartsWith(Statics.Meta.Increase))
        {
            this.Increase(message);
        }

        switch (message)
        {
            case Statics.Enemy.Expire: AddXP(); break;
            case Statics.Meta.LevelUp: LevelUp(); break;
            default: break;
        }
    }

    public int GetSpawnAmount()
    {
        int nextInt = new System.Random().Next(1, (this.Level + 1) / 2) + 1;
        return nextInt > PlayerLeveler.MaxSpawn ? PlayerLeveler.MaxSpawn : nextInt;
    }

    private void LevelUp()
    {
        float scaler = (this.Level / (PlayerLeveler.XPModifier) / 1.5f);

        this.SpawnDelay -= this.SpawnDelay <= 1f ? 0f : scaler;
        this.AttackCooldown -= this.AttackCooldown <= PlayerLeveler.MinAttackCooldown ? 0 : 5;
        // this.MaxHealth += (int)scaler * 2;
        this.EnemySpeed += scaler;
        this.PushRadius += scaler;
        this.PushAngle += 1f;
        this.PushStrength += 2;
        this.Speed += 1f;
    }

    private void Increase(string message)
    {
        string[] parts = message.Split(' ');

        switch (parts[1])
        {
            case "AttackCooldown": break;
            case "MaxHealth": break;
            case "Speed": break;
            case "PushRadius": break;
            case "PushStrength": break;
            case "PushAngle": break;
            case "SpawnDelay": break;
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

