using UnityEngine;

public class CamShift : MonoBehaviour
{
    public Camera cam;
    public MapManager map;
    public bool hoveredOver;
    public int direction; //assign in editor to specify direction of camera shifting
    // 1 - up
    // 2 - right
    // 3 - down
    // 4 - left

    void Start(){}

    void Update() {
        if (hoveredOver) {
            switch (direction) {
                case 1:
                    if (cam.transform.position.y < map.mapSizeY) {
                        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 0.085f, -10);
                    }
                    break;
                case 2:
                    if (cam.transform.position.x < map.mapSizeX) {
                        cam.transform.position = new Vector3(cam.transform.position.x + 0.085f, cam.transform.position.y, -10);
                    }
                    break;
                case 3:
                    if (cam.transform.position.y > 0) {
                        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - 0.085f, -10);
                    }
                    break;
                case 4:
                    if (cam.transform.position.x > 0) {
                        cam.transform.position = new Vector3(cam.transform.position.x - 0.085f, cam.transform.position.y, -10);
                    }
                    break;
            }
        }
    }

    private void OnMouseEnter() {
        hoveredOver = true;
    }

    private void OnMouseExit() {
        hoveredOver = false;
    }
}
