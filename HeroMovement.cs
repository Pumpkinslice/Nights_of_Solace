using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals; //global variable storage

public class HeroMovement : MonoBehaviour
{
    public string name; //hero's name and also works as hero's ID (there can't be multiple same-named heroes)
    public bool isPlayers;
    public bool selected;
    private bool moving; //needed for movement not to conflict with each other
    public int Xcoor, Ycoor;

    //referances
    public Camera cam;
    public MapManager map;

    //army
    public List<int> IDlist, HPlist, ATKlist;

    void Start() {}

    public void Set(string nameSet, bool isplayers, MapManager mapStart, Camera camStart) {
        name = nameSet;
        isPlayers = isplayers;
        if (isPlayers) {
            GetComponent<SpriteRenderer>().color = Color.green;
        } else {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        selected = false;
        moving = false;
        Xcoor = (int)transform.position.x;
        Ycoor = (int)transform.position.y;
        cam = camStart;
        map = mapStart;
    }

    private void OnMouseDown() {
        if (isPlayers) {
            if (!selected) {
                for (int i = 0; i < map.heroes.Count; i++) {
                    map.heroes[i].GetComponent<HeroMovement>().selected = false;
                }
                selected = true;
                cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            } else {
                selected = false;
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            if (selected && !moving && map.CheckUnoccupied(Xcoor, Ycoor + 1)) {
                Ycoor = Ycoor + 1;
                moving = true;
                StartCoroutine(Movement(new Vector3(Xcoor, Ycoor, 0)));
                StartCoroutine(CheckBattle());
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            if (selected && !moving && map.CheckUnoccupied(Xcoor, Ycoor - 1)) {
                Ycoor = Ycoor - 1;
                moving = true;
                StartCoroutine(Movement(new Vector3(Xcoor, Ycoor, 0)));
                StartCoroutine(CheckBattle());
            }
        } else if (Input.GetKeyDown(KeyCode.A)) {
            if (selected && !moving && map.CheckUnoccupied(Xcoor - 1, Ycoor)) {
                Xcoor = Xcoor - 1;
                moving = true;
                StartCoroutine(Movement(new Vector3(Xcoor, Ycoor, 0)));
                StartCoroutine(CheckBattle());
            }
        } else if (Input.GetKeyDown(KeyCode.D)) {
            if (selected && !moving && map.CheckUnoccupied(Xcoor + 1, Ycoor)) {
                Xcoor = Xcoor + 1;
                moving = true;
                StartCoroutine(Movement(new Vector3(Xcoor, Ycoor, 0)));
                StartCoroutine(CheckBattle());
            }
        }
    }

    public IEnumerator Movement(Vector3 position) { //smooth movement
        //fixed cam is needed to decide, do you need to move camera with hero or not
        bool fixedCam = transform.position.x == cam.transform.position.x && transform.position.y == cam.transform.position.y;
        int time = fps / 5;
        float xspeed = (position.x - transform.position.x) / (float)time;
        float yspeed = (position.y - transform.position.y) / (float)time;
        for (int i = 0; i < time; i++) {
            transform.position = new Vector3(transform.position.x + xspeed, transform.position.y + yspeed, 0);
            if (fixedCam) {
                cam.transform.position = new Vector3(cam.transform.position.x + xspeed, cam.transform.position.y + yspeed, -10);
            }
            yield return 0;
        }
        transform.position = position; //just to negate floating numbers being unprecise
        if (fixedCam) {
            cam.transform.position = new Vector3(position.x, position.y, -10);
        }
        moving = false;
    }

    public IEnumerator CheckBattle() { //checks if hostile heroes collide
        bool found = false;
        for (int i = 0; i < map.heroes.Count; i++) { 
            if (map.heroes[i].GetComponent<HeroMovement>().isPlayers == !isPlayers && map.heroes[i].GetComponent<HeroMovement>().Xcoor == Xcoor && map.heroes[i].GetComponent<HeroMovement>().Ycoor == Ycoor) {
                found = true;
                if (isPlayers) {
                    clashingHeroes[1] = map.heroes[i].GetComponent<HeroMovement>();
                    clashingHeroes[0] = this;
                } else {
                    clashingHeroes[0] = map.heroes[i].GetComponent<HeroMovement>();
                    clashingHeroes[1] = this;
                }
                break;
            }
        }
        if (found) {
            map.SaveProgress("BetweenBattleSave"); //save before battle to remember the world
            battleHappened = true;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(map.FadeOutScene());
            yield return new WaitForSeconds(1);
            ChangeScene("BattleScene");
        }
        
    }
}