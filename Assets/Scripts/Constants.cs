namespace Assets.Scripts
{
    public static class Constants {
        public class Axes
        {
            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
            public const string MouseX = "Mouse X";
            public const string MouseY = "Mouse Y";
            public const string MouseScrollWheel = "Mouse ScrollWheel";
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
            public const string Speed = "Speed";
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
    }
}
