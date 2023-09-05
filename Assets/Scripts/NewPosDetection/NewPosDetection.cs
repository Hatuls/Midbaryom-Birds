using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Midbaryom.Inputs;

namespace Midbaryom.Core
{
    public class NewPosDetection : MonoBehaviour
    {
        ZedPoints zedpoints = new ZedPoints();

        float rightHandAngle, leftHandeAngle;
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

            rightHandAngle = Mathf.Sin(zedpoints.rightShoulder.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
            leftHandeAngle = Mathf.Sin(zedpoints.leftShoulder.y / leftShoulderToHand.x) * Mathf.Rad2Deg;

            rightHandAbove = zedpoints.RightHand.y < zedpoints.rightShoulder.y + GameManager.Instance.distanceTemp;
            leftHandAbove = zedpoints.LeftHand.y < zedpoints.leftShoulder.y + GameManager.Instance.distanceTemp;

            //if (zedpoints.rightShoulder != Vector2.zero && zedpoints.leftShoulder != Vector2.zero)
            //{
            //    Vector3 rightShoulderToHand = zedpoints.RightHand - zedpoints.rightShoulder;
            //    Vector3 leftShoulderToHand = zedpoints.LeftHand - zedpoints.leftShoulder;

            //    rightHandAngle = Mathf.Sin(zedpoints.rightShoulder.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
            //    leftHandeAngle = Mathf.Sin(zedpoints.leftShoulder.y / leftShoulderToHand.x) * Mathf.Rad2Deg;
            //}
            //else
            //{
            //    Vector3 rightShoulderToHand = zedpoints.RightHand - zedpoints.Head;
            //    Vector3 leftShoulderToHand = zedpoints.LeftHand - zedpoints.Head;

            //    rightHandAngle = Mathf.Sin(zedpoints.Head.y / rightShoulderToHand.x) * Mathf.Rad2Deg;
            //    leftHandeAngle = Mathf.Sin(zedpoints.Head.y / leftShoulderToHand.x) * Mathf.Rad2Deg;
            //}

            if (GameManager.Instance.useDebugMessages)
            {
                GameManager.Instance.rightR.text = "Right H Angle: " + rightHandAngle.ToString();
                GameManager.Instance.leftR.text = "Left H Angle: " + leftHandeAngle.ToString();
                GameManager.Instance.midL.text = "ABS: " + Mathf.Abs(rightHandAngle + leftHandeAngle).ToString();

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
                rightHandAngle < GameManager.Instance.moveRight_RightArmMax &&
                leftHandAbove && !rightHandAbove)
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
            if (leftHandeAngle > GameManager.Instance.moveLeft_LeftArmMin &&
                leftHandeAngle < GameManager.Instance.moveLeft_LeftArmMax &&
                rightHandAbove && ! leftHandAbove)
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
                leftHandeAngle > GameManager.Instance.hunt_LeftArmMin &&
                leftHandeAngle < GameManager.Instance.hunt_LeftArmMax &&
                !leftHandAbove && !rightHandAbove)
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
