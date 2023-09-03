using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Midbaryom.Core
{
    public static class ZedInputHandler
    {
        public static ZedPoints[] CurrentPlayersPositions = new ZedPoints[7];

        public static void HandleInput(string data)
        {
            //Debug.Log(data);

            if (string.IsNullOrWhiteSpace(data))
            {
                Debug.LogWarning("Got empty data from Zed");
                return;
            }

            string[] splitData = data.Split('*');

            if (splitData.Length == 0)
            {
                Debug.LogWarningFormat("Got invalid data from Zed. Data is {0} ", data);
                return;
            }
            else
            {
                for (int i = 0; i < CurrentPlayersPositions.Length; i++)
                {
                    if (CurrentPlayersPositions[i] != null)
                        CurrentPlayersPositions[i].Detected = false;
                }
                string[] playersPositions = data.Split('*');
                foreach (string player in playersPositions)
                {
                    if (player.Length > 0)
                    {
                        ZedPoints currentPlayerPoints = ParsePlayerPoints(player);
                        CurrentPlayersPositions[currentPlayerPoints.PlayerID] = currentPlayerPoints;
                    }
                }
            }

        }

        public static Vector2 GetRightWristPos(int playerID)
        {
            Vector2 result = new Vector2();
            if (CurrentPlayersPositions is null || CurrentPlayersPositions.Length < playerID)
                return result;


            return CurrentPlayersPositions[playerID].RightHand;
        }

        public static bool IsPlayerDetected(int playerID)
        {
            if (CurrentPlayersPositions.Length == 0 || CurrentPlayersPositions[playerID] == null) return false;

            return CurrentPlayersPositions[playerID].Detected;
        }

        private static ZedPoints ParsePlayerPoints(string data)
        {
            string[] splitData = data.Split(',');
            if (splitData.Length == 0)
            {
                Debug.LogWarning("Got empty player data from Zed");
                return null;
            }
            //else if (splitData.Length != 9)
            //{
            //    Debug.LogWarningFormat("Got invalid player data from Zed. Data is {0}", data);
            //    return null;
            //}
            ZedPoints playerPoints = new ZedPoints();
            playerPoints.PlayerID = int.Parse(splitData[0]) + 0;
            playerPoints.Head = new Vector2(int.Parse(splitData[1]), int.Parse(splitData[2]));
            playerPoints.Pelvis = new Vector2(int.Parse(splitData[3]), int.Parse(splitData[4]));
            playerPoints.LeftHand = new Vector2(int.Parse(splitData[5]), int.Parse(splitData[6]));
            playerPoints.RightHand = new Vector2(int.Parse(splitData[7]), int.Parse(splitData[8]));

            if (splitData.Length > 10)
            {
                playerPoints.Neck = new Vector2(int.Parse(splitData[9]), int.Parse(splitData[10]));
            }

            if (splitData.Length > 12)
            {
                playerPoints.rightShoulder = new Vector2(int.Parse(splitData[11]), int.Parse(splitData[12]));
            }

            if (splitData.Length > 14)
            {
                playerPoints.leftShoulder = new Vector2(int.Parse(splitData[13]), int.Parse(splitData[14]));
            }

            playerPoints.Detected = true;
            return playerPoints;
        }

        public static ZedPoints GetPlayerPosition(int id)
        {
            return CurrentPlayersPositions[id];
        }

        public static void HandleNoInput()
        {
            foreach (var player in CurrentPlayersPositions)
            {
                if (player != null)
                    player.Detected = false;
            }
        }
    }

    public class ZedPoints
    {
        public int PlayerID;
        public Vector2 Pelvis;
        public Vector2 Head;
        public Vector2 LeftHand;
        public Vector2 RightHand;
        public Vector2 Neck;
        public Vector2 rightShoulder;
        public Vector2 leftShoulder;
        public bool Detected;

        public void Print()
        {
            Debug.LogFormat("Player {0}: Pelvis is {1} Head is {2} LeftHand is {3} RightHand is {4} Neck is {5}, RightShouler {6}, LeftShoulder {7}", PlayerID, Pelvis, Head, LeftHand, RightHand, Neck, rightShoulder, leftShoulder);
        }

        public void PrintOffsets()
        {
            Debug.LogFormat("Head X is {0}, Pelvis X is {1}, Offset is {2}", Head.x, Pelvis.x, Head.x - Pelvis.x);
        }
    }
}
