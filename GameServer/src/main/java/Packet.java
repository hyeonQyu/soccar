public class Packet {
	
	public static final int TYPE_START_BUTTON_SEND = 0;
	public static final int TYPE_START_BUTTON_ACK = 1;
	public static final int TYPE_REQUEST_PLAYER_INDEX_SEND = 2;
	public static final int TYPE_REQUEST_PLAYER_INDEX_ACK = 3;
	public static final int TYPE_PLAYER_MOTION_SEND = 4;
	public static final int TYPE_PLAYER_MOTION_ACK = 5;

	static public class Header {
		private int msgType;
		private int length;

		public Header(int msgType) {
			this.msgType = msgType;
			this.length = 0;
		}

		public void setLength(int length) {
			this.length = length;
		}

		public int getLength() {
			return length;
		}

		public int getMsgType() {
			return msgType;
		}
	}

	static public class StartButtonSend {
		private int checkNum;
		
		public int getCheckNum() {
			return checkNum;
		}
	}

	static public class StartButtonAck {
		private int checkNum;
		
		public void setCheckNum(int checkNum) {
			this.checkNum = checkNum;
		}
	}

	static public class RequestPlayerIndexSend {
		private int checkNum;
		
		public int getCheckNum() {
			return checkNum;
		}
	}

	static public class RequestPlayerIndexAck {
		private int playerIndex;
		
		public void setPlayerIndex(int playerIndex) {
			this.playerIndex = playerIndex;
		}
	}

	static public class PlayerMotionSend {
		private int playerIndex;
		private float x;
		private float y;
		private float z;
	}

	static public class PlayerMotionAck {
		private int playerIndex;
		private float x;
		private float y;
		private float z;

		public PlayerMotionAck(int playerIndex, int x, int y, int z) {
			this.playerIndex = playerIndex;
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
	
}