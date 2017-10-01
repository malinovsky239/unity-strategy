using UnityEngine;

namespace Assets.Scripts
{
    public static class Constants
    {
        public enum Formations
        {
            Column = 1,
            Row = 2,
            Wedge = 3
        }

        public class Axes
        {
            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
            public const string MouseX = "Mouse X";
            public const string MouseY = "Mouse Y";
            public const string MouseScrollWheel = "Mouse ScrollWheel";
        }

        public class Mouse
        {
            public const int LeftButton = 0;
            public const int RightButton = 1;
            public const int ScrollWheel = 2;
        }

        public class Tags
        {
            public const string Player = "player";
            public const string Enemy = "enemy";
        }

        public class Resources
        {
            public const string Sword = "Sword";
        }

        public class AnimatorParameters
        {
            public const string SpeedParam = "Speed";
            public const string EnemyWithinAttackingRange = "EnemyIsWithinAttackingRange";
            public const string EnemyWithinFieldOfView = "EnemyIsWithinFieldOfView";
            public const string Alive = "Alive";
            public const string CastingMagic = "CastingMagic";
        }

        public class Messages
        {
            public const string SwitchRotationWheelState = "SwitchRotationWheelState";
            public const string FormationChanged = "FormationChanged";
            public const string CntChanged = "CntChanged";
        }

        public class Intervals
        {
            public const float FromDeathToDisappearing = 10;
            public const float FireballCastingPreparations = 3;
            public const float FireballRecharging = 5;
            public const float DefaultPauseBeforeAttack = 1;
        }

        public class Colors
        {
            public static Color SelectionRect = new Color(0.8f, 0.8f, 0.95f, 0.25f);
            public static Color SelectionRectBorder = new Color(0.8f, 0.8f, 0.95f);
            public static Color HealthPointsBarMainColor = new Color(1, 0, 0, 0.5f);
            public static Color HealthPointsBarBorderColor = new Color(0, 0, 0, 0.5f);
        }

        public class Speed
        {
            public const int Run = 10;
            public const int Walk = 2;
            public const int Stand = 0;
        }

        public const float LargeEps = 1.0f;
        public const float SmallEps = 0.001f;
    }
}
