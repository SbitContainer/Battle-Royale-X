using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class InteractionCooldownController : MonoBehaviour
    {
        readonly Dictionary<string, float> readyAt = new Dictionary<string, float>();

        public bool IsReady(string key)
        {
            if (string.IsNullOrEmpty(key)) return true;
            return !readyAt.TryGetValue(key, out float t) || Time.time >= t;
        }

        public bool TryConsume(string key, float cooldown)
        {
            if (!IsReady(key)) return false;
            if (!string.IsNullOrEmpty(key)) readyAt[key] = Time.time + Mathf.Max(0f, cooldown);
            return true;
        }

        public float Remaining(string key)
        {
            if (string.IsNullOrEmpty(key) || !readyAt.TryGetValue(key, out float t)) return 0f;
            return Mathf.Max(0f, t - Time.time);
        }
    }
}
