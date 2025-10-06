using UnityEngine;

namespace IdleFrisbeeGolf.Data
{
    [CreateAssetMenu(fileName = "PrestigeConfig", menuName = "IdleFrisbeeGolf/PrestigeConfig")]
    public class PrestigeConfig : ScriptableObject
    {
        public double scoreDivider = 1e6;
        public double exponent = 0.5f;
        public double spiritBonusPerToken = 0.02f;
    }
}
