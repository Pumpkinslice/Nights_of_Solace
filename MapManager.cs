using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using static Globals; //global variable storage

public class MapManager : MonoBehaviour
{
    //map divided by coordinates
    public int mapSizeX, mapSizeY;
    public int[,] map; //for quicker access to contents of map
    public GameObject[,] mapObj; //for storing generated assets
    public List<GameObject> heroes;

    //prefabs
    public GameObject mountain, hero, gem, woodenLogs;

    //referances
    public Camera cam;
    public GameObject blackVeil;
    public AudioSource music;
    public TextMeshPro resourcesText;

    void Start() {
        music.volume = volume;
        music.Play();
        StreamReader textfile; //start loading progress
        if (startOfMission) {
            textfile = new StreamReader(Application.dataPath + "/CustomAssets/map0.txt");
            startOfMission = false;
        } else {
            if (battleHappened) {
                textfile = new StreamReader(Application.dataPath + "/SaveFiles/BetweenBattleSave.txt");
            } else {
                textfile = new StreamReader(Application.dataPath + "/SaveFiles/Save.txt");
            }
        }
        string line = textfile.ReadLine();
        mapSizeX = line[0] - '0'; //this action converts char to int
        mapSizeY = line[2] - '0';
        map = new int[mapSizeX, mapSizeY];
        mapObj = new GameObject[mapSizeX, mapSizeY];
        for (int y = 0; y < mapSizeY; y++) { //fills map with objects from a text file
            line = textfile.ReadLine();
            for (int x = 0; x < mapSizeX; x++) {
                int obj = (line[x * 3] - '0') * 10 + (line[x * 3 + 1] - '0');
                map[x, mapSizeY - y - 1] = obj; //mapSizeY - y - 1 lets me to write txt file just like a map without needing to reversing y axis
                switch (obj) {
                    case 1:
                        mapObj[x, mapSizeY - y - 1] = Instantiate(mountain, new Vector3(x, mapSizeY - y - 1, 1), transform.rotation);
                        break;
                    case 2:
                        mapObj[x, mapSizeY - y - 1] = Instantiate(gem, new Vector3(x, mapSizeY - y - 1, 1), transform.rotation);
                        break;
                    case 3:
                        mapObj[x, mapSizeY - y - 1] = Instantiate(woodenLogs, new Vector3(x, mapSizeY - y - 1, 1), transform.rotation);
                        break;
                    default:
                        break;
                }
            }
        }
        //read off heroes
        int numOfHeroes = int.Parse(textfile.ReadLine());
        for (int i = 0; i < numOfHeroes; i++) {
            string name = textfile.ReadLine();
            int[] result = ConvertTo3Int(textfile.ReadLine()); // 0-isplayers 1-x 2-y
            heroes.Add(Instantiate(hero, new Vector3(result[1], result[2], 0), transform.rotation));
            heroes[i].GetComponent<HeroMovement>().Set(name, System.Convert.ToBoolean(result[0]), this, cam);
            int numOfCards = int.Parse(textfile.ReadLine());
            for (int j = 0; j < numOfCards; j++) {
                int[] cardResult = ConvertTo3Int(textfile.ReadLine()); // 0-id 1-hpset 2-atkset
                heroes[i].GetComponent<HeroMovement>().IDlist.Add(cardResult[0]);
                heroes[i].GetComponent<HeroMovement>().HPlist.Add(cardResult[1]);
                heroes[i].GetComponent<HeroMovement>().ATKlist.Add(cardResult[2]);
            }
            if (battleHappened) { //checks if a hero needs to be removed or changed after battle
                for (int j = 0; j < 2; j++) {
                    if (heroes[i].GetComponent<HeroMovement>().name == clashingHeroes[j].name) {
                        if (clashingHeroes[j].IDlist.Count == 0) {
                            Destroy(heroes[i]);
                            heroes.RemoveAt(i);
                            i--;
                            numOfHeroes--;
                        } else {
                            heroes[i].GetComponent<HeroMovement>().IDlist = clashingHeroes[j].IDlist;
                            heroes[i].GetComponent<HeroMovement>().HPlist = clashingHeroes[j].HPlist;
                            heroes[i].GetComponent<HeroMovement>().ATKlist = clashingHeroes[j].ATKlist;
                        }
                    }
                }
            }

        }
        //load resources and finish loading
        int[] resources = ConvertTo3Int(textfile.ReadLine());
        wood = resources[0]; ore = resources[1];
        if (!battleHappened) {
            stardust = resources[2];
        }
        UpdateResources();
        textfile.Close();
        battleHappened = false;
    }

    void Update(){}

    public void UpdateResources() {
        resourcesText.text = "Wood - " + wood + "\nOre - " + ore + "\nStar dust - " + stardust;
    }

    public bool CheckUnoccupied(int x, int y) { //checks if a space is good for hero to step on
        if (x >= mapSizeX || y >= mapSizeY || x < 0 || y < 0) { //out of bounds
            return false;
        } else if (map[x, y] == 1) { //obstacle 
            return false;
        } else if (map[x, y] > 1 && map[x, y] < 8) { //collectible
            // 2 - stardust
            // 3 - wood
            // 4 - ore
            // 5 - fuel
            // 6 - metal
            // 7 - crystal
            StartCoroutine(Collect(x, y));
            return true;
        } else {
            return true;
        }
    }

    public IEnumerator Collect(int x, int y) { //collects a thing from a map, applies effect and slowly disapears
        switch (map[x, y]) {
            case 2:
                stardust = stardust + Random.Range(5, 11);
                UpdateResources();
                break;
            case 3:
                wood = wood + Random.Range(3, 6);
                UpdateResources();
                break;
        }
        map[x, y] = 0;
        for (int i = 0; i < 52; i++) {
            yield return 0;
            try { //checks if element is animated or still
                mapObj[x, y].GetComponent<ElementAnimation>().spriteElement.color = new Color(1, 1, 1, (51 - i) * 5 / 255f);
            } catch {
                mapObj[x, y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (51 - i) * 5 / 255f);
            }
        }
        Destroy(mapObj[x, y]);
        mapObj[x, y] = null;
    }

    public IEnumerator FadeOutScene() {
        blackVeil.SetActive(true);
        for (int i = 0; i < 52; i++) {
            yield return 0;
            blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, i * 5 / 255f);
        }
        blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
    }

    public int[] ConvertTo3Int(string input) { //converts a string into 3 ints
        int[] output = new int[3];
        int endofline = 0;
        output[0] = 0; output[1] = 0; output[2] = 0;
        for (int i = 0; input[i] != ' '; i++) {
            endofline = i;
            output[0] = (output[0] * 10) + (input[i] - '0');
        }
        for (int i = endofline + 2; input[i] != ' '; i++) {
            endofline = i;
            output[1] = (output[1] * 10) + (input[i] - '0');
        }
        for (int i = endofline + 2; i < input.Length; i++) {
            output[2] = (output[2] * 10) + (input[i] - '0');
        }
        return output;
    }

    public string ConvertToString(bool input) { //converts a bool to string for saving purposes
        if (input) {
            return "1";
        } else {
            return "0";
        }
    }

    public void SaveProgress(string where) { //saves progress before battle, after clicking save button and upon exiting
        StreamWriter textfile = new StreamWriter(Application.dataPath + "/SaveFiles/" + where + ".txt");
        textfile.WriteLine(mapSizeX + " " + mapSizeY);
        for (int y = mapSizeY - 1; y > -1; y--) { //here saves the whole map
            string lineToWrite = "";
            for (int x = 0; x < mapSizeX; x++) {
                if (map[x, y] < 10) {
                    lineToWrite = lineToWrite + "0" + map[x, y];
                } else {
                    lineToWrite = lineToWrite + map[x, y];
                }
                if (x != mapSizeX - 1) {
                    lineToWrite = lineToWrite + " ";
                }
            }
            textfile.WriteLine(lineToWrite);
        }
        textfile.WriteLine(heroes.Count);
        for (int i = 0; i < heroes.Count; i++) { //here saves all heroes
            HeroMovement hero = heroes[i].GetComponent<HeroMovement>(); //to not call GetComponent in every line 3 times
            textfile.WriteLine(hero.name);
            textfile.WriteLine(ConvertToString(hero.isPlayers) + " " + hero.Xcoor + " " + hero.Ycoor);
            textfile.WriteLine(hero.IDlist.Count);
            for (int j = 0; j < hero.IDlist.Count; j++) {
                textfile.WriteLine(hero.IDlist[j] + " " + hero.HPlist[j] + " " + hero.ATKlist[j]);
            }
        }
        //saves resources
        textfile.WriteLine(wood + " " + ore + " " + stardust);
        textfile.Close();
    }
}