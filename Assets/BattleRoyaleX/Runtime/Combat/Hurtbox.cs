using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class Hurtbox : MonoBehaviour
    {
        public CharacterRuntime Owner { get; private set; }

        void Awake()
        {
            Owner = GetComponentInParent<CharacterRuntime>();
            Collider c = GetComponent<Collider>();
            if (c != null) c.isTrigger = true;
        }
    }
}
