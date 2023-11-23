using UnityEngine;

public class PlayerCharacterWeaponHands : MonoBehaviour
{
    #region INSPECTOR

    [Header("Left Hand"), SerializeField]
    private Transform leftHand;
    [SerializeField]
    private Vector3 leftHandPos;
    [SerializeField]
    private Vector3 leftHandRotation;

    [Header("Right Hand"), SerializeField]
    private Transform rightHand;
    [SerializeField]
    private Vector3 rightHandPos;
    [SerializeField]
    private Vector3 rightHandRotation;

    #endregion

    [ContextMenu("Update Transform")]
    private void Update()
    {
        if (leftHand)
        {
            leftHand.localPosition = leftHandPos;
            leftHand.localRotation = Quaternion.Euler(leftHandRotation);
        }

        if (rightHand)
        {
            rightHand.localPosition = rightHandPos;
            rightHand.localRotation = Quaternion.Euler(rightHandRotation);
        }
    }

    [ContextMenu("Snapshot Transform")]
    private void SnapshotTransform()
    {
        if (leftHand)
        {
            leftHandPos = leftHand.localPosition;
            leftHandRotation = leftHand.localRotation.eulerAngles;
        }

        if (rightHand)
        {
            rightHandPos = rightHand.localPosition;
            rightHandRotation = rightHand.localRotation.eulerAngles;
        }
    }
}
