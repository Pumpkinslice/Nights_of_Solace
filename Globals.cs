public class Globals //nice storage for global variables, doesn't need to be assigned to a GameObject to persist between scenes
{
    //settings
    public static int fps;
    public static float volume;
    public static string language;
    public static int vsync;

    //flags to carry between scenes
    public static bool startOfMission;
    public static bool battleHappened; 
    public static HeroMovement[] clashingHeroes = new HeroMovement[2]; //stores both heroes for a battle, 0 - player's, 1 - enemy's 

    //resources
    public static int wood;
    public static int ore;
    public static int stardust;

    public static void ChangeScene(string name) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}