using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRig : MonoBehaviour
{
    [Range(0, 1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    void LateUpdate()
    {
        // Update the body position based on the head position
        transform.position = head.ikTarget.position + headBodyPositionOffset;

        // Update the head rotation separately
        head.Map();

        // Update the hands
        leftHand.Map();
        rightHand.Map();
    }
}
