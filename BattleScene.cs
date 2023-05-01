using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Globals; //global variable storage

public class BattleScene : MonoBehaviour {
    //variables for actual battle
    public List<GameObject> deckDraw; //deck from which you draw
    public List<GameObject> hand; //cards that you hold currently
    public List<GameObject> deckDis; //deck where you discard

    //same but for enemy
    public List<GameObject> enemyDraw; //deck from which enemy draws
    public List<GameObject> enemyHand; //cards that enemy holds currently
    public List<GameObject> enemyDis; //deck where enemy discards

    //referances
    public GameObject CardReference; //blank card
    public GameObject abilityButton; //just to work with its particle effect
    public Text textfieldturn, textfieldend, manatextfield;
    public AudioSource music;
    public GameObject blackVeil;

    public bool playersturn; //does player has first turn?
    public bool miracleOpportunity = true;
    public bool abilityOpportunity = false; //lets player do 1 ability per turn (some don't consume ability opportunity)
    public bool action = false; //sets a flag so that player cannot press multiple buttons simultaniously
    public bool endOfBattle = false; //needed for cards to stop shuffling after battle ends

    //constant card coordinats for ease of change
    private float cardPosX = 7.5f;
    private float cardPosY = 2.5f;

    void Start() {
        music.volume = volume;
        music.Play();
        for (int i = 0; i < clashingHeroes[0].IDlist.Count; i++) {
            deckDraw.Add(Instantiate(CardReference, transform.position, transform.rotation));
            deckDraw[i].GetComponent<CardObj>().Set(clashingHeroes[0].IDlist[i], clashingHeroes[0].HPlist[i], clashingHeroes[0].ATKlist[i], true, this);
        }
        for (int i = 0; i < clashingHeroes[1].IDlist.Count; i++) {
            enemyDraw.Add(Instantiate(CardReference, transform.position, transform.rotation));
            enemyDraw[i].GetComponent<CardObj>().Set(clashingHeroes[1].IDlist[i], clashingHeroes[1].HPlist[i], clashingHeroes[1].ATKlist[i], false, this);
        }
        deckDraw = Reshuffle(deckDraw);
        enemyDraw = Reshuffle(enemyDraw);
        for (int i = 0; i < deckDraw.Count; i++) {
            deckDraw[i].transform.position = new Vector3(-cardPosX, -cardPosY, 0.1f * i);
        }
        for (int i = 0; i < enemyDraw.Count; i++) {
            enemyDraw[i].transform.position = new Vector3(-cardPosX, cardPosY, 0.1f * i);
        }
        StartCoroutine(Draw());
    }

    void Update() { }

    public List<GameObject> Reshuffle(List<GameObject> deck) //randomly shuffles set of card
    {
        int coun = deck.Count;
        List<GameObject> newdeck = new List<GameObject>();
        for (int i = 0; i < coun; i++) {
            int rand = Random.Range(0, deck.Count);
            newdeck.Add(deck[rand]);
            deck.RemoveAt(rand);
        }
        return newdeck;
    }

    public IEnumerator ReturnDeck() //returns cards from discard deck to draw deck
    {
        int coun = deckDis.Count;
        deckDis = Reshuffle(deckDis);
        for (int i = 0; i < coun; i++) {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(deckDis[0].GetComponent<CardObj>().Movement(new Vector3(-cardPosX, -cardPosY, 0.1f * i)));
            StartCoroutine(deckDis[0].GetComponent<CardObj>().Rotation(Quaternion.Euler(0, 0, 0)));
            deckDraw.Add(deckDis[0]);
            deckDis.RemoveAt(0);
        }
    }

    public IEnumerator ReturnEnemyDeck() //returns cards from discard deck to draw deck
    {
        int coun = enemyDis.Count;
        enemyDis = Reshuffle(enemyDis);
        for (int i = 0; i < coun; i++) {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(enemyDis[0].GetComponent<CardObj>().Movement(new Vector3(-cardPosX, cardPosY, 0.1f * i)));
            StartCoroutine(enemyDis[0].GetComponent<CardObj>().Rotation(Quaternion.Euler(0, 0, 0)));
            enemyDraw.Add(enemyDis[0]);
            enemyDis.RemoveAt(0);
        }
    }

    public IEnumerator Draw() //start of the turn, both draw 3 cards, decide whose first action
    {
        int playerspd = 0, enemyspd = 0;
        UpdateAbility(true);
        miracleOpportunity = true;
        //player's hand
        if (deckDraw.Count < 3) {
            StartCoroutine(ReturnDeck());
            yield return new WaitForSeconds((deckDis.Count + 2) * 0.2f);
        }
        //enemy's hand
        if (enemyDraw.Count < 3) {
            StartCoroutine(ReturnEnemyDeck());
            yield return new WaitForSeconds((enemyDis.Count + 2) * 0.2f);
        }
        //drawing
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.5f);
            if (deckDraw.Count == 0) {
                break;
            }
            if (i == 1) {
                StartCoroutine(deckDraw[0].GetComponent<CardObj>().Movement(new Vector3(1.7f * (i - 1), -cardPosY, 0.1f * i)));
            } else {
                StartCoroutine(deckDraw[0].GetComponent<CardObj>().Movement(new Vector3(1.7f * (i - 1), -cardPosY - 0.2f, 0.1f * i)));
            }
            StartCoroutine(deckDraw[0].GetComponent<CardObj>().Rotation(Quaternion.Euler(0, 0, (1 - i) * 10)));
            deckDraw[0].GetComponent<CardObj>().drawn = true;
            deckDraw[0].GetComponent<CardObj>().angle = Quaternion.Euler(0, 0, (1 - i) * 10);
            playerspd = playerspd + deckDraw[0].GetComponent<CardObj>().speed;
            hand.Add(deckDraw[0]);
            deckDraw.RemoveAt(0);
        }
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.5f);
            if (enemyDraw.Count == 0) {
                break;
            }
            StartCoroutine(enemyDraw[0].GetComponent<CardObj>().Movement(new Vector3(2.75f * (i - 1), cardPosY, 0)));
            enemyDraw[0].GetComponent<CardObj>().drawn = true;
            enemyDraw[0].GetComponent<CardObj>().angle = new Quaternion(0, 0, 0, 0);
            enemyspd = enemyspd + enemyDraw[0].GetComponent<CardObj>().speed;
            enemyHand.Add(enemyDraw[0]);
            enemyDraw.RemoveAt(0);
        }
        if (playerspd > enemyspd || (playerspd == enemyspd && Random.Range(0, 2) == 1)) {
            playersturn = true;
            textfieldturn.text = "Your turn";
            action = false;
        } else {
            playersturn = false;
            textfieldturn.text = "Enemy's turn";
            StartCoroutine(AIAttack());
        }
    }

    public IEnumerator End() //discard cards from hand into discard pile
    {
        if (!endOfBattle) {
            yield return new WaitForSeconds(1.1f);
            int count = hand.Count;
            DisableChoise(true);
            DisableChoise(false);
            for (int i = 0; i < count; i++) {
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(hand[0].GetComponent<CardObj>().Movement(new Vector3(-cardPosX + 3, -cardPosY, -0.1f * i)));
                StartCoroutine(hand[0].GetComponent<CardObj>().Resize(new Vector3(2.5f, 3.4f, 0)));
                hand[0].GetComponent<CardObj>().drawn = false;
                hand[0].GetComponent<CardObj>().clicked = false;
                deckDis.Add(hand[0]);
                hand.RemoveAt(0);
            }
            count = enemyHand.Count;
            for (int i = 0; i < count; i++) {
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(enemyHand[0].GetComponent<CardObj>().Movement(new Vector3(-cardPosX + 3, cardPosY, -0.1f * i)));
                StartCoroutine(enemyHand[0].GetComponent<CardObj>().Resize(new Vector3(2.5f, 3.4f, 0)));
                enemyHand[0].transform.Rotate(0, 0, (1 - i) * 10f);
                enemyHand[0].GetComponent<CardObj>().drawn = false;
                enemyHand[0].GetComponent<CardObj>().clicked = false;
                enemyDis.Add(enemyHand[0]);
                enemyHand.RemoveAt(0);
            }
            StartCoroutine(Draw());
        }
    }

    public IEnumerator Attack() {
        int unit = -1;
        int target = -1;
        for (int i = 0; i != hand.Count; i++) {
            if (hand[i].GetComponent<CardObj>().clicked) {
                unit = i;
                break;
            }
        }
        if (unit == -1) {
            action = false;
            yield break;
        }
        for (int i = 0; i != enemyHand.Count; i++) {
            if (enemyHand[i].GetComponent<CardObj>().clicked) {
                target = i;
                break;
            }
        }
        if (target == -1) {
            action = false;
            yield break;
        }
        yield return new WaitForSeconds(0.3f);
        hand[unit].GetComponent<CardObj>().DoDamage(enemyHand[target]);
        yield return new WaitForSeconds(2);
        if (playersturn) {
            StartCoroutine(AIAttack());
        } else {
            StartCoroutine(End());
        }
    }

    public IEnumerator AIAttack() //deciding the best move
    {
        if (enemyHand.Count == 0) {
            yield break;
        }
        int maxdmg = 0;
        for (int i = 0; i < enemyHand.Count; i++) {
            //finds the most damaging card
            if (enemyHand[i].GetComponent<CardObj>().atk > enemyHand[maxdmg].GetComponent<CardObj>().atk) {
                maxdmg = i;
            }
        }
        int target = 0;
        for (int i = 0; i < hand.Count; i++) {
            //basicaly tries to destroy a card in a single hit, while also trying to deal the highest damage
            if (hand[i].GetComponent<CardObj>().hp <= enemyHand[maxdmg].GetComponent<CardObj>().atk && hand[i].GetComponent<CardObj>().hp > hand[target].GetComponent<CardObj>().hp) {
                target = i;
            }
        }
        yield return new WaitForSeconds(0.5f);
        enemyHand[maxdmg].GetComponent<CardObj>().DoDamage(hand[target]);
        yield return new WaitForSeconds(1.5f);
        if (playersturn) {
            StartCoroutine(End());
        } else {
            action = false;
        }
    }

    public IEnumerator CheckHP(GameObject card, bool isPlayers) //checks if card is alife or does the routine to remove it
    {
        yield return new WaitForSeconds(0.5f);
        card.GetComponent<SpriteRenderer>().material.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);
        card.SetActive(false);
        if (isPlayers) {
            hand.Remove(card);
        } else {
            enemyHand.Remove(card);
        }
        StartCoroutine(CheckVictory());
    }

    public IEnumerator CheckVictory() {
        bool playerDead = false, enemyDead = false;
        if (deckDraw.Count == 0 && hand.Count == 0 && deckDis.Count == 0) {
            clashingHeroes[0].IDlist.Clear(); //marks that this hero is dead
            playerDead = true;
            endOfBattle = true;
        }
        if (enemyDraw.Count == 0 && enemyHand.Count == 0 && enemyDis.Count == 0) {
            clashingHeroes[1].IDlist.Clear(); //marks that this hero is dead
            enemyDead = true;
            endOfBattle = true;
        }
        if (playerDead && enemyDead) {
            textfieldend.text = "Draw";
            yield return new WaitForSeconds(3);
            StartCoroutine(FadeOut());
            yield return new WaitForSeconds(2);
            ChangeScene("MapScene");
        } else if (playerDead) {
            //saves card changes
            clashingHeroes[1].IDlist.Clear();
            clashingHeroes[1].HPlist.Clear();
            clashingHeroes[1].ATKlist.Clear();
            List<GameObject> allcards = new List<GameObject>();
            allcards.AddRange(enemyDraw);
            allcards.AddRange(enemyHand);
            allcards.AddRange(enemyDis);
            for (int i = 0; i < allcards.Count; i++) {
                clashingHeroes[1].IDlist.Add(allcards[i].GetComponent<CardObj>().id);
                clashingHeroes[1].HPlist.Add(allcards[i].GetComponent<CardObj>().hp);
                clashingHeroes[1].ATKlist.Add(allcards[i].GetComponent<CardObj>().maxAtk);
            }
            textfieldend.text = "You've lost";
            yield return new WaitForSeconds(3);
            StartCoroutine(FadeOut());
            yield return new WaitForSeconds(2);
            ChangeScene("MapScene");
        } else if (enemyDead) {
            clashingHeroes[0].IDlist.Clear();
            clashingHeroes[0].HPlist.Clear();
            clashingHeroes[0].ATKlist.Clear();
            List<GameObject> allcards = new List<GameObject>();
            allcards.AddRange(deckDraw);
            allcards.AddRange(hand);
            allcards.AddRange(deckDis);
            for (int i = 0; i < allcards.Count; i++) {
                clashingHeroes[0].IDlist.Add(allcards[i].GetComponent<CardObj>().id);
                clashingHeroes[0].HPlist.Add(allcards[i].GetComponent<CardObj>().hp);
                clashingHeroes[0].ATKlist.Add(allcards[i].GetComponent<CardObj>().maxAtk);
            }
            textfieldend.text = "You've won";
            yield return new WaitForSeconds(3);
            StartCoroutine(FadeOut());
            yield return new WaitForSeconds(2);
            ChangeScene("MapScene");
        }
    }

    public IEnumerator FadeOut() { //fades out the screen
        blackVeil.SetActive(true);
        for (int i = 0; i < 52; i++) {
            yield return 0;
            blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, i * 5 / 255f);
        }
        blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
    }

    public void DisableChoise(bool playershand) //to declick previous clicked card before clicking on a new one
    {
        if (playershand) {
            for (int i = 0; i < hand.Count; i++) {
                hand[i].GetComponent<CardObj>().clicked = false;
                hand[i].GetComponent<ParticleSystem>().Stop();
            }
        } else {
            for (int i = 0; i < enemyHand.Count; i++) {
                enemyHand[i].GetComponent<CardObj>().clicked = false;
                enemyHand[i].GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    public void UpdateAbility(bool start) //updates the ability button
    {
        if (start && abilityOpportunity) //start of the round with not spent ability does nothing 
        {
            return;
        }
        if (abilityOpportunity) {
            abilityOpportunity = false;
            abilityButton.GetComponent<ParticleSystem>().Stop();
        } else {
            abilityOpportunity = true;
            abilityButton.GetComponent<ParticleSystem>().Play();
        }
    }

    public IEnumerator Ability() {
        int card = -1;
        for (int i = 0; i < hand.Count; i++) {
            if (hand[i].GetComponent<CardObj>().clicked) {
                card = i;
            }
        }
        if (card == -1) {
            yield return 0;
            action = false;
            yield break;
        }
        switch (hand[card].GetComponent<CardObj>().id) {
            case 1:
                StartCoroutine(hand[card].GetComponent<CardObj>().RecieveDamage(5));
                for (int i = 0; i < hand.Count; i++) {
                    if (i != card && !hand[i].GetComponent<CardObj>().corrupt) //not affecting itself and corrupted cards
                    {
                        hand[i].GetComponent<CardObj>().atk = hand[i].GetComponent<CardObj>().atk + 5;
                        hand[i].GetComponent<CardObj>().UpdateStats();
                    }
                }
                break;
            case 6:
                if (abilityOpportunity) {
                    UpdateAbility(false);
                    int atk = 0;
                    int hp = 0;
                    for (int i = 0; i < hand.Count; i++) {
                        atk = atk + hand[i].GetComponent<CardObj>().atk;
                        hp = hp + hand[i].GetComponent<CardObj>().hp;
                        hand[i].GetComponent<CardObj>().hp = 0;
                        StartCoroutine(CheckHP(hand[i], true));
                    }
                    yield return new WaitForSeconds(5);
                    hand.Add(Instantiate(CardReference, new Vector3(0, 0, -0.2f), transform.rotation));
                    hand[0].transform.localScale = hand[0].transform.localScale * 1.2f;
                    hand[0].GetComponent<CardObj>().scene = this;
                    hand[0].GetComponent<CardObj>().maxAtk = atk;
                    hand[0].GetComponent<CardObj>().maxHp = hp;
                    hand[0].GetComponent<CardObj>().Set(7, hp, atk, true, this);
                    yield return new WaitForSeconds(2);
                    StartCoroutine(hand[0].GetComponent<CardObj>().Movement(new Vector3(0, -cardPosY, 0.1f)));
                    StartCoroutine(hand[0].GetComponent<CardObj>().Resize(new Vector3(2.5f, 3.4f, 0)));
                    hand[0].GetComponent<CardObj>().drawn = true;

                }
                break;
            default:
                break;
        }
        yield return 0;
        action = false;
    }

    public IEnumerator Miracle() {
        yield return 0;
        if (miracleOpportunity) {
            if (stardust >= 10) {
                for (int i = 0; i < enemyHand.Count; i++) {
                    if (enemyHand[i].GetComponent<CardObj>().clicked) {
                        StartCoroutine(enemyHand[i].GetComponent<CardObj>().RecieveDamage(5));
                        stardust = stardust - 10;
                        manatextfield.text = "star dust: " + stardust;
                        miracleOpportunity = false;
                        break;
                    }
                }
            }
        }
        action = false;
    }
}