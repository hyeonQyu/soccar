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
    public class SendingAbsolutePositions
    {
        public Vector3[] BallPositions;
        public int PlayerIndex;
        public Vector3 PlayerPosition;

        //public BallsPosition BallsPosition;
        //public PersonalPosition MyPosition;

        public SendingAbsolutePositions(int playerIndex)
        {
            BallPositions = new Vector3[2];
            PlayerIndex = playerIndex;
            PlayerPosition = new Vector3();
            //BallsPosition = new BallsPosition();
            //MyPosition = new PersonalPosition(playerIndex);
        }
    }

    // 공 + 플레이어 좌표 수신
    public class ReceivingPositions
    {
        public Vector3[] BallPositions;
        public Vector3[] PlayerPositions;

        //public BallsPosition BallsPosition;
        //public PlayersPosition PlayersPosition;

        public ReceivingPositions()
        {
            BallPositions = new Vector3[2];
            PlayerPositions = new Vector3[PlayerController.Players.Length];

            //BallsPosition = new BallsPosition();
            //PlayersPosition = new PlayersPosition();
        }
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
