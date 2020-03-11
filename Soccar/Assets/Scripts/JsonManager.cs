using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class JsonManager
{
    public static string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public static T JsonToObject<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static byte[] JsonToByte(string json)
    {
        string jsonCamel = RenameToCamelCase(json);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonCamel);
        return bytes;
    }

    public static string ByteToJson(byte[] bytes)
    {
        string json = Encoding.Default.GetString(bytes);
        string jsonPascal = RenameToPascalCase(json);
        return jsonPascal;
    }

    // 헤더의 json 길이를 26으로 맞춰주기 위함
    public static void SetHeaderLength(Packet.Header header, ref string jsonHeader)
    {
        if(header.MsgType < 10)
            jsonHeader += ' ';
        if(header.Length < 10)
            jsonHeader += ' ';
    }

    // C# 명명 규칙 -> JAVA 명명 규칙
    private static string RenameToCamelCase(string json)
    {
        string camelJson = "";

        int length = json.Length;

        for(int i = 0; i < length; i++)
        {
            char c = json[i];

            camelJson += c;

            if(json[i] == '\"')
            {
                if(json[i + 1] != ':')
                {
                    // 필드 변수 첫 글자를 소문자로
                    camelJson += char.ToLower(json[i + 1]);
                    i++;
                }
            }
        }

        return camelJson;
    }

    // JAVA 명명 규칙 -> C# 명명 규칙
    private static string RenameToPascalCase(string json)
    {
        string pascalJson = "";

        int length = json.Length;

        for(int i = 0; i < length; i++)
        {
            char c = json[i];

            if(c == ' ')
                continue;

            pascalJson += c;

            if(json[i] == '\"')
            {
                if(json[i + 1] != ':')
                {
                    // 필드 변수 첫 글자를 대문자로
                    pascalJson += char.ToUpper(json[i + 1]);
                    i++;
                }
            }
        }

        return pascalJson;
    }
}
