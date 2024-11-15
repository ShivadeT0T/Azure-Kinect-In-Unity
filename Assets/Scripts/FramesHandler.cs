using System.Collections.Concurrent;
using System.Collections.Generic;

public enum HandlerType
{
    SAVE,
    LOAD,
    BOTH
}
public class FramesHandler
{
    private readonly int FrameLimit;
    private int FrameCount = 0;
    public BackgroundDataNoDepth[] FramesArray;
    public ConcurrentQueue<BackgroundDataNoDepth> FramesProcessor;
    public List<BackgroundDataNoDepth> FramesList;

    public FramesHandler(int frameLimit, HandlerType handlerType)
    {
        FrameLimit = frameLimit;

        switch (handlerType)
        {
            case HandlerType.SAVE:
                FramesArray = new BackgroundDataNoDepth[FrameLimit];
                FramesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
                break;
            case HandlerType.LOAD:
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            case HandlerType.BOTH:
                FramesArray = new BackgroundDataNoDepth[FrameLimit];
                FramesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            default:
                break;
        }
    }

    private void ProcessFrame()
    {

    }

}