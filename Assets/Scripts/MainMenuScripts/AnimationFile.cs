
using System;

public struct AnimationFile
{
    public string Name;
    public DateTime CreationTime;

    public AnimationFile(string name, DateTime creationTime)
    {
        Name = name;
        CreationTime = creationTime;
    }
}
