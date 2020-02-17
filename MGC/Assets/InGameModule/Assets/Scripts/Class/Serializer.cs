using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

 class Serializer
 {
    public static byte[] StructureToByte(object obj)                // struct to byte[]
    {
        int datasize = Marshal.SizeOf(obj);                         // 구조체에 할당된 메모리의 크기 구함
        IntPtr buff = Marshal.AllocHGlobal(datasize);               // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당
        Marshal.StructureToPtr(obj, buff, false);                   // 할당된 구조체 객체의 주소를 구함
        byte[] data = new byte[datasize];                           // 구조체가 복사될 배열 선언
        Marshal.Copy(buff, data, 0, datasize);                      // 구조체 객체를 배열에 복사
        Marshal.FreeHGlobal(buff);                                  // 비관리 메모리 영역에 할당했던 메모리를 해제
        return data;                                                // 배열을 리턴
    }

    public static T ByteToStructure<T>(byte[] array)                // byte[] to struct
        where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(array, 0, ptr, size);
        var s = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return s;
    }

    public static string EncodingByteArrayToString(byte[] _baData, int _iStringStartIdx, int _iStringSize)
    {
        byte[] baBuffer = new byte[_iStringSize];
        int iStringStartIdx = _iStringStartIdx;
        int iBufferSize = 0;

        while(_baData[iStringStartIdx] != '\0')
        {
            baBuffer[iBufferSize] = _baData[iStringStartIdx];
            ++iStringStartIdx;
            ++iBufferSize;
        }

        Array.Resize(ref baBuffer, iBufferSize);

        string strNewString = Encoding.UTF8.GetString(baBuffer);

        return strNewString;
    }

    public static byte[] EncodingStringToByteArray(string strData, int iByteArraySize)
    {
        byte[] buffer = new byte[iByteArraySize];
        byte[] data = Encoding.UTF8.GetBytes(strData);
        
        for(int i = 0; i < data.Length; ++i)
        {
            buffer[i] = data[i];
        }

        return buffer;
    }

    public static char[] EncodingByteArrayToCharArray(byte[] data)
    {
        char[] buffer = Encoding.UTF8.GetChars(data);

        return buffer;
    }

    public static sbyte[] EncodingByteArrayToSByteArray(byte[] data)
    {
        sbyte[] buffer = new sbyte[data.Length];

        for(int i = 0; i < data.Length; ++i)
        {
            buffer[i] = Convert.ToSByte(data[i]);
        }

        return buffer;
    }
}