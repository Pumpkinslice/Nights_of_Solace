using UnityEngine;

public class Button_prompt : MonoBehaviour {
    void Start() { }
    void Update() { }

    public void OnClickEnd(BattleScene scene) {
        if (!scene.action) {
            scene.action = true;
            if (scene.playersturn) {
                StartCoroutine(scene.AIAttack());
            } else {
                StartCoroutine(scene.End());
            }
        }
    }

    public void OnClickAttack(BattleScene scene) {
        if (!scene.action) {
            scene.action = true;
            StartCoroutine(scene.Attack());
        }
    }

    public void OnClickAbility(BattleScene scene) {
        if (!scene.action) {
            scene.action = true;
            StartCoroutine(scene.Ability());
        }
    }

    public void OnClickMiracle(BattleScene scene) {
        if (!scene.action) {
            scene.action = true;
            StartCoroutine(scene.Miracle());
        }
    }

    public void OnClickExit() {
        Application.Quit();
    }
}