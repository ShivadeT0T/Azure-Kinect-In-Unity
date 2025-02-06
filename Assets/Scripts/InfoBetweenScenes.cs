using System.IO;

public enum MenuState
{
    NORMAL,
    ERROR
}

public static class InfoBetweenScenes
{
    // "incorrectFile" "test"
    public static string AnimationFileName = "test";
    public static string ErrorMessage;
    public static MenuState menuState = MenuState.NORMAL;
    public static string prefabDirectory = "PrefabModels" + Path.DirectorySeparatorChar;
    public static string prefabModelName = "jasper";
    // Intervals between poses in seconds
    public static int poseInterval = 2;
}
