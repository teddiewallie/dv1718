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
    }

    public static class Player
    {
        public static float Height = 1.5f;
        public static float Radius = 2f;

        public const string Attack = "PlayerAttack";
        public const string Hit = "PlayerHit";
        public const string Miss = "PlayerMiss";
        public const string Jump = "PlayerJump";
    }

    public static class Enemy
    {
        public static float Height = 1.5f;

        public const string Expire = "EnemyExpire";
        public const string Attack = "EnemyAttack";
    }
}
