using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem
    {
        public int healAmount;

        public override void AttemptToCastSpell(PlayerAnimatorManager playerAnimatorManager, PlayerStats playerStats)
        {
            base.AttemptToCastSpell(playerAnimatorManager, playerStats);
            GameObject instantiatedWarmupSpellFx = Instantiate(spellWarmUpFx, playerAnimatorManager.transform);
            playerAnimatorManager.PLayTargetAnimation(spellAnimation, true);
            Destroy(instantiatedWarmupSpellFx, 2.5f);
        }

        public override void SucessfullyCastedSpell(PlayerAnimatorManager playerAnimatorManager, PlayerStats playerStats)
        {
            base.SucessfullyCastedSpell(playerAnimatorManager, playerStats);
            GameObject instantiatedSpellFx = Instantiate(spellCastFx, playerAnimatorManager.transform);
            playerStats.HealPlayer(healAmount);
            Destroy(instantiatedSpellFx, 3f);
        }
    }
}
