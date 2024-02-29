using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class SpellItem : Item
    {
        [HideInInspector]
        public enum SpellType
        {
            FaithSpell,
            MagicSpell,
            PyroSpell
        }

        [Header("Spell Cost")]
        public int focusPointCost;


        [Header("Spell FX and Animation")]
        public GameObject spellWarmUpFx;
        public GameObject spellCastFx;
        public string spellAnimation;

        [Header("Spell Type")]
        public SpellType spellType;
        [TextArea]
        public string spellDescription;

        public virtual void AttemptToCastSpell(PlayerAnimatorManager playerAnimatorManager, PlayerStats playerStats)
        {
            Debug.Log("Attempting to cast a spell");
        }

        public virtual void SucessfullyCastedSpell(PlayerAnimatorManager playerAnimatorManager, PlayerStats playerStats)
        {
            Debug.Log("The spell has been casted!");
            playerStats.TakeFocusDamage(focusPointCost);
        }

    }
}
