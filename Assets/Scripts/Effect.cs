using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player target = null;
    public Card cardUsed = null;
    public Image effectImage = null;

    public AudioSource iceSound = null;
    public AudioSource fireSound = null;
    public AudioSource destructSound = null;

    public void EndTrigger()
    {
        bool bounce = false;
        if(target.HasMirror())
        {
            bounce = true;
            target.SetMirror(false);
            target.PlaySmashSound();

            if(target.isPlayer)
            {
                GameController.instance.CastAttackEffect(cardUsed, GameController.instance.enemy);
            }
            else
            {
                GameController.instance.CastAttackEffect(cardUsed, GameController.instance.player);
            }
        }
        else
        {
            int damage = cardUsed.cardData.damage;
            if(!target.isPlayer)
            {
                if(cardUsed.cardData.damageType == CardData.DamageType.Fire && target.isFire)
                    damage /= 2;
                if(cardUsed.cardData.damageType == CardData.DamageType.Ice && !target.isFire)
                    damage /= 2;
            }
            target.health -= damage;
            target.PlayHitAnim();
            GameController.instance.UpdateHealths();
            if(target.health <= 0)
            {
                target.health = 0;
                if(target.isPlayer)
                    GameController.instance.PlayPlayerDieSound();
                else
                    GameController.instance.PlayEnemyDieSound();
            }

            if(!bounce)
                GameController.instance.NextPlayersTurn();
            GameController.instance.isPlayable = true;
        }

        Destroy(gameObject);
    }

    internal void PlayIceSound()
    {
        iceSound.Play();
    }

    internal void PlayFireballSound()
    {
        fireSound.Play();
    }

    internal void PlayDestructSound()
    {
        destructSound.Play();
    }
}
