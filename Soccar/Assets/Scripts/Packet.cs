using UnityEngine;

// 값이 아닌 레퍼런스가 변경되어야 하므로 패킷은 클래스로 생성
public class Packet
{
    // 플레이어의 움직임
    public class PlayerMotion
    {
        public long Timestamp;
        public int PlayerIndex;
        public Vector3 Position;

        public PlayerMotion(int playerIndex)
        {
            Timestamp = 0;
            PlayerIndex = playerIndex;
            Position = new Vector3(0, 0, 0);
        }
    }
}
