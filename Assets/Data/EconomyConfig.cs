using UnityEngine;

namespace IdleFrisbeeGolf.Data
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "IdleFrisbeeGolf/EconomyConfig")]
    public class EconomyConfig : ScriptableObject
    {
        [System.Serializable]
        public class UpgradeDefinition
        {
            public string id;
            public string displayName;
            public double baseCost = 10;
            public double growth = 1.1f;
            public double baseValue = 1;
            public double valueGrowth = 1.05f;
            public UpgradeType type;
        }

        public enum UpgradeType
        {
            Accuracy,
            Power,
            AutoThrow,
            CourseMultiplier
        }

        public UpgradeDefinition[] upgrades;
        public double autoThrowBaseInterval = 2f;
        public double autoThrowMinInterval = 0.25f;
        public double baseScorePerThrow = 10;
    }
}
