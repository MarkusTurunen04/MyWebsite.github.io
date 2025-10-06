using System;
using System.Collections.Generic;

namespace IdleFrisbeeGolf.Game
{
    [Serializable]
    public class GameState
    {
        public double totalScore;
        public double currentScore;
        public double spiritTokens;
        public double gems;
        public Dictionary<string, int> upgradeLevels = new();
        public double manualThrows;
        public double autoThrowers;
        public double courseMultiplier = 1;
        public double accuracyMultiplier = 1f;
        public double powerMultiplier = 1f;
        public bool adsRemoved;
        public long lastSaveTimestamp;
        public long lastActiveTimestamp;
        public bool tutorialCompleted;
    }
}
