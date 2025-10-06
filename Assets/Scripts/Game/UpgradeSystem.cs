using System;
using System.Collections.Generic;
using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Data;
using UnityEngine;

namespace IdleFrisbeeGolf.Game
{
    /// <summary>
    /// Controls upgrade purchasing logic and economy scaling.
    /// </summary>
    public class UpgradeSystem : MonoBehaviour
    {
        private GameState _state;
        private EconomyConfig _economyConfig;
        private Action<EconomyConfig.UpgradeDefinition, int> _onUpgradePurchased;
        private readonly Dictionary<string, EconomyConfig.UpgradeDefinition> _lookup = new();
        private IdleManager _idleManager;

        public void Initialize(GameState state, EconomyConfig config, Action<EconomyConfig.UpgradeDefinition, int> onUpgradePurchased)
        {
            _state = state;
            _economyConfig = config;
            _onUpgradePurchased = onUpgradePurchased;
            _lookup.Clear();
            foreach (var upgrade in _economyConfig.upgrades)
            {
                _lookup[upgrade.id] = upgrade;
                if (!_state.upgradeLevels.ContainsKey(upgrade.id))
                {
                    _state.upgradeLevels[upgrade.id] = 0;
                }
            }
            _idleManager = FindObjectOfType<IdleManager>();
        }

        public double GetUpgradeCost(string id)
        {
            if (!_lookup.TryGetValue(id, out var upgrade))
            {
                throw new ArgumentException($"Unknown upgrade {id}");
            }

            var level = _state.upgradeLevels[id];
            return upgrade.baseCost * Math.Pow(upgrade.growth, level);
        }

        public bool Purchase(string id)
        {
            if (!_lookup.TryGetValue(id, out var upgrade))
            {
                return false;
            }

            var cost = GetUpgradeCost(id);
            if (!_idleManager.CanAfford(cost))
            {
                return false;
            }

            _idleManager.SpendScore(cost);
            _state.upgradeLevels[id]++;
            _onUpgradePurchased?.Invoke(upgrade, _state.upgradeLevels[id]);
            EventBus.Publish(new UpgradePurchasedMessage(id, _state.upgradeLevels[id]));
            return true;
        }

        public int GetLevel(string id)
        {
            return _state.upgradeLevels.TryGetValue(id, out var level) ? level : 0;
        }

        public void ResetUpgrades()
        {
            foreach (var key in new List<string>(_state.upgradeLevels.Keys))
            {
                _state.upgradeLevels[key] = 0;
            }
        }
    }

    public readonly struct UpgradePurchasedMessage
    {
        public readonly string Id;
        public readonly int Level;

        public UpgradePurchasedMessage(string id, int level)
        {
            Id = id;
            Level = level;
        }
    }
}
