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
}
