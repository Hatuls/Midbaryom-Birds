using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Midbaryom.Inputs;

namespace Midbaryom.Core
{
    public class NewPosDetection : MonoBehaviour
    {
        ZedPoints zedpoints = new ZedPoints();

        float rightHandAngle, leftHandAngle;
        bool rightHandAbove, leftHandAbove;

        [SerializeField] private Player playerRef;



        MoveDir currentMoveDirection = MoveDir.None;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                PostureLeftDetected();
            }
            else if (Input.GetKey(KeyCode.C))
            {
                PostureRightDetected();
            }
            else if (Input.GetKey(KeyCode.X))
            {
                PostureHuntDetected();
            }
            else
            {
                playerRef.PlayerController.CustomResetCamRotation();
            }


            // from here on out - must detect a person to check.
            zedpoints = ZedInputHandler.GetPlayerPosition(0);

            if (zedpoints == null) return;

            CalculateHandAngles();

            if(!DetectLeft() && !DetectRight())
            {
                if(!DetectHunt())
                {
                    playerRef.PlayerController.CustomResetCamRotation();
                }
            }
            //if (!DetectLeft() && !DetectRight())
            //{
            //    DetectHunt();
            //}

        }


        private void CalculateHandAngles()
        {
            Vector3 rightShoulderToHand = zedpoints.RightHand - zedpoints.rightShoulder;
            Vector3 leftShoulderToHand = zedpoints.LeftHand - zedpoints.leftShoulder;

            float rightHandSine = rightShoulderToHand.y / rightShoulderToHand.x;
            float leftHandSine = leftShoulderToHand.y / leftShoulderToHand.x;
            //rightHandAngle = Mathf.Asin(rightHandSine) * Mathf.Rad2Deg;
            //leftHandeAngle = Mathf.Asin(leftHandSine) * Mathf.Rad2Deg;
            rightHandAngle = Vector3.Angle(rightShoulderToHand, Vector3.right);
            leftHandAngle = Vector3.Angle(leftShoulderToHand, Vector3.left);

            rightHandAbove = zedpoints.RightHand.y < zedpoints.rightShoulder.y;// + GameManager.Instance.distanceTemp;
            leftHandAbove = zedpoints.LeftHand.y < zedpoints.leftShoulder.y;// + GameManager.Instance.distanceTemp;

            if (!rightHandAbove) { rightHandAngle *= -1; }
            if (!leftHandAbove) { leftHandAngle *= -1; }

            if (GameManager.Instance.useDebugMessages)
            {
                GameManager.Instance.rightR.text = "Right H Angle: " + rightHandAngle.ToString();
                GameManager.Instance.leftR.text = "Left H Angle: " + leftHandAngle.ToString();
                //GameManager.Instance.rightR.text ="Right H Angle: " + rightHandAngle.ToString();
                //GameManager.Instance.leftR.text = "Left H Angle: " + leftHandeAngle.ToString();
                GameManager.Instance.midL.text = "ABS: " + Mathf.Abs(rightHandAngle + leftHandAngle).ToString();

                GameManager.Instance.rightL.text = "Right H above: " + rightHandAbove.ToString();
                GameManager.Instance.leftL.text = "Left H above: " + leftHandAbove.ToString();
                //GameManager.Instance.aboveR.text = handAbove == true ? "Above R: Yes" : "Above R: No";
            }
        }

        private bool DetectRight()
        {
            //bool handAbove = leftHand.z > leftShoulder.z + ZedPoseDetector.handZPosOffset;

            /*if (rightHandAngle > GameManager.Instance.moveRight_RightArmMin && 
                rightHandAngle < GameManager.Instance.moveRight_RightArmMax &&
                leftHandeAngle > GameManager.Instance.moveRight_LeftArmMin &&
                leftHandeAngle < GameManager.Instance.moveRight_LeftArmMax)*/
            if (rightHandAngle > GameManager.Instance.moveRight_RightArmMin &&
                rightHandAngle < GameManager.Instance.moveRight_RightArmMax)
            {
                PostureRightDetected();
                GameManager.Instance.midR.text = "Right Detected";
                return true;
            }

            return false;
        }

        public void PostureRightDetected()
        {
            if (playerRef.PlayerController._player == null) return;

            playerRef.PlayerController.CustomCamRotation(Vector3.right);

            Debug.Log("Moving Right");

            //if (currentMoveDirection == MoveDir.right) return;

            //currentMoveDirection = MoveDir.right;

            //SoundManager.Instance.CallPlaySound(sounds.MoveRightInGame);
        }

        private bool DetectLeft()
        {
            /*if (rightHandAngle > GameManager.Instance.moveLeft_RightArmMin &&
                rightHandAngle < GameManager.Instance.moveLeft_RightArmMax &&
                leftHandeAngle > GameManager.Instance.moveLeft_LeftArmMin &&
                leftHandeAngle < GameManager.Instance.moveLeft_LeftArmMax)*/
            if (leftHandAngle > GameManager.Instance.moveLeft_LeftArmMin &&
                leftHandAngle < GameManager.Instance.moveLeft_LeftArmMax)
            {
                PostureLeftDetected();
                GameManager.Instance.midR.text = "Left Detected";
                return true;
            }

            return false;
        }

        public void PostureLeftDetected()
        {
            if (playerRef.PlayerController._player == null) return;

            playerRef.PlayerController.CustomCamRotation(Vector3.left);

            Debug.Log("Moving Left");

            //if (currentMoveDirection == MoveDir.left) return;

            //currentMoveDirection = MoveDir.left;

            //SoundManager.Instance.CallPlaySound(sounds.MoveLeftInGame);

        }

        private bool DetectHunt()
        {
            if (rightHandAngle > GameManager.Instance.hunt_RightArmMin &&
                rightHandAngle < GameManager.Instance.hunt_RightArmMax &&
                leftHandAngle > GameManager.Instance.hunt_LeftArmMin &&
                leftHandAngle < GameManager.Instance.hunt_LeftArmMax)
            {
                PostureHuntDetected();
                GameManager.Instance.midR.text = "Hunt Detected";
                return true;
            }

            return false;
        }

        public void PostureHuntDetected()
        {
            Debug.Log("Hunt Detected");

            if (playerRef.PlayerController._player == null) return;


            playerRef.PlayerController.HuntDown();
        }
    }
}
