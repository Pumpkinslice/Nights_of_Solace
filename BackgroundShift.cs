using UnityEngine;

public class BackgroundShift : MonoBehaviour {
    public bool background; //set from editor, desides if sprite moves on y axis
    void Start() { }

    void Update() {
        if (background) {
            Vector3 coor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(-coor.x / 25, -coor.y / 25 + 0.5f, 100);
        } else {
            Vector3 coor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(-coor.x / 35, 0.9f, 99);
        }
    }
}