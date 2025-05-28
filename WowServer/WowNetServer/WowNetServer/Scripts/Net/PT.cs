using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Web.Script.Serialization;
//*****************************************
//创建人： Trigger 
//功能说明：协议封装与解析工具类
//***************************************** 
public class PT
{
    //json解码编码器
    private static JavaScriptSerializer jss = new JavaScriptSerializer();

    /// <summary>
    /// 编码协议体
    /// </summary>
    /// <param name="ptBase"></param>
    /// <returns></returns>
    public static byte[] EncodeBody(PTBase ptBase)
    {
        return Encoding.UTF8.GetBytes(jss.Serialize(ptBase));
    }
    /// <summary>
    /// 解码协议体
    /// </summary>
    /// <param name="protoName"></param>
    /// <param name="bytes"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static PTBase DecodeBody(string protoName,byte[] bytes,
        int startIndex,int count)
    {
        return (PTBase)jss.Deserialize(Encoding.UTF8.GetString(bytes, startIndex, count),
            Type.GetType(protoName));
    }
    /// <summary>
    /// 编码协议名
    /// </summary>
    /// <param name="ptBase"></param>
    /// <returns></returns>
    public static byte[] EncodeName(PTBase ptBase)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(ptBase.protoName);
        Int16 length = (Int16)nameBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian)
        {
            lengthBytes.Reverse();
        }
        byte[] bytes = lengthBytes.Concat(nameBytes).ToArray();
        return bytes;
    }
    /// <summary>
    /// 解码协议名
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static string DecodeName(byte[] bytes,int startIndex,out int count)
    {
        count = 0;
        if (bytes.Length<2+startIndex)
        {
            return "";
        }
        Int16 length = (Int16)(bytes[startIndex] | bytes[startIndex + 1] << 8);  
        if (startIndex+2+length>bytes.Length)
        {
            return "";
        }
        count = length+2;
        return Encoding.UTF8.GetString(bytes,startIndex+2,length);
    }
}
