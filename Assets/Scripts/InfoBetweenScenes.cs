public enum MenuState
{
    NORMAL,
    ERROR
}

public static class InfoBetweenScenes
{
    public static string AnimationFileName = "incorrectFile";
    public static string ErrorMessage;
    public static MenuState menuState = MenuState.NORMAL;
}
