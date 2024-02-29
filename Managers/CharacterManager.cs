using UnityEngine;

namespace AVE
{
    public class CharacterManager : MonoBehaviour
    {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Combat Colliders")]
        public BoxCollider backStabBoxCollider;
        public BackStabCollider backStabCollider;

        // Used in back-stab and riposte animations
        public int pendingCriticalDamage;

    }
}