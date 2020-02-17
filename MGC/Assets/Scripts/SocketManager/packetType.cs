using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;



namespace PACKET
{

    public enum BasePacketType : byte
    {
        basePacketTypeNone = 0,
        // client to server packet
        basePacketTypeLogin,
        basePacketTypePreLoad,
        basePacketTypeRoom,
        basePacketTypeGame,
        basePacketTypeMarket,
        basePacketTypeRanking,
        basePacketTypeSocial,
        basePacketTypeSize,
    };
    
    public enum LoginPacketType : byte
    {
        loginPacketTypeNone = 0,

        loginPacketTypeLoginResponse, 
        loginPacketTypeLoginRequest,  // d

        loginPacketTypeLogoutRequest,  // d

        loginPacketTypeSignupResponse,
        loginPacketTypeSignupRequest,  // d

        loginPacketTypeDeleteResponse, 
        loginPacketTypeDeleteRequest,  // d

        loginPacketTypeShowChannelResponse, 
        loginPacketTypeShowChannelRequest,  // d

        loginPacketTypeChannelInResonse,
        loginPacketTypeChannelInRequest,    // d

        loginPacketTypeSize,
    };

    public enum PreLoadType : byte
    {
        packetTypeNone = 0,

        preLoadPlayerInfo,

        packetTypeCount,
    }

    public enum RoomPacketType : byte
    {
        roomPacketTypeNone = 0,

        roomPacketTypeMakeRoomRequest , // c->s
        roomPacketTypeMakeRoomResponse , // s -> c

        roomPacketTypeRoomListRequest, // c -> s
        roomPacketTypeRoomListResponse, // s->c

        roomPacketTypeEnterRoomRequest,  // c -> s
        roomPacketTypeEnterRoomResponse,  // s->c

        roomPacketTypeLeaveRoomRequest, // c->s
        roomPacketTypeLeaveRoomResponse, // s->c

        packetTypeRoomRoomInfoRequest, // c->s
        packetTypeRoomRoomInfoResponse, //GameServer -> Client Broadcast (It will be send when new player enter the room)

        packetTypeRoomToggleReadyRequest, //Client -> GameServer
        packetTypeRoomToggleReadyResponse, //GameServer -> Client (To tell)

        packetTypeRoomStartGameRequest,
        packetTypeRoomStartGameResponse,

        roomPacketTypeCount, // limit
    }

    public enum RoomGameType : byte
    {
        GameTypeNone = 0,

        [Description("스무고개")]
        GameTypeTwentyQuestion,
        [Description("릴레이 소설")]
        GameTypeRelayNovel,
        [Description("금칙어")]
        GameTypeBanKeyword,
        [Description("캐치 마인드")]
        GameTypeCatchMind,

        GameTypeCount,
    }

    public enum ErrorTypeStartGame : byte
    {
        errorTypeStartGameNone = 0,

        errorTypeStartGameReady, //Someone do not press ready buddon
        errorTypeStartGameNotHaveGame,
        errorTypeStartGameAlreadyStartGame,
        errorTypeStartGameRoomIsNotWaitingGame,

        errorTypeStartGameCount,
    }

    public enum SocialPacketType : byte
    {
        packetTypeSocialNone,

        //chat
        packetTypeSocialChatNormalRequest,
        packetTypeSocialChatNormalResponse,

        packetTypeSocialChatFriendRequest,
        packetTypeSocialChatFriendResponse,

        packetTypeSocialChatGuildRequest,
        packetTypeSocialChatGuildResponse,

        //Friend
        packetTypeSocialAddFriendRequest, //친추 요청
        packetTypeSocialAddFriendResponse,
        packetTypeSocialDeleteFriendRequest,
        packetTypeSocialDeleteFriendResponse,

        packetTypeSocialConfirmFriendRequest, //받은 친추 목록
        packetTypeSocialConfirmFriendResponse,

        packetTypeSocialAcceptFriendRequest, //친구 요청 수락
        packetTypeSocialAcceptFriendResponse,
        packetTypeSocialFriendListRequest, //친구 리스트
        packetTypeSocialFriendListResponse,

        packetTypeSocialCount,
    }


    public enum ErrorTypeAddFriend : byte
    {
        [Description("알수 없는 이유로\n친구 요청 실패")]
        none = 0,
        [Description("존재하지 않는 유저입니다")]
        notExistPlayer,
        [Description("더 이상 친구를 추가 할 수 없습니다.")]
        srcFriendListIsFull,
        [Description("대상의 친구 수가 꽉 찼습니다.")]
        destFriendListIsFull,
        [Description("대상의 친구 요청 수가 꽉 찼습니다.")]
        destFriendRequestListIsFull,
        [Description("이미 보낸 요청입니다.")]
        alreadySendRequest,
        [Description("이미 친구인 대상입니다.")]
        samePlayer,
        count,
    };


    public enum ErrorTypeAcceptFriend : byte
    {
        none = 0,

        srcFriendListIsFull,
        destFriendListIsFull,

        count,
    }


    public enum ErrorTypeEnterRoom : byte
    {
        errorTypeNone = 0,

        errorTypeNotExistRoom,
        errorTypeWrongPassword,
        errorTypeAlreadyIncluded,
        errorTypeGameStart,
        errorTypeMaxPlayer,
        errorTypeCanNotEnterRoom,
        errorTypePlayerLogout,

        errorTypeCount,
    };


    public enum AnomalyType : byte
    {
        loginServer_Reconnect,
        loginServer_Disconnect,
        mainServer_Reconnect,
        mainServer_Disconnect
    }
}
