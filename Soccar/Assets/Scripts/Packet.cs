using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Packet
{
    public const int TypeStartButtonSend = 0;
    public const int TypeStartButtonAck = 1;
    public const int TypeRequestPlayerIndexSend = 2;
    public const int TypeRequestPlayerIndexAck = 3;
    public const int TypePlayerMotionSend = 4;
    public const int TypePlayerMotionAck = 5;

    [Serializable]
    public struct Header
    {
        public int MsgType;
        public int Length;

        public Header(int msgType)
        {
            MsgType = msgType;
            Length = 0;
        }
    }

    [Serializable]
    public struct StartButtonSend
    { 
        public int CheckNum;
    }
    [Serializable]
    public struct StartButtonAck
    {
        public int CheckNum;
    }

    [Serializable]
    public struct RequestPlayerIndexSend
    {
        public int CheckNum;
    }
    [Serializable]
    public struct RequestPlayerIndexAck
    {
        public int PlayerIndex;
    }

    [Serializable]
    public struct PlayerMotionSend
    {
        public int PlayerIndex;
        public float X;
        public float Y;
        public float Z;
    }
    [Serializable]
    public struct PlayerMotionAck
    {
        public int PlayerIndex;
        public float X;
        public float Y;
        public float Z;

        public PlayerMotionAck(int playerIndex, int x, int y, int z)
        {
            PlayerIndex = playerIndex;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
