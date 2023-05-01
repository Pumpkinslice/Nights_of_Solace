using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    private int pressed; //refers to which tab is currently open
    private bool opened;
    public GameObject tabpanel;
    public GameObject[] tab; //assign through editor
    public GameObject[] tabtext; //assign through editor
    public Sprite closedtab, openedtab;

    void Start() {
        pressed = 0;
        opened = false;
    }

    void Update(){}

    public void OnClickQuestion() {
        StartCoroutine(QuestionMark());
    }

    private IEnumerator QuestionMark() {
        if (!opened) {
            opened = true;
            tabpanel.SetActive(true);
            tabpanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(6.5f, -0.5f);
        } else {
            opened = false;
            tabpanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(15f, -0.5f);
            tabpanel.SetActive(false);
        }
        yield return 0;
    }

    public void OnClickTab(int num) {
        if (pressed == num) {
            pressed = 0;
        } else {
            pressed = num;
        }
        for (int i = 0; i < tab.Length; i++) {
            if (i == pressed - 1) {
                tab[i].GetComponent<Image>().sprite = openedtab;
                tabtext[i].SetActive(true);
            } else {
                tab[i].GetComponent<Image>().sprite = closedtab;
                tabtext[i].SetActive(false);
            }
            tab[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(11f, 230f - (50f * i));
        }
        if (pressed != 0) {
            int deviation = 0;
            if (pressed == 1 || pressed == 3) {
                deviation = 85;
            } else if (pressed == 2) {
                deviation = 155;
            }
            for (int i = pressed; i != tab.Length; i++) {
                tab[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(11f, tab[i].GetComponent<RectTransform>().anchoredPosition.y - deviation);
            }
        }
    }
}