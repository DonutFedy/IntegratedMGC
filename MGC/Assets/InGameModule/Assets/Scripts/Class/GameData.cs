using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
public class GameData
{
    //0// CONST DATA
    public const int iStringStartIdx = 2;
    public const int iStringAnswerSize = 31;
    public const int iStringNicknameSize = 31;
    public const int iStringChatSize = 91;
    public const int iReceiveIngameTimePacketStructSize = 6;
    public const int iReceivePointPacketStructSize = 18;
    //0//

    //1// GAME DATA
    public enum EnumGameType : byte
    {
        NONE,
        TWENTY,
        RELAY,
        BAN,
        CATCH
    }

    public enum EnumGameTwentyStructType : byte
    {
        NONE,
        RECEIVE_PLAYER_POSITION_PACKET,
        SEND_GAME_READY_PACKET,
        RECEIVE_POPUP_TIME_START_PACKET,
        RECEIVE_POPUP_TIME_END_PACKET,
        RECEIVE_EXAMINER_ANSWERS_PACKET,
        SEND_EXAMINER_SELECT_ANSWER_PACKET,
        RECEIVE_SELECT_END_PACKET,
        RECEIVE_INGAME_TIME_PACKET,
        SEND_QUESTION_PACKET,
        RECEIVE_QUESTION_PACKET,
        SEND_YES_OR_NO_PACKET,
        RECEIVE_YES_OR_NO_PACKET,
        SEND_CHALLENGE_ANSWER_PACKET,
        RECEIVE_CHECK_ANSWER_PACKET,
        RECEIVE_NEXT_CHALLENGER_PACKET,
        RECEIVE_ROUND_END_PACKET,
        RECEIVE_SOCRE_PACKET,
        RECEIVE_ANSWER_POPUP_ANSWER_PACKET,
        RECEIVE_ANSWER_POPUP_TIME_START_PACKET,
        RECEIVE_GAME_END_PACKET,
        RECEIVE_EXAMINER_EXIT_PACKET,
        RECEIVE_NON_EXAMINER_EXIT_PACKET,
        SEND_RESERVE_EXIT_OR_CANCEL_PACKET,
        RECEIVE_EXIT_PACKET,
        RECEIVE_RESERVE_EXIT_OR_CANCEL_NICKNAME_PACEKT,
        RECEIVE_QUESTION_COUNT_PACKET
    }

    public enum EnumGameRelayStructType : byte
    {

    }

    public enum EnumGameBanStructType : byte
    {

    }

    public enum EnumGameCatchStructType : byte
    {
        NONE,
        RECEIVE_PLAYER_POSITION_PACKET,
        SEND_GAME_READY_PACKET,
        RECEIVE_POPUP_TIME_START_PACKET,
        RECEIVE_POPUP_TIME_END_PACKET,
        RECEIVE_EXAMINER_ANSWERS_PACKET,
        SEND_EXAMINER_SELECT_ANSWER_PACKET,
        RECEIVE_SELECT_END_PACKET,
        RECEIVE_INGAME_TIME_PACKET,
        SEND_POINT_PACKET,
        RECEIVE_POINT_PACKET,
        SEND_CHAT_PACKET,
        RECEIVE_CHAT_PACKET
    }
    
    public enum EnumPlayerState : byte
    {
        EXAMINER,
        CHALLENGER,
        AWAITER
    }

    public enum EnumPlayerColor : byte
    {
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        PURPLE
    }

    public enum EnumLineColor : byte
    {
        BLACK,
        WHITE,
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        PURPLE
    }
    //1//

    //2// SEND DATA
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 41)]
    public struct SendGameStartPacket
    {
        public byte byteGameType;
        public byte byteStructType;        
        public int iRoomNum;
        public int iPlayerColor;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayerID;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct SendGameReadyPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct SendGameAllReadyPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
    public struct SendExaminerSelectedAnswerPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 93)]
    public struct SendQuestionPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
    public struct SendChallengeAnswerPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct SendYesOrNoPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iYesOrNo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct SendReserveExitOrCancelPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iReserve;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
    public struct SendPointPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iLineIdx;
        public int iColor;
        public float fX;
        public float fY;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 93)]
    public struct SendChatPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
    }
    //2//

    //3// RECEIVE DATA
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceiveRoomMasterPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 161)]
    public struct ReceivePlayerPositionTwentyPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position5;
        public int iPlayerCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 216)]
    public struct ReceivePlayerPositionCatchPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position5;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayersID_Position6;
        public int iPlayersColor_Position1;
        public int iPlayersColor_Position2;
        public int iPlayersColor_Position3;
        public int iPlayersColor_Position4;
        public int iPlayersColor_Position5;
        public int iPlayersColor_Position6;
        public int iPlayerCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceivePopupTimeStartPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceivePopupTimeEndPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 157)]
    public struct ReceiveExaminerAnswersPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer5;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceiveSelectCompletePacket
    {
        public byte byteGameType;
        public byte byteStructType;        
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct ReceiveIngameTimePacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iIngameTime;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 124)]
    public struct ReceiveQuestionPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 68)]
    public struct ReceiveCheckAnswerPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strChat;
        public int iResult;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
    public struct ReceiveYesOrNoPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iYesOrNo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
    public struct ReceiveNextChallengerPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iChangeType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 68)]
    public struct ReceiveRoundEndPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNextExaminer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNextChallenger;
        public int iRound;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
    public struct ReceiveScorePacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iScore;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceiveAnswerPopupTimePacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 33)]
    public struct ReceiveAnswerPopupAnswerPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 177)]
    public struct ReceiveGameEndPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname5;
        public int iScore1;
        public int iScore2;
        public int iScore3;
        public int iScore4;
        public int iScore5;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
    public struct ReceiveExaminerExitPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iGameEnd;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 41)]
    public struct ReceiveNonExaminerExitPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iGameEnd;
        public int iChallenger;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct ReceiveExitPacket
    {
        public byte byteGameType;
        public byte byteStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 37)]
    public struct ReceiveReserveExitOrCancelNicknamePacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iReserve;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct ReceiveQuestionCountPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iQuestionCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 18)]
    public struct ReceivePointPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        public int iLineIdx;
        public int iColor;
        public float fX;
        public float fY;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 101)]
    public struct ReceiveChatPacket
    {
        public byte byteGameType;
        public byte byteStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
        public int iAnswer;
        public int iPlayerIdx;
    }
    //3//
}