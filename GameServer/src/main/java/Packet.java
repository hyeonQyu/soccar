public class Packet {
	
	public static final int LENGTH_HEADER = 26;
	public static final int TYPE_START_BUTTON_SEND = 0;
	public static final int TYPE_START_BUTTON_ACK = 1;
	public static final int TYPE_REQUEST_PLAYER_INDEX_SEND = 2;
	public static final int TYPE_REQUEST_PLAYER_INDEX_ACK = 3;
	public static final int TYPE_PLAYER_MOTION_SEND = 4;
	public static final int TYPE_PLAYER_MOTION_ACK = 5;

	static public class Header {
		private int msgType;
		private int length;
		
		public Header() {}

		public Header(int msgType) {
			this.msgType = msgType;
			this.length = 0;
		}
		
		public void setMsgType(int msgType) {
			this.msgType = msgType;
		}

		public void setLength(int length) {
			this.length = length;
		}
		
		public int getMsgType() {
			return msgType;
		}

		public int getLength() {
			return length;
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
		
		public int getCheckNum() {
			return checkNum;
		}
		
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
		
		public int getPlayerIndex() {
			return playerIndex;
		}
		
		public void setPlayerIndex(int playerIndex) {
			this.playerIndex = playerIndex;
		}
	}

	static public class PlayerMotionSend {
		private int playerIndex;
		private float x;
		private float y;
		private float z;
		
		public int getPlayerIndex() {
			return playerIndex;
		}
		
		public float getX() {
			return x;
		}
		
		public float getY() {
			return y;
		}
		
		public float getZ() {
			return z;
		}
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
		
		public int getPlayerIndex() {
			return playerIndex;
		}
		
		public float getX() {
			return x;
		}
		
		public float getY() {
			return y;
		}
		
		public float getZ() {
			return z;
		}
	}
	
}