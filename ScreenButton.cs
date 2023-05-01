using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenButton : MonoBehaviour
{
    public BattleScene scene;
    public Text manatextfield;
    public GameObject button;
    public GameObject screen;
    private bool screenIsUp;

    void Start() {
        screenIsUp = false;
    }
    void Update(){}

    private void OnMouseEnter() {
        if (!screenIsUp) {
            if (scene.miracleOpportunity) {
                screenIsUp = true;
                screen.GetComponent<Animator>().SetTrigger("ScreenUp");
                StartCoroutine(TurnOnButtons());
            }
        } else {
            screenIsUp = false;
            manatextfield.text = "";
            button.SetActive(false);
            screen.GetComponent<Animator>().SetTrigger("ScreenDown");
        }
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
    }

    private IEnumerator TurnOnButtons() {
        yield return new WaitForSeconds(0.55f);
        if (screenIsUp) { //to prevent ui appearing if opened and closed the screen too fast
            manatextfield.text = "star dust: " + Globals.stardust;
            button.SetActive(true);
        }
    }

    private void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
    }
}
