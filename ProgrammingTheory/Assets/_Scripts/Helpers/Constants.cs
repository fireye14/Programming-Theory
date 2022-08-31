using UnityEngine;

namespace Assets._Scripts.Helpers
{
    public static class Constants
    {
        public static class Paths
        {
            //public static readonly string settings = Application.streamingAssetsPath + "/Settings.json";
            //public static readonly string playerSettings = Application.streamingAssetsPath + "/Player.json";
        }

        public static class Tags
        {
            public const string Player = nameof(Player);
            public const string DeathZone = nameof(DeathZone);
            public const string Weapon = nameof(Weapon);
            public const string Enemy = nameof(Enemy);
        }
        public static class Layers
        {
            public static readonly int Player = LayerMask.NameToLayer(nameof(Player));
            public static readonly int Enemy = LayerMask.NameToLayer(nameof(Enemy));
            public static readonly int Invincible = LayerMask.NameToLayer(nameof(Invincible));
        }
    }
}