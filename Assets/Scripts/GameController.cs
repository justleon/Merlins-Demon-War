using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playersHand = new Hand();
    public Hand enemyHand = new Hand();

    public Player player = null;
    public Player enemy = null;

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public GameObject cardPrefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    public GameObject effectRight = null;
    public GameObject effectLeft = null;

    public Sprite fireballImage = null;
    public Sprite iceballImage = null;
    public Sprite multiFireballImage = null;
    public Sprite multiIceballImage = null;
    public Sprite fireAndIceballImage = null;
    public Sprite destructBallImage = null;

    public bool playersTurn = true;

    public TMP_Text turnText = null;
    public TMP_Text scoreText = null;

    public int playerScore = 0;
    public int playerKills = 0;

    public Image enemySkipTurnImage = null;

    public Sprite fireEnemy = null;
    public Sprite iceEnemy = null;

    public AudioSource playerDieSound = null;
    public AudioSource enemyDieSound = null;

    private void Awake()
    {
        instance = this;

        SetUpEnemy();

        playerDeck.Create();
        enemyDeck.Create();

        StartCoroutine(DealHands());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        if(playersTurn && isPlayable)
            NextPlayersTurn();
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < 3; i++)
        {
            playerDeck.DealCard(playersHand);
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player whichPlayer, Hand fromHand)
    {
        if(!CardValid(card, whichPlayer, fromHand))
            return false;
        isPlayable = false;
        CastCard(card, whichPlayer, fromHand);

        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);

        fromHand.RemoveCard(card);

        return false;
    }

    internal bool CardValid(Card cardPlayed, Player whichPlayer, Hand fromHand)
    {
        bool valid = false;
        if(cardPlayed == null)
            return false;
        
        if(fromHand.playerHand)
        {
            if(cardPlayed.cardData.cost <= player.mana)
            {
                if(whichPlayer.isPlayer && cardPlayed.cardData.isDefenceCard)
                    valid = true;
                
                if(!whichPlayer.isPlayer && !cardPlayed.cardData.isDefenceCard)
                    valid = true;
            }
        }
        else
        {
            if(cardPlayed.cardData.cost <= enemy.mana)
            {
                if(!whichPlayer.isPlayer && cardPlayed.cardData.isDefenceCard)
                    valid = true;
                
                if(whichPlayer.isPlayer && !cardPlayed.cardData.isDefenceCard)
                    valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player whichPlayer, Hand fromHand)
    {
        if(card.cardData.isMirrorCard)
        {
            whichPlayer.PlayMirrorSound();
            whichPlayer.SetMirror(true);
            NextPlayersTurn();
            isPlayable = true;
        }
        else 
        {
            if(card.cardData.isDefenceCard)
            {
                whichPlayer.health += card.cardData.damage;
                whichPlayer.PlayHealSound();
                if(whichPlayer.health > whichPlayer.maxHealth)
                    whichPlayer.health = whichPlayer.maxHealth;
                
                UpdateHealths();

                StartCoroutine(CastHealEffect(whichPlayer));
            }
            else
            {
                CastAttackEffect(card, whichPlayer);
            }

            if(fromHand.playerHand)
                playerScore += card.cardData.damage;

            UpdateScore();
        }

        if(fromHand.playerHand)
        {
            GameController.instance.player.mana -= card.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else
        {
            GameController.instance.enemy.mana -= card.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Player whichPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        NextPlayersTurn();
        isPlayable = true;
    }

    internal void CastAttackEffect(Card card, Player whichPlayer)
    {
        GameObject effectGO = null;
        if(whichPlayer.isPlayer)
            effectGO = Instantiate(effectRight, canvas.gameObject.transform);
        else
            effectGO = Instantiate(effectLeft, canvas.gameObject.transform);

        Effect effect = effectGO.GetComponent<Effect>();
        if(effect != null)
        {
            effect.target = whichPlayer;
            effect.cardUsed = card;
            switch(card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    if(card.cardData.isMulti)
                        effect.effectImage.sprite = multiFireballImage;
                    else
                        effect.effectImage.sprite = fireballImage;
                    effect.PlayFireballSound();
                break;
                case CardData.DamageType.Ice:
                    if(card.cardData.isMulti)
                        effect.effectImage.sprite = multiIceballImage;
                    else
                        effect.effectImage.sprite = iceballImage;
                    effect.PlayIceSound();
                break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceballImage;
                    effect.PlayFireballSound();
                    effect.PlayIceSound();
                break;
                case CardData.DamageType.Destruct:
                    effect.effectImage.sprite = destructBallImage;
                    effect.PlayDestructSound();
                break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if(player.health <= 0)
        {
            StartCoroutine(GameOver());
        }

        if(enemy.health <= 0)
        {
            playerKills++;
            playerScore += 100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemyHand.ClearHand();
        yield return new WaitForSeconds(1);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = UnityEngine.Random.Range(0, 2) == 1 ? false : true;
        if(enemy.isFire)
            enemy.playerImage.sprite = fireEnemy;
        else
            enemy.playerImage.sprite = iceEnemy;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    internal void NextPlayersTurn()
    {
        playersTurn = !playersTurn;
        bool isEnemyDead = false;
        if(playersTurn)
        {
            if(player.mana < 5)
                player.mana++;
        }
        else
        {
            if(enemy.health > 0)
            {
                if (enemy.mana < 5)
                    enemy.mana++;
            }
            else
                isEnemyDead = true;
        }

        if(isEnemyDead)
        {
            playersTurn = !playersTurn;
            if(player.mana < 5)
                player.mana++;
        }
        else
        {
            SetTurnText();
            if(!playersTurn)
                MonstersTurn();
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    internal void SetTurnText()
    {
        if(playersTurn)
            turnText.text = "Merlin's Turn";
        else
            turnText.text = "Enemy's Turn";
    }

    private void MonstersTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(MonsterCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> available = new List<Card>();

        for(int i = 0; i < 3; i++)
        {
            if(CardValid(enemyHand.cards[i], enemy, enemyHand))
                available.Add(enemyHand.cards[i]);
            else if(CardValid(enemyHand.cards[i], player, enemyHand))
                available.Add(enemyHand.cards[i]);
        }

        if(available.Count == 0)
        {
            NextPlayersTurn();
            return null;
        }

        int choice = UnityEngine.Random.Range(0, available.Count);
        return available[choice];
    }

    private IEnumerator MonsterCastCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);

        if(card)
        {
            TurnCard(card);
            yield return new WaitForSeconds(1.5f);

            if(card.cardData.isDefenceCard)
                UseCard(card, enemy, enemyHand);
            else
                UseCard(card, player, enemyHand);

            yield return new WaitForSeconds(1f);

            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1f);
        }
        else
        {
            enemySkipTurnImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            enemySkipTurnImage.gameObject.SetActive(false);
        }
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if(animator)
        {
            animator.SetTrigger("Flip");
        }
        else
            Debug.Log("No animator found");
    }

    private void UpdateScore()
    {
        scoreText.text = "Demons killed: " + playerKills.ToString() + ". Score: " + playerScore.ToString();
    }

    
    internal void PlayPlayerDieSound()
    {
        playerDieSound.Play();
    }

    
    internal void PlayEnemyDieSound()
    {
        enemyDieSound.Play();
    }
}
