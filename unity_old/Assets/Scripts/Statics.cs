public static class Statics
{
    public static float Gravity = -9.82f;
    public static float JumpForce = 5f;

    public static class Meta
    {
        public static int MenuScene = 0;
        public static int RunScene = 1;

        public static float MouseSensitivity = 50f;

        public const string LevelUp = "LevelUp";
        public const string Increase = "Increase";
        public const string GameOver = "GameOver";
        public const string TogglePause = "TogglePause";
        public const string RunSceneLoaded = "RunSceneLoaded";
        public const string MainMenuRequest = "MainMenuRequest";

        public const string AttackCooldown = "Attack Cooldown";
        public const string EnemyAttackCooldown = "Enemy Attack Cooldown";
        public const string MaxHealth = "Max Health";
        public const string PlayerSpeed = "Player Speed";
        public const string PushRadius = "Push Radius";
        public const string PushStrength = "Push Strength";
        public const string PushAngle = "Push Angle";
        public const string SpawnDelay = "Spawn Delay";
    }

    public static class Player
    {
        public static float Height = 0.5f;
        public static float Radius = 3f;

        public const string Attack = "PlayerAttack";
        public const string Hit = "PlayerHit";
        public const string Miss = "PlayerMiss";
        public const string Jump = "PlayerJump";
        public const string Heal = "PlayerHeal";
    }

    public static class Enemy
    {
        public static float Height = 0.5f;

        public const string Expire = "EnemyExpire";
        public const string Attack = "EnemyAttack";
    }
}
