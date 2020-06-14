using UnityEngine;

// 값이 아닌 레퍼런스가 변경되어야 하므로 패킷은 클래스로 생성
public class Packet
{
    // 수신
    public class PlayersPosition
    {
        public Vector3[] Positions;

        public PlayersPosition()
        {
            Positions = new Vector3[PlayerController.Players.Length];
        }
    }

    // 상대좌표 전송
    public class PersonalPosition
    {
        //public long Timestamp;
        public int PlayerIndex;
        public Vector3 Position;

        public PersonalPosition(int playerIndex)
        {
            //Timestamp = 0;
            PlayerIndex = playerIndex;
            Position = new Vector3(0, 0, 0);
        }
    }

    // 공의 위치
    public class BallsPosition
    {
        public Vector3[] Positions;

        public BallsPosition()
        {
            Positions = new Vector3[2];
        }
    }

    // 공 + 절대좌표 전송
    public class SendingTransform
    {
        public Vector3[] BallPositions;
        public Quaternion[] BallRotations;
        public int PlayerIndex;
        public Vector3 PlayerPosition;
        public Vector3 PlayerRotation;
        public int AnimHashCode;
        public float PlayerSpeed;
        public float ShootPower;

        public SendingTransform(int playerIndex, int ballCount)
        {
            BallPositions = new Vector3[ballCount];
            BallRotations = new Quaternion[ballCount];
            PlayerIndex = playerIndex;
            PlayerPosition = new Vector3();
            PlayerRotation = new Vector3();
            AnimHashCode = 0;
            PlayerSpeed = 0;
            ShootPower = 0;
        }
    }

    // 공 + 절대좌표 수신
    public class ReceivingTransform
    {
        public Vector3[] BallPositions;
        public Quaternion[] BallRotations;
        public Vector3[] PlayerPositions;
        public Vector3[] PlayerRotations;
        public int[] AnimHashCodes;
        public float[] PlayerSpeeds;
        public float[] ShootPowers;
    }

    public class SendingTackleEvent
    {
        public int PlayerIndex;
        public Vector3 PlayerPosition;

        public SendingTackleEvent(int playerIndex, Vector3 playerPosition)
        {
            PlayerIndex = playerIndex;
            PlayerPosition = playerPosition;
        }
    }

    public class ReceivingTackleEvent
    {
        public int PlayerIndex;
        public Vector3 PlayerPosition;
    }

    public class SendingCompleteLoading
    {
        public int PlayerIndex;
        public string PlayerName;

        public SendingCompleteLoading(int playerIndex, string playerName)
        {
            PlayerIndex = playerIndex;
            PlayerName = playerName;
        }
    }

    public class ReceivingKickOff
    {
        public string[] PlayerNames;
    }

    public class SendingScore
    {
        public int Scorer;
        public int Conceder;
        public byte IsFeverBall;

        public SendingScore(int scorer, int conceder, bool isFeverBall)
        {
            Scorer = scorer;
            Conceder = conceder;
            IsFeverBall = isFeverBall ? (byte)1 : (byte)0;
        }
    }

    public class ReceivingScore
    {
        public int[] ScoreBoard;
        public int Scorer;
        public int Conceder;
    }

    // 대기중인 방 정보 수신
    public class ReceivingRoomList
    {
        public int[] RoomKeys;
        public string[] RoomNames;
        public int[] Headcounts;

        public ReceivingRoomList()
        {
            RoomKeys = new int[RoomManager.MaxRoomCount];
            RoomNames = new string[RoomManager.MaxRoomCount];
            Headcounts = new int[RoomManager.MaxRoomCount];
        }
    }

    // 현재 입장한 방 정보 수신
    public class ReceivingRoomInfo
    {
        public int RoomKey;
        public string RoomName;
        public string[] PlayerKeys;
        public string[] PlayerNames;
    }

    // 방 생성
    public class SendingCreateRoom
    {
        public string RoomName;
        public string PlayerName;

        public SendingCreateRoom(string roomName, string playerName)
        {
            RoomName = roomName;
            PlayerName = playerName;
        }
    }

    // 방 입장 전송
    public class SendingEnterRoom
    {
        public int RoomKey;
        public string PlayerName;

        public SendingEnterRoom(int roomKey, string playerName)
        {
            RoomKey = roomKey;
            PlayerName = playerName;
        }
    }

    // 방 퇴장 전송
    public class SendingExitRoom
    {
        public int RoomKey;
        public string PlayerName;

        public SendingExitRoom(int roomKey, string playerName)
        {
            RoomKey = roomKey;
            PlayerName = playerName;
        }
    }

    // 채팅 수신
    public class ReceivingChat
    {
        public string PlayerKey;
        public string Message;
    }

    // 채팅 전송
    public class SendingChat
    {
        public string RoomKey;
        public string Message;

        public SendingChat(string roomKey, string message)
        {
            RoomKey = roomKey;
            Message = message;
        }
    }

    public class ReceivingGameStart 
    {
        public int Port;
        public int Headcount;
        public int PlayerIndex;
    }
}
