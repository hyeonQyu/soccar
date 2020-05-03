using UnityEngine;

// 값이 아닌 레퍼런스가 변경되어야 하므로 패킷은 클래스로 생성
public class Packet
{
    public class PlayersPosition
    {
        public Vector3[] Positions;

        public PlayersPosition()
        {
            Positions = new Vector3[PlayerController.Players.Length];
        }
    }

    // 플레이어의 움직임
    public class PersonalPosition
    {
        public long Timestamp;
        public int PlayerIndex;
        public Vector3 Position;

        public PersonalPosition(int playerIndex)
        {
            Timestamp = 0;
            PlayerIndex = playerIndex;
            Position = new Vector3(0, 0, 0);
        }
    }

    // 공의 위치
    public class BallsPosition
    {
        public Vector3[] Positions;
    }
}
