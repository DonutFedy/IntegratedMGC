using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PACKET
{
    public class C_Buffer
    {
        const int           BUFSIZE = 1024;
        public byte[]       m_buf = new byte[BUFSIZE];
        public int          m_index = 0;
        public int          m_length = 0;

        void SetLength(int size)
        {
            m_length = size;
        }
        void Reset()
        {
            System.Array.Clear(m_buf, 0, BUFSIZE);
            m_index = 0;
            m_length = 0;
        }

        public C_Buffer(byte[] buf)
        {
            System.Array.Copy(m_buf, buf, buf.Length);
            m_length = buf.Length;
        }
        public C_Buffer(byte[] buf, int size)
        {
            System.Array.Resize<byte>(ref buf, size);
            System.Array.Copy(buf,m_buf,size);
            m_length = size;
        }
        public C_Buffer(C_Buffer rhs)
        {
            System.Array.Copy(m_buf, rhs.m_buf, rhs.m_length);
            m_index = rhs.m_index;
            m_length = rhs.m_length;
        }

        public C_Buffer()
        {
        }


        #region operator <<
        //1Byte
        public C_Buffer set(byte rhs)
        {
            m_buf[m_index] = System.Convert.ToByte(rhs);
            ++m_index;
            ++m_length;
            return this;
        }

        public C_Buffer set(bool rhs)
        {
            m_buf[m_index] = System.Convert.ToByte( rhs );
            ++m_index;
            ++m_length;
            return this;
        }
        // int
        public C_Buffer set(Int16 rhs)
        {
            byte[] data = BitConverter.GetBytes(rhs);
            for(int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            return this;
        }
        public C_Buffer set(Int32 rhs)
        {
            byte[] data = BitConverter.GetBytes(rhs);
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            return this;
        }
        public C_Buffer set(UInt32 rhs)
        {
            byte[] data = BitConverter.GetBytes(rhs);
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            return this;
        }
        public C_Buffer set(Int64 rhs)
        {
            byte[] data = BitConverter.GetBytes(rhs);
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            return this;
        }
        public C_Buffer set(ulong  rhs)
        {
            byte[] data = BitConverter.GetBytes(rhs);
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            return this;
        }
        //string
        public C_Buffer set(string rhs)
        {
            
            byte[] data = Encoding.UTF8.GetBytes(rhs);
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = data[i];
                ++m_index;
                ++m_length;
            }
            /* // ascii
            char[] data = rhs.ToCharArray();
            for (int i = 0; i < data.Length; ++i)
            {
                m_buf[m_index] = (byte)data[i];
                ++m_index;
                ++m_length;
            }
            */
            m_buf[m_index] = (byte)'\n';
            ++m_index;
            ++m_length;
            return this;
        }
        public C_Buffer set(byte[] rhs)
        {
            for (int i = 0; i < rhs.Length; ++i)
            {
                m_buf[m_index] = rhs[i];
                ++m_index;
                ++m_length;
            }
            m_buf[m_index] = (byte)'\n';
            ++m_index;
            ++m_length;
            return this;
        }
        public C_Buffer set(ErrorTypeEnterRoom rhs)
        {
            m_buf[m_index] = System.Convert.ToByte(rhs);
            ++m_index;
            ++m_length;
            return this;
        }
        public C_Buffer set(ErrorTypeAddFriend rhs)
        {
            m_buf[m_index] = System.Convert.ToByte(rhs);
            ++m_index;
            ++m_length;
            return this;
        }
        public C_Buffer set(ErrorTypeAcceptFriend rhs)
        {
            m_buf[m_index] = System.Convert.ToByte(rhs);
            ++m_index;
            ++m_length;
            return this;
        }
        public C_Buffer set(RoomGameType rhs)
        {
            m_buf[m_index] = System.Convert.ToByte(rhs);
            ++m_index;
            ++m_length;
            return this;
        }




        #endregion

        #region operator >>
        public C_Buffer get(ref bool type)
        {
            if (m_length <= m_index)
                return this;
            type = System.Convert.ToBoolean(m_buf[m_index]);
            ++m_index;
            return this;
        }
        public C_Buffer get(ref byte type)
        {
            if (m_length <= m_index)
                return this;
            type = m_buf[m_index];
            ++m_index;
            return this;
        }
        public C_Buffer get(ref string str)
        {
            /* // ascii
            if (m_length <= m_index)
                return this;
            
            while ('\n' != m_buf[m_index])
            {
                str += (char)m_buf[m_index];
                ++m_index;
            }
            */
            
            byte[] curByte = new byte[BUFSIZE];
            int curIndex = 0;
            while ('\n' != m_buf[m_index])
            {
                curByte[curIndex] = m_buf[m_index];
                ++m_index;
                ++curIndex;
            }
            Array.Resize<byte>(ref curByte, curIndex);
            str = Encoding.UTF8.GetString(curByte);
            
            ++m_index;
            return this;
        }
        public C_Buffer get(ref Int16 nInt)
        {
            if (m_length <= m_index)
                return this;
            nInt = 0;
            nInt = BitConverter.ToInt16(m_buf, m_index);
            m_index += sizeof(Int16);
            return this;
        }
        public C_Buffer get(ref Int32 nInt)
        {
            if (m_length <= m_index)
                return this;
            nInt = 0;
            nInt = BitConverter.ToInt32(m_buf, m_index);
            m_index += sizeof(Int32);
            return this;
        }
        public C_Buffer get(ref UInt32 nInt)
        {
            if (m_length <= m_index)
                return this;
            nInt = 0;
            nInt = BitConverter.ToUInt32(m_buf, m_index);
            m_index += sizeof(UInt32);
            return this;
        }
        public C_Buffer get(ref Int64 nInt)
        {
            if (m_length <= m_index)
                return this;
            nInt = 0;
            nInt = BitConverter.ToInt64(m_buf, m_index);
            m_index += sizeof(Int64);
            return this;
        }

        public C_Buffer get(ref ErrorTypeEnterRoom m_errorType)
        {
            if (m_length <= m_index)
                return this;
            m_errorType = (ErrorTypeEnterRoom)m_buf[m_index];
            ++m_index;
            return this;
        }
        public C_Buffer get(ref ErrorTypeAddFriend m_errorType)
        {
            if (m_length <= m_index)
                return this;
            m_errorType = (ErrorTypeAddFriend)m_buf[m_index];
            ++m_index;
            return this;
        }
        public C_Buffer get(ref ErrorTypeAcceptFriend m_errorType)
        {
            if (m_length <= m_index)
                return this;
            m_errorType = (ErrorTypeAcceptFriend)m_buf[m_index];
            ++m_index;
            return this;
        }
        public C_Buffer get(ref RoomGameType m_errorType)
        {
            if (m_length <= m_index)
                return this;
            m_errorType = (RoomGameType)m_buf[m_index];
            ++m_index;
            return this;
        }

        #endregion

    }


    public static class pakcetMaker
    {
        public static void setType(byte[] buf, int nType1, int nType2)
        {
            buf[0] = (byte)nType1;
            buf[1] = (byte)nType2;
        }

        public static C_BasePacket makePacket(C_Buffer buf)
        {
            C_BasePacket packet = null;

            byte type = 0;
            buf.get(ref type);
            switch (((BasePacketType)type))
            {
                case BasePacketType.basePacketTypeNone:
                    break;
                case BasePacketType.basePacketTypeLogin:
                    packet = makeLoginPacket(buf);
                    break;
                case BasePacketType.basePacketTypePreLoad:
                    packet = makePreLoadPacket(buf);
                    break;
                case BasePacketType.basePacketTypeGame:
                    packet = new C_InGamePacket();
                    break;
                case BasePacketType.basePacketTypeRoom:
                    packet = makeRoomPacket(buf);
                    break;
                case BasePacketType.basePacketTypeMarket:
                    break;
                case BasePacketType.basePacketTypeRanking:
                    break;
                case BasePacketType.basePacketTypeSocial:
                    packet = makeSocialPacket(buf);
                    break;
                case BasePacketType.basePacketTypeSize:
                    break;
            }
            if (packet == null)
                return null;
            packet.deserialize(buf);

            return packet;
        }

        static C_LoginPacket makeLoginPacket(C_Buffer buf)
        {
            C_LoginPacket packet = null;

            byte type = 0;
            buf.get(ref type);
            switch (((LoginPacketType)type))
            {
                case LoginPacketType.loginPacketTypeNone:
                    break;
                case LoginPacketType.loginPacketTypeLoginResponse:
                    packet = new C_LoginResponsePacket();
                    break;
                case LoginPacketType.loginPacketTypeLoginRequest:
                    packet = new C_LoginRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeLogoutRequest:
                    packet = new C_LogoutRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeSignupResponse:
                    packet = new C_SignupResponsePacket();
                    break;
                case LoginPacketType.loginPacketTypeSignupRequest:
                    packet = new C_SignupRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeDeleteResponse:
                    packet = new C_DeleteResponsePacket();
                    break;
                case LoginPacketType.loginPacketTypeDeleteRequest:
                    packet = new C_DeleteRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeShowChannelResponse:
                    packet = new C_ChannelListResponsePacket();
                    break;
                case LoginPacketType.loginPacketTypeShowChannelRequest:
                    packet = new C_ChannelListRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeChannelInResonse:
                    packet = new C_ChannelInResponsePacket();
                    break;
                case LoginPacketType.loginPacketTypeChannelInRequest:
                    packet = new C_ChannelInRequestPacket();
                    break;
                case LoginPacketType.loginPacketTypeSize:
                    break;
            }
            return packet;
        }


        static C_BasePreLoadPacket makePreLoadPacket(C_Buffer buf)
        {
            C_BasePreLoadPacket packet = null;

            byte type = 0;
            buf.get(ref type);
            switch ((PreLoadType)type)
            {
                case PreLoadType.preLoadPlayerInfo:
                    packet = new C_PreLoadPacketLoadPlayerInfo();
                    break;
            }

            return packet;
        }

        static C_BaseRoomPacket makeRoomPacket(C_Buffer buf)
        {
            C_BaseRoomPacket packet = null;

            byte type = 0;
            buf.get(ref type);
            switch (((RoomPacketType)type))
            {
                case RoomPacketType.roomPacketTypeMakeRoomResponse:
                    packet = new C_RoomPacketMakeRoomResponse();
                    break;
                case RoomPacketType.roomPacketTypeRoomListResponse:
                    packet = new C_RoomPacketRoomListResponse();
                    break;
                case RoomPacketType.roomPacketTypeEnterRoomResponse:
                    packet = new C_RoomPacketEnterRoomResponse();
                    break;
                case RoomPacketType.roomPacketTypeLeaveRoomResponse:
                    packet = new C_RoomPacketLeaveRoomResponse();
                    break;
                case RoomPacketType.packetTypeRoomRoomInfoResponse:
                    packet = new C_RoomPacketRoomInfoResponse();
                    break;
                case RoomPacketType.packetTypeRoomStartGameResponse:
                    packet = new C_RoomPacketStartGameResponse();
                    break;
                case RoomPacketType.roomPacketTypeCount:
                    break;
            }
            return packet;
        }


        static C_BaseSocialPacket makeSocialPacket(C_Buffer buf)
        {
            C_BaseSocialPacket packet = null;

            byte type = 0;
            buf.get(ref type);

            switch ((SocialPacketType)type)
            {
                case SocialPacketType.packetTypeSocialNone:
                    break;
                case SocialPacketType.packetTypeSocialChatNormalResponse:
                    packet = new C_SocialPacketChatNormalResponse();
                    break;
                case SocialPacketType.packetTypeSocialChatFriendResponse:
                    packet = new C_SocialPacketChatFriendResponse();
                    break;
                case SocialPacketType.packetTypeSocialChatGuildResponse:
                    break;
                case SocialPacketType.packetTypeSocialAddFriendResponse:
                    packet = new C_SocialPacketAddFriendResponse();
                    break;
                case SocialPacketType.packetTypeSocialDeleteFriendResponse:
                    packet = new C_SocialPacketDeleteFriendResponse();
                    break;
                case SocialPacketType.packetTypeSocialConfirmFriendResponse:
                    packet = new C_SocialPacketConfirmFriendResponse();
                    break;
                case SocialPacketType.packetTypeSocialAcceptFriendResponse:
                    packet = new C_SocialPacketAcceptFriendResponse();
                    break;
                case SocialPacketType.packetTypeSocialFriendListResponse:
                    packet = new C_SocialPacketFriendListResponse();
                    break;
                case SocialPacketType.packetTypeSocialCount:
                    break;
            }
            return packet;
        }
    }

}