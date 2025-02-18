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

    // Borrowed from TrackerHandler to see how the average skeleton renders.

    Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public Quaternion[] absoluteJointRotations = new Quaternion[(int)JointId.Count];

    #region joint mapping
    void Awake()
    {
        parentJointMap = new Dictionary<JointId, JointId>();

        parentJointMap[JointId.Pelvis] = JointId.Count;
        parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
        parentJointMap[JointId.SpineChest] = JointId.SpineNavel;
        parentJointMap[JointId.Neck] = JointId.SpineChest;
        parentJointMap[JointId.ClavicleLeft] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderLeft] = JointId.ClavicleLeft;
        parentJointMap[JointId.ElbowLeft] = JointId.ShoulderLeft;
        parentJointMap[JointId.WristLeft] = JointId.ElbowLeft;
        parentJointMap[JointId.HandLeft] = JointId.WristLeft;
        parentJointMap[JointId.HandTipLeft] = JointId.HandLeft;
        parentJointMap[JointId.ThumbLeft] = JointId.HandLeft;
        parentJointMap[JointId.ClavicleRight] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderRight] = JointId.ClavicleRight;
        parentJointMap[JointId.ElbowRight] = JointId.ShoulderRight;
        parentJointMap[JointId.WristRight] = JointId.ElbowRight;
        parentJointMap[JointId.HandRight] = JointId.WristRight;
        parentJointMap[JointId.HandTipRight] = JointId.HandRight;
        parentJointMap[JointId.ThumbRight] = JointId.HandRight;
        parentJointMap[JointId.HipLeft] = JointId.SpineNavel;
        parentJointMap[JointId.KneeLeft] = JointId.HipLeft;
        parentJointMap[JointId.AnkleLeft] = JointId.KneeLeft;
        parentJointMap[JointId.FootLeft] = JointId.AnkleLeft;
        parentJointMap[JointId.HipRight] = JointId.SpineNavel;
        parentJointMap[JointId.KneeRight] = JointId.HipRight;
        parentJointMap[JointId.AnkleRight] = JointId.KneeRight;
        parentJointMap[JointId.FootRight] = JointId.AnkleRight;
        parentJointMap[JointId.Head] = JointId.Pelvis;
        parentJointMap[JointId.Nose] = JointId.Head;
        parentJointMap[JointId.EyeLeft] = JointId.Head;
        parentJointMap[JointId.EarLeft] = JointId.Head;
        parentJointMap[JointId.EyeRight] = JointId.Head;
        parentJointMap[JointId.EarRight] = JointId.Head;

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

        //Body animationAverageBody = CalculateRecordingJointAverage(animationBodies);

        Body animationAverageBody = animationBodies[0];

        CalculateBoneScale(animationAverageBody);

        renderSkeleton(averageLiveBody, 0);

        return animation.Select(frame => ScaleFrame(frame)).ToList();
    }

    public BackgroundDataNoDepth ScaleFrame(BackgroundDataNoDepth oldFrame)
    {
        BackgroundDataNoDepth newFrame = BackgroundDataNoDepth.DeepCopy(oldFrame);
        int closestBody = findClosestTrackedBody(newFrame);
        newFrame.Bodies[closestBody] = ScaleSkeletonCoordinates(newFrame.Bodies[closestBody]);
        return newFrame;
    }

    public Body ScaleSkeletonCoordinates(Body skeleton)
    {
        //skeleton.JointPositions3D[(int)JointId.Pelvis].X *= boneScaler[JointId.Pelvis];
        //skeleton.JointPositions3D[(int)JointId.Pelvis].Y *= boneScaler[JointId.Pelvis];
        //skeleton.JointPositions3D[(int)JointId.Pelvis].Z *= boneScaler[JointId.Pelvis];

        for (int jointNum = 1; jointNum < (int)JointId.Count; jointNum++)
        {
            //skeleton.JointPositions3D[jointNum].X *= boneScaler[(JointId)jointNum];
            //skeleton.JointPositions3D[jointNum].Y *= boneScaler[(JointId)jointNum];
            //skeleton.JointPositions3D[jointNum].Z *= boneScaler[(JointId)jointNum];

            Vector3 initialJointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);

            Vector3 localPos;
            localPos = new Vector3(skeleton.JointPositions3D[0].X, skeleton.JointPositions3D[0].Y, skeleton.JointPositions3D[0].Z);

            //if (parentJointMap[(JointId)jointNum] != JointId.HandLeft && parentJointMap[(JointId)jointNum] != JointId.HandRight)
            //{
            //    // Pelvis as the local position
            //    localPos = new Vector3(skeleton.JointPositions3D[0].X, skeleton.JointPositions3D[0].Y, skeleton.JointPositions3D[0].Z);
            //}
            //else
            //{
            //    // Hand as the local position
            //    localPos = new Vector3(
            //        skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X, 
            //        skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, 
            //        skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z
            //        );
            //    //Debug.Log("Should be hand as the parentJoint: " + parentJointMap[(JointId)jointNum]);
            //}

            Vector3 relativePos = initialJointPos - localPos;

            Vector3 scaledPos;

            if ((parentJointMap[(JointId)jointNum] != JointId.HandLeft && parentJointMap[(JointId)jointNum] != JointId.HandRight))
            {
                scaledPos = relativePos * boneScaler[(JointId)jointNum];

            }
            else
            {
                scaledPos = relativePos * boneScaler[parentJointMap[(JointId)jointNum]];
            }

            Vector3 jointPos = scaledPos + localPos;


            skeleton.JointPositions3D[jointNum].X = jointPos.x;
            skeleton.JointPositions3D[jointNum].Y = jointPos.y;
            skeleton.JointPositions3D[jointNum].Z = jointPos.z;

            // Same for 2D

            Vector2 initialJointPos2 = new Vector2(skeleton.JointPositions2D[jointNum].X, skeleton.JointPositions2D[jointNum].Y);
            Vector2 localPos2 = new Vector2(skeleton.JointPositions2D[0].X, skeleton.JointPositions2D[0].Y);
            Vector2 relativePos2 = initialJointPos2 - localPos2;

            Vector2 scaledPos2;

            if ((parentJointMap[(JointId)jointNum] != JointId.HandLeft && parentJointMap[(JointId)jointNum] != JointId.HandRight))
            {
                scaledPos2 = relativePos2 * boneScaler[(JointId)jointNum];

            }
            else
            {
                scaledPos2 = relativePos2 * boneScaler[parentJointMap[(JointId)jointNum]];
            }

            Vector2 jointPos2 = scaledPos2 + localPos2;


            skeleton.JointPositions2D[jointNum].X = jointPos2.x;
            skeleton.JointPositions2D[jointNum].Y = jointPos2.y;
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
        //Debug.Log("Inital body: " + bodies[0].JointPositions2D[0]);
        //Debug.Log("Average body: " + averageBody.JointPositions2D[0]);
        return averageBody;
    }


    public void CalculateBoneScale(Body animSkeleton)
    {
        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            //Debug.Log((JointId)jointNum);
            Vector3 liveJointPos = new Vector3(averageLiveBody.JointPositions3D[jointNum].X, averageLiveBody.JointPositions3D[jointNum].Y, averageLiveBody.JointPositions3D[jointNum].Z);
            Vector3 animJointPos = new Vector3(animSkeleton.JointPositions3D[jointNum].X, animSkeleton.JointPositions3D[jointNum].Y, animSkeleton.JointPositions3D[jointNum].Z);

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

                Vector2 liveParentPos2 = new Vector2(averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].X, averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].Y);
                Vector2 animParentPos2 = new Vector2(averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].X, averageLiveBody.JointPositions2D[(int)parentJointMap[(JointId)jointNum]].Y);

            }
            else
            {
                liveParentPos = transform.position;
                animParentPos = transform.position;
            }

            //Debug.Log((JointId)jointNum + ": live child pos: " + liveJointPos);
            //Debug.Log((JointId)jointNum + ": live parent pos: " + liveParentPos);
            //Debug.Log((JointId)jointNum + ": anim child pos: " + animJointPos);
            //Debug.Log((JointId)jointNum + ": anim parent pos: " + animParentPos);
            // Going to test getting scale for 3D only this time

            float liveBoneLength = Vector3.Distance(liveJointPos, liveParentPos);
            float animBoneLength = Vector3.Distance(animJointPos, animParentPos);

            //Debug.Log("Live distance: " + liveBoneLength);
            //Debug.Log("Anim distance: " + animBoneLength);

            float scalingFactor = liveBoneLength / animBoneLength;

            //Debug.Log("Scaling factor: " + scalingFactor);

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
            //Debug.Log((int)skeleton.JointPrecisions[jointNum]);

            if ((int)skeleton.JointPrecisions[jointNum] < 2) return false;
        }
        return true;
    }

    public void renderSkeleton(Body skeleton, int skeletonNumber)
    {
        for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
        {
            Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].X, -skeleton.JointPositions3D[jointNum].Y, skeleton.JointPositions3D[jointNum].Z);
            Vector3 offsetPosition = transform.rotation * jointPos;
            Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;
            Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y,
                skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);
            absoluteJointRotations[jointNum] = jointRot;
            // these are absolute body space because each joint has the body root for a parent in the scene graph
            transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
            transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

            const int boneChildNum = 0;
            if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count)
            {
                Vector3 parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].X,
                    -skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Y, skeleton.JointPositions3D[(int)parentJointMap[(JointId)jointNum]].Z);
                Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
            }
            else
            {
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
            }
        }
    }
}
