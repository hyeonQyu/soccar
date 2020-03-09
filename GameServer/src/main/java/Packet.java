public class Packet {
	public static final int TYPE_START_BUTTON_SEND = 0;
	public static final int TYPE_START_BUTTON_ACK = 1;
	public static final int TYPE_REQUEST_PLAYER_INDEX_SEND = 2;
	public static final int TYPE_REQUEST_PLAYER_INDEX_ACK = 3;
	public static final int TYPE_PLAYER_MOTION_SEND = 4;
	public static final int TYPE_PLAYER_MOTION_ACK = 5;

	static public class Header {
		public int MsgType;
		public int Length;

		public Header(int msgType) {
			this.MsgType = msgType;
			this.Length = 0;
		}
	}

	static public class StartButtonSend {
		public int CheckNum;
	}

	static public class StartButtonAck {
		public int CheckNum;
	}

	static public class RequestPlayerIndexSend {
		public int CheckNum;
	}

	static public class RequestPlayerIndexAck {
		public int PlayerIndex;
	}

	static public class PlayerMotionSend {
		public int PlayerIndex;
		public float X;
		public float Y;
		public float Z;
	}

	static public class PlayerMotionAck {
		public int PlayerIndex;
		public float X;
		public float Y;
		public float Z;

		public PlayerMotionAck(int playerIndex, int x, int y, int z) {
			PlayerIndex = playerIndex;
			X = x;
			Y = y;
			Z = z;
		}
	}
}