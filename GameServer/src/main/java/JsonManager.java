import java.io.IOException;

import com.fasterxml.jackson.core.JsonParseException;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;

public class JsonManager {
	
	private static ObjectMapper objectMapper = new ObjectMapper();
	
	// json문자열을 객체로 변환
	public static Object jsonToObject(String json, Class classType) throws JsonParseException, JsonMappingException, IOException {
		return objectMapper.readValue(json, classType);
	}
	
	// 패킷 json byte로 변환
	public static byte[] objectToByte(Object object) throws JsonProcessingException {
		System.out.println("BODY");
		return objectMapper.writeValueAsBytes(object);
	}
	
	// 헤더를 json byte로 변환(길이를 26으로 맞춤)
	public static byte[] objectToByte(Packet.Header header) throws JsonProcessingException {
		System.out.println("HEADER");
		String json = objectToJson(header);
		
		if(header.getMsgType() < 10)
			json += ' ';
		if(header.getLength() < 10)
			json += ' ';
		
		return json.getBytes();
	}
	
	// 객체를 json문자열로 변환
	private static String objectToJson(Object object) throws JsonProcessingException {
		return objectMapper.writeValueAsString(object);
	}

}
