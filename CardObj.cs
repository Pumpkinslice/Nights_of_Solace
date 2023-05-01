using System.Collections;
using UnityEngine;
using TMPro;
using static Globals; //global variable storage

public class CardObj : MonoBehaviour {
    //card's stats
    public int id; //all abilities depend on this parameter
    public int atk;
    public int hp;
    public int maxAtk; //to remember atk of customly made cards between battles
    public int maxHp; //for heal effects not to overheal
    public int speed;
    public bool corrupt; //prevents healing and buffing is applied

    //scene flags
    public bool drawn;
    public bool clicked;
    public bool isPlayers; //is in possession of player

    //references
    public GameObject textfieldatk;
    public GameObject textfieldhp;
    public GameObject textfieldspd;
    public GameObject textfieldname;
    public GameObject textfieldspecialty;
    public GameObject unitsprite;
    public GameObject corruptsprite;
    public BattleScene scene;

    //object's stats
    private float z; //to remember previous "layer" upon mouse hovering over 
    public Quaternion angle; //to remember previous rotation upon mouse hovering over 

    void Start() { }
    void Update() { }

    public void Set(int idset, int hpset, int atkset, bool isPlayer, BattleScene sceneset) //use atkset if the card is customly generated
    {
        id = idset;
        drawn = false;
        clicked = false;
        corrupt = false;
        isPlayers = isPlayer;
        scene = sceneset;
        hp = hpset;
        switch (idset) {
            case 0:
                atk = 5; maxHp = 10; speed = 3;
                textfieldname.GetComponent<TextMeshPro>().SetText("Frenzied zealot");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Enchanted krises let him steal hp from wounded enemies");
                break;
            case 1:
                atk = 7; maxHp = 15; speed = 1;
                textfieldname.GetComponent<TextMeshPro>().SetText("Generous cultist");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Sacrifices own health to bolster other unit's attack (repeated)");
                break;
            case 2:
                atk = 10; maxHp = 20; speed = 0;


                break;
            case 6:
                atk = 20; maxHp = 30; speed = -2;
                textfieldname.GetComponent<TextMeshPro>().SetText("Star caller");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Sacrifice your entire hand to beckon distant stars... (once per turn)");
                break;
            case 7:
                atk = atkset; maxHp = hpset; speed = 0;
                textfieldname.GetComponent<TextMeshPro>().SetText("Star eater");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Celestial might wounds entire armies");
                break;
            case 10:
                atk = 15; maxHp = 12; speed = 1;
                textfieldname.GetComponent<TextMeshPro>().SetText("Expert Demolitionist");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Blows up enemy team one last time as a farewell gesture");
                break;
            case 11:
                atk = 12; maxHp = 25; speed = 0;
                textfieldname.GetComponent<TextMeshPro>().SetText("Corruptor gator");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Corrupts its victums, preventing them from receiving all blessings");
                break;
            case 12:
                atk = 16; maxHp = 35; speed = -1;
                textfieldname.GetComponent<TextMeshPro>().SetText("Mining behemoth");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Monstrous drill hits adjustment units as well");
                break;
            case 16:
                atk = 10; maxHp = 15; speed = -2;
                textfieldname.GetComponent<TextMeshPro>().SetText("Peacemaker");
                textfieldspecialty.GetComponent<TextMeshPro>().SetText("Blessed shield weakens those who dare attack");
                break;
            default:
                break;
        }
        try { //if sprite does not exist
            unitsprite.GetComponent<SpriteRenderer>().sprite = LoadSprite(Application.dataPath + "/CustomAssets/Unit" + idset + ".png");
        } catch { }
        maxAtk = atk;
        unitsprite.GetComponent<SpriteRenderer>().size = new Vector2(0.05f, 0.09f);
        textfieldatk.GetComponent<TextMeshPro>().SetText(atk.ToString());
        textfieldhp.GetComponent<TextMeshPro>().SetText(hp.ToString());
        textfieldspd.GetComponent<TextMeshPro>().SetText(speed.ToString());
    }

    public void UpdateStats() //to quickly apply stat changes
    {
        textfieldatk.GetComponent<TextMeshPro>().SetText(atk.ToString());
        textfieldhp.GetComponent<TextMeshPro>().SetText(hp.ToString());
        textfieldspd.GetComponent<TextMeshPro>().SetText(speed.ToString());
    }

    private void OnMouseEnter() {
        if (drawn) {
            z = gameObject.transform.position.z;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -0.1f);
            StartCoroutine(Rotation(Quaternion.Euler(0, 0, 0)));
            StartCoroutine(Resize(new Vector3(3f, 4.08f, 0)));
        }
    }

    private void OnMouseExit() {
        if (drawn) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, z);
            StartCoroutine(Rotation(angle));
            StartCoroutine(Resize(new Vector3(2.5f, 3.4f, 0)));
        }
    }

    private void OnMouseDown() {
        if (isPlayers) {
            if (drawn && !clicked) {
                scene.DisableChoise(true);
                clicked = true;
                gameObject.GetComponent<ParticleSystem>().Play();
            } else if (drawn && clicked) {
                clicked = false;
                gameObject.GetComponent<ParticleSystem>().Stop();
            }
        } else {
            if (drawn && !clicked) {
                scene.DisableChoise(false);
                clicked = true;
#pragma warning disable CS0618 // Type or member is obsolete
                gameObject.GetComponent<ParticleSystem>().startColor = new Color(1, 0, 0, 1);
                gameObject.GetComponent<ParticleSystem>().Play();
            } else if (drawn && clicked) {
                clicked = false;
                gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    public IEnumerator RecieveDamage(int dmg) {
        if (hp < dmg) {
            hp = 0;
        } else {
            hp = hp - dmg;
        }
        Color initial = gameObject.GetComponent<SpriteRenderer>().material.color;
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(0.8f, 0, 0, 1);
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<SpriteRenderer>().material.color = initial;
        textfieldhp.GetComponent<TextMeshPro>().SetText(hp.ToString());
        switch(id) { //on hit ability
            case 16:
                if (isPlayers) {
                    for (int i = 0; i < scene.enemyHand.Count; i++) {
                        if (scene.enemyHand[i].GetComponent<CardObj>().atk < 3) {
                            scene.enemyHand[i].GetComponent<CardObj>().atk = 0;
                        } else {
                            scene.enemyHand[i].GetComponent<CardObj>().atk -= 3;
                        }
                        scene.enemyHand[i].GetComponent<CardObj>().UpdateStats();
                    }
                } else {
                    for (int i = 0; i < scene.hand.Count; i++) {
                        if (scene.hand[i].GetComponent<CardObj>().atk < 3) {
                            scene.hand[i].GetComponent<CardObj>().atk = 0;
                        } else {
                            scene.hand[i].GetComponent<CardObj>().atk -= 3;
                        }
                        scene.hand[i].GetComponent<CardObj>().UpdateStats();
                    }
                }
                break;
            default:
                break;
        }
        if (hp == 0) {
            switch(id) { //on death ability
                case 10:
                    if (isPlayers) {
                        for (int i = 0; i < scene.enemyHand.Count; i++) {
                            StartCoroutine(scene.enemyHand[i].GetComponent<CardObj>().RecieveDamage(atk));
                        }
                    } else {
                        for (int i = 0; i < scene.hand.Count; i++) {
                            StartCoroutine(scene.hand[i].GetComponent<CardObj>().RecieveDamage(atk));
                        }
                    }
                    break;
                default:
                    break;
            }
            StartCoroutine(scene.CheckHP(gameObject, isPlayers));
        }
    }

    public IEnumerator Heal(int heal) {
        if (hp + heal > maxHp) {
            hp = maxHp;
        } else {
            hp = hp + heal;
        }
        Color initial = gameObject.GetComponent<SpriteRenderer>().material.color;
        gameObject.GetComponent<SpriteRenderer>().material.color = new Color(0.6f, 1, 0, 1);
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<SpriteRenderer>().material.color = initial;
        textfieldhp.GetComponent<TextMeshPro>().SetText(hp.ToString());
    }

    public void DoDamage(GameObject target) {
        switch (id) { //on hit ability
            case 0:
                if (!corrupt) {
                    if (target.GetComponent<CardObj>().hp < atk) {
                        StartCoroutine(Heal(target.GetComponent<CardObj>().hp));
                    } else {
                        StartCoroutine(Heal(atk));
                    }
                }
                StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                break;
            case 7:
                if (isPlayers) {
                    for (int i = 0; i < scene.enemyDraw.Count; i++) {
                        if (scene.enemyDraw[i].GetComponent<CardObj>().hp > atk / 3) {
                            StartCoroutine(scene.enemyDraw[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        } else {
                            StartCoroutine(scene.enemyDraw[i].GetComponent<CardObj>().RecieveDamage(scene.enemyDraw[i].GetComponent<CardObj>().hp - 1));
                        }
                    }
                    for (int i = 0; i < scene.enemyHand.Count; i++) {
                        if (scene.enemyHand[i] == target) {
                            StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                        } else {
                            StartCoroutine(scene.enemyHand[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        }
                    }
                    for (int i = 0; i < scene.enemyDis.Count; i++) {
                        if (scene.enemyDis[i].GetComponent<CardObj>().hp > atk / 3) {
                            StartCoroutine(scene.enemyDis[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        } else {
                            StartCoroutine(scene.enemyDis[i].GetComponent<CardObj>().RecieveDamage(scene.enemyDis[i].GetComponent<CardObj>().hp - 1));
                        }
                    }
                } else {
                    for (int i = 0; i < scene.deckDraw.Count; i++) {
                        if (scene.deckDraw[i].GetComponent<CardObj>().hp > atk / 3) {
                            StartCoroutine(scene.deckDraw[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        } else {
                            StartCoroutine(scene.deckDraw[i].GetComponent<CardObj>().RecieveDamage(scene.deckDraw[i].GetComponent<CardObj>().hp - 1));
                        }
                    }
                    for (int i = 0; i < scene.hand.Count; i++) {
                        if (scene.hand[i] == target) {
                            StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                        } else {
                            StartCoroutine(scene.hand[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        }
                    }
                    for (int i = 0; i < scene.deckDis.Count; i++) {
                        if (scene.deckDis[i].GetComponent<CardObj>().hp > atk / 3) {
                            StartCoroutine(scene.deckDis[i].GetComponent<CardObj>().RecieveDamage(atk / 3));
                        } else {
                            StartCoroutine(scene.deckDis[i].GetComponent<CardObj>().RecieveDamage(scene.deckDis[i].GetComponent<CardObj>().hp - 1));
                        }
                    }
                }
                break;
            case 11:
                if (isPlayers) {
                    for (int i = 0; i < scene.enemyHand.Count; i++) {
                        scene.enemyHand[i].GetComponent<CardObj>().corrupt = true;
                        scene.enemyHand[i].GetComponent<CardObj>().corruptsprite.SetActive(true);
                    }
                } else {
                    for (int i = 0; i < scene.hand.Count; i++) {
                        scene.hand[i].GetComponent<CardObj>().corrupt = true;
                        scene.hand[i].GetComponent<CardObj>().corruptsprite.SetActive(true);
                    }
                }
                StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                break;
            case 12:
                int primaryTarget = 0;
                if (isPlayers) {
                    for (int i = 0; i < scene.enemyHand.Count; i++) {
                        if (scene.enemyHand[i] == target) {
                            primaryTarget = i;
                        }
                    }
                    if (primaryTarget != 0) {
                        StartCoroutine(scene.enemyHand[primaryTarget - 1].GetComponent<CardObj>().RecieveDamage(atk / 2));
                    }
                    if (primaryTarget != scene.enemyHand.Count - 1) {
                        StartCoroutine(scene.enemyHand[primaryTarget + 1].GetComponent<CardObj>().RecieveDamage(atk / 2));
                    }
                } else {
                    for (int i = 0; i < scene.hand.Count; i++) {
                        if (scene.hand[i] == target) {
                            primaryTarget = i;
                        }
                    }
                    if (primaryTarget != 0) {
                        StartCoroutine(scene.hand[primaryTarget - 1].GetComponent<CardObj>().RecieveDamage(atk / 2));
                    }
                    if (primaryTarget != scene.hand.Count - 1) {
                        StartCoroutine(scene.hand[primaryTarget + 1].GetComponent<CardObj>().RecieveDamage(atk / 2));
                    }
                }
                StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                break;
            default:
                StartCoroutine(target.GetComponent<CardObj>().RecieveDamage(atk));
                break;
        }
    }

    public IEnumerator Rotation(Quaternion newAngle) //smooth rotation
    {
        int quarter = fps / 12; //lasts 5th of a second with normal fps
        int firstAngle, secondAngle;
        if (newAngle.eulerAngles.z > 180) {
            firstAngle = (int)newAngle.eulerAngles.z - 360;
        } else {
            firstAngle = (int)newAngle.eulerAngles.z;
        }
        if (transform.eulerAngles.z > 180) {
            secondAngle = (int)transform.eulerAngles.z - 360;
        } else {
            secondAngle = (int)transform.eulerAngles.z;
        }
        float path = (firstAngle - secondAngle) / (float)quarter;
        for (int i = 0; i < quarter; i++) {
            transform.Rotate(0, 0, path);
            yield return 0;
        }
        transform.eulerAngles = newAngle.eulerAngles; //just to negate floating numbers being unprecise
    }

    public IEnumerator Movement(Vector3 position) //smooth movement
    {
        int time = fps / 3;
        float xspeed = (position.x - transform.position.x) / (float)time;
        float yspeed = (position.y - transform.position.y) / (float)time;
        for (int i = 0; i < time; i++) {
            transform.position = new Vector3(transform.position.x + xspeed, transform.position.y + yspeed, position.z);
            yield return 0;
        }
        transform.position = position; //just to negate floating numbers being unprecise
    }

    public IEnumerator Resize(Vector3 size) { //smooth size change
        int time = fps / 6;
        float xspeed = (size.x - transform.localScale.x) / (float)time;
        float yspeed = (size.y - transform.localScale.y) / (float)time;
        for (int i = 0; i < time; i++) {
            transform.localScale = new Vector3(transform.localScale.x + xspeed, transform.localScale.y + yspeed, 0);
            yield return 0;
        }
        transform.localScale = size; //just to negate floating numbers being unprecise
    }

    private Sprite LoadSprite(string filename) //loads an image from assets to not have them all loaded everytime
    {
        //bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Save/" + filename);
        Texture2D texture = new Texture2D(1920, 1080);
        texture.LoadImage(System.IO.File.ReadAllBytes(filename));
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1920, 1080), new Vector2(0, 0));
        return sprite;
    }
}