using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointCalibration : MonoBehaviour
{
    public Dictionary<JointId, JointId> parentJointMap;

    Dictionary<JointId, Quaternion> basisJointMap;

    Dictionary<JointId, float> boneScaler;

    private List<Body> liveBodyData = new List<Body>();

    Body averageLiveBody;


    // Max allowed size of history list.
    private int maxListSize = 100;

    //private bool enoughData = false;

    #region joint mapping
    // Start is called before the first frame update
    void Awake()
    {
        parentJointMap = new Dictionary<JointId, JointId>();

        // pelvis has no parent so set to count
        parentJointMap[JointId.Pelvis] = JointId.Head;
        parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
        parentJointMap[JointId.SpineChest] = JointId.Pelvis;
        parentJointMap[JointId.Neck] = JointId.Pelvis;
        parentJointMap[JointId.ClavicleLeft] = JointId.Pelvis;
        parentJointMap[JointId.ShoulderLeft] = JointId.Pelvis;
        parentJointMap[JointId.ElbowLeft] = JointId.Pelvis;
        parentJointMap[JointId.WristLeft] = JointId.Pelvis;
        parentJointMap[JointId.HandLeft] = JointId.Pelvis;
        parentJointMap[JointId.HandTipLeft] = JointId.Pelvis;
        parentJointMap[JointId.ThumbLeft] = JointId.Pelvis;
        parentJointMap[JointId.ClavicleRight] = JointId.Pelvis;
        parentJointMap[JointId.ShoulderRight] = JointId.Pelvis;
        parentJointMap[JointId.ElbowRight] = JointId.Pelvis;
        parentJointMap[JointId.WristRight] = JointId.Pelvis;
        parentJointMap[JointId.HandRight] = JointId.Pelvis;
        parentJointMap[JointId.HandTipRight] = JointId.Pelvis;
        parentJointMap[JointId.ThumbRight] = JointId.Pelvis;
        parentJointMap[JointId.HipLeft] = JointId.Pelvis;
        parentJointMap[JointId.KneeLeft] = JointId.Pelvis;
        parentJointMap[JointId.AnkleLeft] = JointId.Pelvis;
        parentJointMap[JointId.FootLeft] = JointId.Pelvis;
        parentJointMap[JointId.HipRight] = JointId.Pelvis;
        parentJointMap[JointId.KneeRight] = JointId.Pelvis;
        parentJointMap[JointId.AnkleRight] = JointId.Pelvis;
        parentJointMap[JointId.FootRight] = JointId.Pelvis;
        parentJointMap[JointId.Head] = JointId.Pelvis;
        parentJointMap[JointId.Nose] = JointId.Pelvis;
        parentJointMap[JointId.EyeLeft] = JointId.Pelvis;
        parentJointMap[JointId.EarLeft] = JointId.Pelvis;
        parentJointMap[JointId.EyeRight] = JointId.Pelvis;
        parentJointMap[JointId.EarRight] = JointId.Pelvis;

        Vector3 zpositive = Vector3.forward;
        Vector3 xpositive = Vector3.right;
        Vector3 ypositive = Vector3.up;
        // spine and left hip are the same
        Quaternion leftHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion spineHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion rightHipBasis = Quaternion.LookRotation(xpositive, zpositive);
        // arms and thumbs share the same basis
        Quaternion leftArmBasis = Quaternion.LookRotation(ypositive, -zpositive);
        Quaternion rightArmBasis = Quaternion.LookRotation(-ypositive, zpositive);
        Quaternion leftHandBasis = Quaternion.LookRotation(-zpositive, -ypositive);
        Quaternion rightHandBasis = Quaternion.identity;
        Quaternion leftFootBasis = Quaternion.LookRotation(xpositive, ypositive);
        Quaternion rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive);

        basisJointMap = new Dictionary<JointId, Quaternion>();

        // pelvis has no parent so set to count
        basisJointMap[JointId.Pelvis] = spineHipBasis;
        basisJointMap[JointId.SpineNavel] = spineHipBasis;
        basisJointMap[JointId.SpineChest] = spineHipBasis;
        basisJointMap[JointId.Neck] = spineHipBasis;
        basisJointMap[JointId.ClavicleLeft] = leftArmBasis;
        basisJointMap[JointId.ShoulderLeft] = leftArmBasis;
        basisJointMap[JointId.ElbowLeft] = leftArmBasis;
        basisJointMap[JointId.WristLeft] = leftHandBasis;
        basisJointMap[JointId.HandLeft] = leftHandBasis;
        basisJointMap[JointId.HandTipLeft] = leftHandBasis;
        basisJointMap[JointId.ThumbLeft] = leftArmBasis;
        basisJointMap[JointId.ClavicleRight] = rightArmBasis;
        basisJointMap[JointId.ShoulderRight] = rightArmBasis;
        basisJointMap[JointId.ElbowRight] = rightArmBasis;
        basisJointMap[JointId.WristRight] = rightHandBasis;
        basisJointMap[JointId.HandRight] = rightHandBasis;
        basisJointMap[JointId.HandTipRight] = rightHandBasis;
        basisJointMap[JointId.ThumbRight] = rightArmBasis;
        basisJointMap[JointId.HipLeft] = leftHipBasis;
        basisJointMap[JointId.KneeLeft] = leftHipBasis;
        basisJointMap[JointId.AnkleLeft] = leftHipBasis;
        basisJointMap[JointId.FootLeft] = leftFootBasis;
        basisJointMap[JointId.HipRight] = rightHipBasis;
        basisJointMap[JointId.KneeRight] = rightHipBasis;
        basisJointMap[JointId.AnkleRight] = rightHipBasis;
        basisJointMap[JointId.FootRight] = rightFootBasis;
        basisJointMap[JointId.Head] = spineHipBasis;
        basisJointMap[JointId.Nose] = spineHipBasis;
        basisJointMap[JointId.EyeLeft] = spineHipBasis;
        basisJointMap[JointId.EarLeft] = spineHipBasis;
        basisJointMap[JointId.EyeRight] = spineHipBasis;
        basisJointMap[JointId.EarRight] = spineHipBasis;

        boneScaler = new Dictionary<JointId, float>();

        boneScaler[JointId.Pelvis] = 1.0f;
        boneScaler[JointId.SpineNavel] = 1.0f;
        boneScaler[JointId.SpineChest] = 1.0f;
        boneScaler[JointId.Neck] = 1.0f;
        boneScaler[JointId.ClavicleLeft] = 1.0f;
        boneScaler[JointId.ShoulderLeft] = 1.0f;
        boneScaler[JointId.ElbowLeft] = 1.0f;
        boneScaler[JointId.WristLeft] = 1.0f;
        boneScaler[JointId.HandLeft] = 1.0f;
        boneScaler[JointId.HandTipLeft] = 1.0f;
        boneScaler[JointId.ThumbLeft] = 1.0f;
        boneScaler[JointId.ClavicleRight] = 1.0f;
        boneScaler[JointId.ShoulderRight] = 1.0f;
        boneScaler[JointId.ElbowRight] = 1.0f;
        boneScaler[JointId.WristRight] = 1.0f;
        boneScaler[JointId.HandRight] = 1.0f;
        boneScaler[JointId.HandTipRight] = 1.0f;
        boneScaler[JointId.ThumbRight] = 1.0f;
        boneScaler[JointId.HipLeft] = 1.0f;
        boneScaler[JointId.KneeLeft] = 1.0f;
        boneScaler[JointId.AnkleLeft] = 1.0f;
        boneScaler[JointId.FootLeft] = 1.0f;
        boneScaler[JointId.HipRight] = 1.0f;
        boneScaler[JointId.KneeRight] = 1.0f;
        boneScaler[JointId.AnkleRight] = 1.0f;
        boneScaler[JointId.FootRight] = 1.0f;
        boneScaler[JointId.Head] = 1.0f;
        boneScaler[JointId.Nose] = 1.0f;
        boneScaler[JointId.EyeLeft] = 1.0f;
        boneScaler[JointId.EarLeft] = 1.0f;
        boneScaler[JointId.EyeRight] = 1.0f;
        boneScaler[JointId.EarRight] = 1.0f;
    }
    #endregion

    public bool CalibrationComplete(BackgroundDataNoDepth frame)
    {
        BackgroundDataNoDepth copyFrame = BackgroundDataNoDepth.DeepCopy(frame);
        for (int i = 0; i < (int)copyFrame.NumOfBodies; i++)
        {
            if (IsPrecise(copyFrame.Bodies[i])) liveBodyData.Add(copyFrame.Bodies[i]);
        }

        return liveBodyData.Count == maxListSize;
    }
    public List<BackgroundDataNoDepth> ScaleList(List<BackgroundDataNoDepth> animation)
    {
        if (animation.Count == 0)
        {
            Debug.Log("Animation list empty. Returning ");
            return animation;
        }
        List<Body> animationBodies = ExtractBodies(animation);

        Body animationAverageBody = CalculateRecordingJointAverage(animationBodies);

        CalculateBoneScale(animationAverageBody);

        return animation.Select(frame => ScaleFrames(frame)).ToList();
    }

    public BackgroundDataNoDepth ScaleFrames(BackgroundDataNoDepth frame)
    {
        int closestBody = findClosestTrackedBody(frame);
        frame.Bodies[closestBody] = ScaleSkeletonCoordinates(frame.Bodies[closestBody]);
        return frame;
    }

    public Body ScaleSkeletonCoordinates(Body skeleton)
    {
        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
                skeleton.JointPositions3D[jointNum].X *= boneScaler[(JointId)jointNum];
                skeleton.JointPositions3D[jointNum].Y *= boneScaler[(JointId)jointNum];
                skeleton.JointPositions3D[jointNum].Z *= boneScaler[(JointId)jointNum];

                skeleton.JointPositions2D[jointNum].X *= boneScaler[(JointId)jointNum];
                skeleton.JointPositions2D[jointNum].Y *= boneScaler[(JointId)jointNum];

            Debug.Log("BoneScale at " + (JointId)jointNum + ": " + boneScaler[(JointId)jointNum]);
        }
        return skeleton;
    }

    public void CalculateLiveJointAverage()
    {
        if (liveBodyData.Count == 0)
        {
            Debug.Log("List is empty. Average calculation impossible"); 
            return;
        }

        //Debug.Log(liveBodyData.Count);
        averageLiveBody = Body.DeepCopy(liveBodyData[0]);
        int maxJointsLength = averageLiveBody.Length;

        foreach (Body body in liveBodyData.Skip(1))
        {
            for (int i = 0; i < maxJointsLength; i++)
            {
                // Rotation and precision not important at this point
                averageLiveBody.JointPositions2D[i] += body.JointPositions2D[i];
                averageLiveBody.JointPositions3D[i] += body.JointPositions3D[i];
            }
        }

        for (int i = 0; i < maxJointsLength; i++)
        {
            averageLiveBody.JointPositions2D[i] = averageLiveBody.JointPositions2D[i] / maxListSize;
            averageLiveBody.JointPositions3D[i] = averageLiveBody.JointPositions3D[i] / maxListSize;
        }
        //Debug.Log("Inital body: " + liveBodyData[0].JointPositions2D[0]);
        //Debug.Log("Average body: " + averageLiveBody.JointPositions2D[0]);
    }

    public Body CalculateRecordingJointAverage(List<Body> bodies)
    {
        if (bodies.Count == 0)
        {
            Debug.Log("Given body list is empty. Can't calculate the average");
            // Return averageLiveBody which will make the scale 1.
            return averageLiveBody;
        }

        Body averageBody = Body.DeepCopy(bodies[0]);
        int maxJointsLength = averageBody.Length;

        foreach (Body body in bodies.Skip(1))
        {
            for (int i = 0; i < maxJointsLength; i++)
            {
                // Rotation and precision not important at this point
                averageBody.JointPositions2D[i] += body.JointPositions2D[i];
                averageBody.JointPositions3D[i] += body.JointPositions3D[i];
            }
        }

        for (int i = 0; i < maxJointsLength; i++)
        {
            averageBody.JointPositions2D[i] = averageBody.JointPositions2D[i] / bodies.Count;
            averageBody.JointPositions3D[i] = averageBody.JointPositions3D[i] / bodies.Count;
        }
        Debug.Log("Inital body: " + bodies[0].JointPositions2D[0]);
        Debug.Log("Average body: " + averageBody.JointPositions2D[0]);
        return averageBody;
    }


    public void CalculateBoneScale(Body animSkeleton)
    {
        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            Vector3 liveJointPos = new Vector3(averageLiveBody.JointPositions3D[jointNum].X, averageLiveBody.JointPositions3D[jointNum].Y, averageLiveBody.JointPositions3D[jointNum].Z);
            Vector3 animJointPos = new Vector3(animSkeleton.JointPositions3D[jointNum].X, animSkeleton.JointPositions3D[jointNum].Y, animSkeleton.JointPositions3D[jointNum].Z);
            Debug.Log("child pos: " + animJointPos);

            Vector2 liveJointPos2 = new Vector2(averageLiveBody.JointPositions2D[jointNum].X, averageLiveBody.JointPositions2D[jointNum].Y);
            Vector2 animJointPos2 = new Vector2(animSkeleton.JointPositions2D[jointNum].X, animSkeleton.JointPositions2D[jointNum].Y);

            Vector3 liveParentPos;
            Vector3 animParentPos;

            if (parentJointMap[(JointId)jointNum] != JointId.Count)
            {
                liveParentPos = new Vector3(averageLiveBody.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                    averageLiveBody.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, averageLiveBody.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                animParentPos = new Vector3(animSkeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                    animSkeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, animSkeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                Debug.Log("parent pos: " + animParentPos);

                Vector2 liveParentPos2 = new Vector2(averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].X, averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].Y);
                Vector2 animParentPos2 = new Vector2(averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].X, averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].Y);

            }
            else
            {
                liveParentPos = transform.position;
                animParentPos = transform.position;
            }
                // Going to test getting scale for 3D only this time

                float liveBoneLength = Vector3.Distance(liveJointPos, liveParentPos);
                float animBoneLength = Vector3.Distance(animJointPos, animParentPos);

                Debug.Log("Live distance: " + liveBoneLength);
                Debug.Log("Anim distance: " + animBoneLength);

                float scalingFactor = liveBoneLength / animBoneLength;

                boneScaler[(JointId)jointNum] = scalingFactor;
        }
    }

    public List<Body> ExtractBodies(List<BackgroundDataNoDepth> frames)
    {
        // If frames empty return our own data, which will probably result in scale being one
        if (frames.Count == 0)
        {
            Debug.Log("Given animation list is empty. Can't extract bodies");
            return liveBodyData;
        }

        List<Body> bodies = new List<Body>();

        foreach (BackgroundDataNoDepth frame in frames)
        {
            int closestBody = findClosestTrackedBody(frame);
            bodies.Add(frame.Bodies[closestBody]);
        }

        return bodies;

    }

    // Borrowed function from TrackerHanlder.cs
    private int findClosestTrackedBody(BackgroundDataNoDepth trackerFrameData)
    {
        int closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            if (pelvisPos.magnitude < minDistanceFromKinect)
            {
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
        }
        return closestBody;
    }

    public bool IsPrecise(Body skeleton)
    {
        for(int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            Debug.Log((int)skeleton.JointPrecisions[jointNum]);

            if ((int)skeleton.JointPrecisions[jointNum] < 2) return false;
        }
        return true;
    }
}
