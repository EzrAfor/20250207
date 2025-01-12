using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//协议封装与解析工具类
public  class PT 
{
    public static byte[] EncodeBody(PTBase ptBase) { 

        return Encoding.UTF8.GetBytes(JsonUtility.ToJson(ptBase));
    }

    public static PTBase DecodeBody(string protoName,byte[] bytes,int startIndex,int count)
    {

        return (PTBase)JsonUtility.FromJson(Encoding.UTF8.GetString(bytes, startIndex, count), Type.GetType(protoName));

    }

    public static byte[] EncodeName(PTBase ptBase) {
        byte[] nameBytes = Encoding.UTF8.GetBytes(ptBase.protoName);
        Int16 length = (Int16)nameBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian) {
            lengthBytes.Reverse();
        }
        byte[] bytes = lengthBytes.Concat(nameBytes).ToArray();
        return bytes;
    }

    public static string DecodeName(byte[] bytes, int startIndex,out int count) {
        count = 0;
        if (bytes.Length<2+startIndex) {
            return "";
        }
        Int16 length = (Int16)(bytes[startIndex] | bytes[startIndex + 1]<<8);
        
        if (startIndex + 2 + length > bytes.Length) {
            return "";
        }
        count = length+2;
        return Encoding.UTF8.GetString(bytes, startIndex + 2, length);


    }



}
