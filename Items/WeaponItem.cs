using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;

        [HideInInspector]
        public enum WeaponType
        {
            SpellCaster, FaithCaster, PyroCaster, MeleeWeapon
        }

        [Header("Damage")] 
        public int baseDamage = 25;
        public int criticalDamageMultiplier = 4;

        [Header("Weapon Stats")]
        public bool isUnarmed;

        [Header("Idle Animations")]
        public string Right_Hand_Idle;
        public string Left_Hand_Idle;
        public string TH_Idle;

        [Header("One Handed Attacks")]
        public string OH_Light_Attack_1;
        public string OH_Light_Attack_2;
        public string OH_Heavy_Attack_1;

        [Header("Two Handed Attacks")]
        public string TH_Light_Attack_1;
        public string TH_Light_Attack_2;
        public string TH_Light_Attack_3;

        public string TH_Heavy_Attack_1;

        [Header("Stamina Costs")]
        public int baseStamina = 0;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

        [Header("Weapon Type")]
        public WeaponType weaponType;

    }
}