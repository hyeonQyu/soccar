using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 값이 아닌 레퍼런스가 변경되어야 하므로 패킷은 클래스로 생성
public class Packet
{
    // 플레이어의 움직임
    public class PlayerMotion
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

        public void SetLocation(Vector3 movingPosition)
        {
            X = movingPosition.x;
            Y = movingPosition.y;
            Z = movingPosition.z;
        }
    }
}
