using System.IO;
using UnityEngine;
using TMPro;
using static Globals; //global variable storage

public class SettingsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsPanel;
    public TMP_Dropdown FPS;
    public TMP_Dropdown lang;
    public UnityEngine.UI.Slider vol;
    public UnityEngine.UI.Toggle sync;
    public AudioSource music;
    public bool languageChange = false; //decides if restart is needed after changing language

    void Start() {
        //reads setting file
        StreamReader textfile = new StreamReader(Application.dataPath + "/SaveFiles/Settings.txt");
        string line = textfile.ReadLine();
        fps = int.Parse(line);
        line = textfile.ReadLine();
        vsync = int.Parse(line);
        line = textfile.ReadLine();
        volume = int.Parse(line) / 100f;
        language = textfile.ReadLine();
        textfile.Close();
        //applies changes from settings
        Application.targetFrameRate = fps;
        QualitySettings.vSyncCount = vsync;
        music.volume = volume;
        //updating setting menu
        if (fps == 30) {
            FPS.value = 0;
        } else if (fps == 60) {
            FPS.value = 1;
        } else if (fps == 120) {
            FPS.value = 2;
        }
        if (vsync == 1) {
            sync.isOn = true;
        } else {
            sync.isOn = false;
        }
        vol.value = volume;
        if (language == "eng") {
            lang.value = 0;
        } else if (language == "rus") {
            lang.value = 1;
        }
    }

    void Update(){}

    public void FPSOnValueChanged() {
        fps = int.Parse(FPS.options[FPS.value].text);
        Application.targetFrameRate = fps;
    }

    public void LanguageOnValueChanged() {
        if (lang.options[lang.value].text == "English") {
            language = "eng";
        } else if (lang.options[lang.value].text == "Russian") {
            language = "rus";
        }
        languageChange = true;
    }

    public void VolumeOnValueChanged() {
        volume = vol.value;
        music.volume = volume;
    }

    public void SyncOnValueChanged() {
        if (sync.isOn) {
            vsync = 1;
        } else {
            vsync = 0;
        }
        QualitySettings.vSyncCount = vsync;
    }
}