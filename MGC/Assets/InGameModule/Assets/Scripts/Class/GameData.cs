using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
public class GameData
{
    //0// CONST DATA
    public const int iStringStartIdx = 8;
    public const int iStringAnswerSize = 31;
    public const int iStringNicknameSize = 31;
    public const int iStringChatSize = 91;
    //0//

    //1// GAME DATA
    public enum EnumGameType
    {
        TWENTY,
        RELAY,
        BAN,
        CATCH
    }

    public enum EnumGameTwentyStructType : byte
    {
        NONE,
        SEND_GAME_START_PACKET,
        RECEIVE_ROOM_MASTER_PACKET,
        SEND_GAME_ALL_READY_PACKET,
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

    public enum EnumGameRelayStructType
    {

    }

    public enum EnumGameBanStructType
    {

    }

    public enum EnumGameCatchStructType
    {
        NONE,
        SEND_GAME_START_PACKET,
        RECEIVE_ROOM_MASTER_PACKET,
        SEND_GAME_ALL_READY_PACKET,
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
    
    public enum EnumPlayerState
    {
        EXAMINER,
        CHALLENGER,
        AWAITER
    }

    public enum EnumPlayerColor
    {
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
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 47)]
    public struct SendGameStartPacket
    {
        public int iGameType;
        public int iStructType;        
        public int iRoomNum;
        public int iPlayerColor;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strPlayerID;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct SendGameReadyPacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct SendGameAllReadyPacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
    public struct SendExaminerSelectedAnswerPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 99)]
    public struct SendQuestionPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
    public struct SendChallengeAnswerPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct SendYesOrNoPacket
    {
        public int iGameType;
        public int iStructType;
        public int iYesOrNo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct SendReserveExitOrCancelPacket
    {
        public int iGameType;
        public int iStructType;
        public int iReserve;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
    public struct SendPointPacket
    {
        public int iGameType;
        public int iStructType;
        public int iLineIdx;
        public int iColor;
        public float fX;
        public float fY;
    }
    //2//

    //3// RECEIVE DATA
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceiveRoomMasterPacket
    {
        public int iGameType;
        public int iStructType;
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 187)]
    public struct ReceivePlayerPositionTwentyPacket
    {
        public int iGameType;
        public int iStructType;
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
        public int iPlayersColor_Position1;
        public int iPlayersColor_Position2;
        public int iPlayersColor_Position3;
        public int iPlayersColor_Position4;
        public int iPlayersColor_Position5;
        public int iPlayerCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 222)]
    public struct ReceivePlayerPositionCatchPacket
    {
        public int iGameType;
        public int iStructType;
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
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceivePopupTimeStartPacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceivePopupTimeEndPacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 163)]
    public struct ReceiveExaminerAnswersPacket
    {
        public int iGameType;
        public int iStructType;
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
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceiveSelectCompletePacket
    {
        public int iGameType;
        public int iStructType;        
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct ReceiveIngameTimePacket
    {
        public int iGameType;
        public int iStructType;
        public int iIngameTime;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 130)]
    public struct ReceiveQuestionPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 91)]
        public string strChat;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 74)]
    public struct ReceiveCheckAnswerPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strChat;
        public int iResult;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
    public struct ReceiveYesOrNoPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iYesOrNo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
    public struct ReceiveNextChallengerPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iChangeType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 74)]
    public struct ReceiveRoundEndPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNextExaminer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNextChallenger;
        public int iRound;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
    public struct ReceiveScorePacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iScore;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceiveAnswerPopupTimePacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 39)]
    public struct ReceiveAnswerPopupAnswerPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strAnswer;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 183)]
    public struct ReceiveGameEndPacket
    {
        public int iGameType;
        public int iStructType;
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
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
    public struct ReceiveExaminerExitPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iGameEnd;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
    public struct ReceiveNonExaminerExitPacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iGameEnd;
        public int iChallenger;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct ReceiveExitPacket
    {
        public int iGameType;
        public int iStructType;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 43)]
    public struct ReceiveReserveExitOrCancelNicknamePacket
    {
        public int iGameType;
        public int iStructType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strNickname;
        public int iReserve;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct ReceiveQuestionCountPacket
    {
        public int iGameType;
        public int iStructType;
        public int iQuestionCount;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
    public struct ReceivePointPacket
    {
        public int iGameType;
        public int iStructType;
        public int iLineIdx;
        public int iColor;
        public float fX;
        public float fY;
    }
    //3//
}