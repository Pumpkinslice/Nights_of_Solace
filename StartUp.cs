using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Globals; //global variable storage

public class StartUp : MonoBehaviour
{
    //buttons and their texts to interact with
    public GameObject newgameButton; public GameObject newgameText;
    public GameObject continueButton; public GameObject continueText;
    public GameObject settingsButton; public GameObject settingsText;
    public GameObject creditsButton; public GameObject creditsText;
    public GameObject exitButton; public GameObject exitText;

    //references
    public Text title;
    public AudioSource music;
    public GameObject blackVeil;
    public GameObject mainMenu, settingsPanel, creditsPanel;

    void Start() {
        StartCoroutine(MenuPopUp());
    } 

    void Update(){}

    public IEnumerator MenuPopUp() {
        yield return new WaitForSeconds(1.5f);
        settingsPanel.SetActive(false);
        music.Play();
        yield return new WaitForSeconds(0.5f);
        string name = "";
        if (language == "eng") {
            name = "Nights of Solace";
        } else if (language == "rus") {
            name = "Ночи Утешений";
        }
        for (int i = 0; i < name.Length; i++) {
            title.text = title.text + name[i];
            if (name[i] != ' ') {
                yield return new WaitForSeconds(0.15f);
            }
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeIn(newgameButton, newgameText));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeIn(continueButton, continueText));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeIn(settingsButton, settingsText));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeIn(creditsButton, creditsText));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeIn(exitButton, exitText));
    }

    public IEnumerator FadeIn(GameObject button, GameObject text) {
        for (int i = 0; i < 52; i++) {
            button.GetComponent<Image>().color = new Color(1f, 1f, 1f, i * 5 / 250f);
            text.GetComponent<TextMeshProUGUI>().color = new Color(0.2f, 0.2f, 0.2f, i * 5 / 250f);
            yield return 0;
        }
    }

    public void OnClickNew() {
        battleHappened = false;
        startOfMission = true;
        StartCoroutine(FadeOutScene());
    }

    public void OnClickContinue() {
        battleHappened = false;
        startOfMission = false;
        StartCoroutine(FadeOutScene());
    }

    public IEnumerator FadeOutScene() {
        blackVeil.SetActive(true);
        for (int i = 0; i < 52; i++) {
            yield return 0;
            blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, i * 5 / 255f);
        }
        blackVeil.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);
        ChangeScene("MapScene");
    }

    public void OnClickSettings() {
        StartCoroutine(MenuChange(mainMenu, settingsPanel));
    }

    public void OnClickCredits() {
        StartCoroutine(MenuChange(mainMenu, creditsPanel));
    }

    public void OnClickReturnSettings() {
        StreamWriter textfile = new StreamWriter(Application.dataPath + "/SaveFiles/Settings.txt");
        textfile.WriteLine(fps);
        textfile.WriteLine(vsync);
        textfile.WriteLine((int)(volume * 100f));
        textfile.WriteLine(language);
        textfile.Close();
        if (settingsPanel.GetComponent<SettingsMenu>().languageChange) {
            ChangeScene("MainMenu");
        } else {
            StartCoroutine(MenuChange(settingsPanel, mainMenu));
        }
    }

    public void OnClickReturnCredits() {
        StartCoroutine(MenuChange(creditsPanel, mainMenu));
    }

    public IEnumerator MenuChange(GameObject oldMenu, GameObject newMenu) {
        for (int i = 50; i > 0; i--) {
            oldMenu.GetComponent<CanvasGroup>().alpha = 0.02f * i;
            yield return 0;
        }
        oldMenu.GetComponent<CanvasGroup>().alpha = 0f;
        oldMenu.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        newMenu.SetActive(true);
        for (int i = 0; i < 50; i++) {
            newMenu.GetComponent<CanvasGroup>().alpha = 0.02f * i;
            yield return 0;
        }
        newMenu.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void OnClickExit() {
        Application.Quit();
    }
}
