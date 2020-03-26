using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Packet
{
    // 플레이어의 움직임
    public struct PlayerMotion
    {
        public int PlayerIndex;
        public float X;
        public float Y;
        public float Z;

        public PlayerMotion(int playerIndex)
        {
            PlayerIndex = playerIndex;
            X = Y = Z = 0;
        }

        public void SetLocation(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
