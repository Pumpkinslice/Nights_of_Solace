using System.Collections;
using UnityEngine;

public class ElementAnimation : MonoBehaviour
{
    public string elementName; //set in editor to specify, which element to render
    private bool active;
    private float time = 2f / 12f; //does full animation in 2 seconds
    private int num;
    private Sprite[] image = new Sprite[12];
    public SpriteRenderer spriteElement;

    void Start() {
        num = 0;
        active = false;
        for (int i = 0; i < 12; i++) {
            Texture2D texture = new Texture2D(200, 200);
            texture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/CustomAssets/" + elementName + "/" + elementName + i + ".png"));
            image[i] = Sprite.Create(texture, new Rect(0, 0, 200, 200), new Vector2(0, 0));
        }
        spriteElement.sprite = image[11];
        StartCoroutine(Animated());
    }

    void Update(){}

    private IEnumerator Animated() {
        while(true) {
            if (active) { //to avoid working when offscreen
                spriteElement.sprite = image[num];
                if (num == 11) {
                    num = 0;
                } else {
                    num++;
                }
            }
            yield return new WaitForSeconds(time);
        }
    }

    private void OnBecameVisible() {
        active = true;
    }

    private void OnBecameInvisible() {
        active = false;
    }
}