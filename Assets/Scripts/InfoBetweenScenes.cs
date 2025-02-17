using System.IO;

public enum MenuState
{
    NORMAL,
    ERROR
}

public enum DifficultyState
{
    HARD,
    NORMAL,
    EASY
}

public static class InfoBetweenScenes
{
    // "incorrectFile" "test"
    public static string AnimationFileName = "GameTest";
    public static string ErrorMessage;
    public static MenuState menuState = MenuState.NORMAL;
    public static string prefabDirectory = "PrefabModels" + Path.DirectorySeparatorChar;
    public static string prefabModelName = "jasper";
    public static DifficultyState diffficultyState = DifficultyState.HARD;
}
