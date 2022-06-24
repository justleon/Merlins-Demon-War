using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage = null;
    public Image mirrorImage = null;
    public Image healthNumberImage = null;
    public Image glowImage = null;

    public int maxHealth = 5;
    public int health = 5; //curent health
    public int mana = 1;

    public bool isPlayer;
    public bool isFire; //is a fire monster

    public GameObject[] manaBalls = new GameObject[5];

    private Animator animator = null;

    public AudioSource dealSound = null;
    public AudioSource healSound = null;
    public AudioSource mirrorSound = null;
    public AudioSource mirrorSmashSound = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateHealth();
        UpdateManaBalls();
    }

    internal void PlayHitAnim()
    {
        if(animator != null)
            animator.SetTrigger("Hit");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(!GameController.instance.isPlayable)
            return;
        
        GameObject obj = eventData.pointerDrag;
        if(obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if(card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playersHand);
            }
        }
    }

    public void UpdateHealth()
    {
        if(health >= 0 && health < GameController.instance.healthNumbers.Length)
        {
            healthNumberImage.sprite = GameController.instance.healthNumbers[health];
        }
        else
        {
            Debug.LogWarning("Health out of range!: " + health.ToString());
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }

    internal bool HasMirror()
    {
        return mirrorImage.gameObject.activeInHierarchy;
    }

    internal void UpdateManaBalls()
    {
        for(int i = 0; i < 5; i++)
        {
            if (mana > i)
                manaBalls[i].SetActive(true);
            else
                manaBalls[i].SetActive(false);
        }
    }

    internal void PlayMirrorSound()
    {
        mirrorSound.Play();
    }

    internal void PlaySmashSound()
    {
        mirrorSmashSound.Play();
    }

    internal void PlayHealSound()
    {
        healSound.Play();
    }

    internal void PlayCardSound()
    {
        dealSound.Play();
    }
}
