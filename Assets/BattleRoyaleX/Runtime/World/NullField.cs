using UnityEngine;

namespace BattleRoyaleX
{
    public sealed class NullField : MonoBehaviour
    {
        void Awake()
        {
            Collider c = GetComponent<Collider>();
            if (c != null)
            {
                c.enabled = true;
                c.isTrigger = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Hitbox hitbox = other.GetComponent<Hitbox>();
            if (hitbox != null && hitbox.Packet.nullifiable)
            {
                hitbox.Cancel();
                CombatEvents.Raise(new CombatEventData(CombatEventKind.Nullify, other.transform.position, null, hitbox.Owner));
            }
        }
    }
}
