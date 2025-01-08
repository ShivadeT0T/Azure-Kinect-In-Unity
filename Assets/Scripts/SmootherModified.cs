using System.Collections.Generic;
using System.Numerics;

public class SmootherModified
{
    // Max allowed size of history list.
    private int maxSize = 100;

    // In case smoother has enough frames in history to perform smooth action.
    private bool hasEnoughForSmoothing = false;

    // Number of the latest frames used to smooth current position; default 5.
    public int NumberSmoothingFrames { get; set; } = 5;

    // Holds received data about moves.
    private List<Body> rawData = new List<Body>();

    // Holds received which are smoothened a little bit.
    private List<Body> smoothenedData = new List<Body>();

    // Process skeleton position and sends back smoothened or raw based on passed parameter.
    public Body ReceiveNewBodyData(Body newData, bool smoothing)
    {
        // In case list is too big.
        if (rawData.Count > maxSize)
        {
            Resize();
        }

        // Add new frame data to raw data used for smoothing.
        rawData.Add(newData);

        // In case value for smoothing is invalid just return original raw frame.
        if (NumberSmoothingFrames <= 1)
        {
            return rawData[rawData.Count - 1];
        }

        // Mark that smoother has enough frames for smoothing.
        if (rawData.Count > NumberSmoothingFrames)
        {
            hasEnoughForSmoothing = true;
        }

        if (smoothenedData.Count == 0)
        {
            smoothenedData.Add(newData);
        }
        else
        {
            Body temp = smoothenedData[smoothenedData.Count - 1] + newData;
            if (hasEnoughForSmoothing)
            {
                temp = temp - rawData[rawData.Count - NumberSmoothingFrames];
            }
            smoothenedData.Add(temp);
        }

        return smoothing && hasEnoughForSmoothing
            ? CalculateAverage(smoothenedData[0], smoothenedData[smoothenedData.Count - 1], newData, NumberSmoothingFrames)
            : rawData[rawData.Count - 1];
    }
    
    // Calculates the average of the smoothenedData
    public Body CalculateAverage(Body firstData, Body cumulativeData, Body nextData, int addAmount)
    {
        float floatAmount = (float)addAmount;
        int maxJointsLength = nextData.Length;
        Body avgData = new Body(maxJointsLength);

        for (int bodyPoint = 0; bodyPoint < nextData.Length; bodyPoint++)
        {
            avgData.JointPositions3D[bodyPoint] = cumulativeData.JointPositions3D[bodyPoint] / floatAmount;
            avgData.JointPositions2D[bodyPoint] = cumulativeData.JointPositions2D[bodyPoint] / floatAmount;

            Vector4 cumulativeVector = new Vector4
                (
                cumulativeData.JointRotations[bodyPoint].X,
                cumulativeData.JointRotations[bodyPoint].Y,
                cumulativeData.JointRotations[bodyPoint].Z,
                cumulativeData.JointRotations[bodyPoint].W
                );

            avgData.JointRotations[bodyPoint] =
                QuaternionMath.AverageQuaternion
                (
                    ref cumulativeVector,
                    nextData.JointRotations[bodyPoint],
                    firstData.JointRotations[bodyPoint],
                    addAmount
                );

            avgData.JointPrecisions[bodyPoint] = cumulativeData.JointPrecisions[bodyPoint];
        }
        return avgData;
    }

    // Deletes old position data from list which do not have more impact on smoothing algorithm.
    public void Resize()
    {
        if (rawData.Count > NumberSmoothingFrames)
        {
            rawData.RemoveRange(0, rawData.Count - NumberSmoothingFrames);
        }
        if (smoothenedData.Count > NumberSmoothingFrames)
        {
            smoothenedData.RemoveRange(0, smoothenedData.Count - NumberSmoothingFrames);
        }
    }

}
