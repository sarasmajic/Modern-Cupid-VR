using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenu
{
    [MenuItem("Tools/My Scene Loader/MainMenu")]
    public static void OpenMenu() {
        OpenScene("MainMenu");
    }
    
    [MenuItem("Tools/My Scene Loader/Level0")]
    public static void OpenLevel0() {
        OpenScene("Level0");
    }

    [MenuItem("Tools/My Scene Loader/Sandbox")]
    public static void OpenSandbox()
    {
        OpenScene("Sandbox");
    }

    private static void OpenScene(string sceneName) {
        EditorSceneManager.OpenScene("Assets/Scenes/"+ sceneName +".unity", OpenSceneMode.Single);
    }
    
}
