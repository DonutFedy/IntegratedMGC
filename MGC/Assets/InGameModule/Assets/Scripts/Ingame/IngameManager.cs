using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class IngameManager : MonoBehaviour
{
    private readonly string IP = "10.255.252.89";
    //private readonly string IP = "127.0.0.1";
    private readonly int PORT = 50;

    private int m_iPlayerColor;
    private string m_strNickname;
    private Client m_client;
    private EventBuffer m_eventBuffer;
    private GameObject m_goGameTwenty, m_goGameRelay, m_goGameBan, m_goGameCatch;
    private MiniGameTwenty m_miniGameTwenty;
    private MiniGameCatch m_miniGameCatch;
    private GameData.SendGameStartPacket m_sendGameStartPacket;
    private GameData.SendGameReadyPacket m_sendGameReadyPacket;
    private byte[] m_baSendGameReadyPacket;

    public GameObject m_goButtonAllReady, m_goNickNameInputField, m_goCanvas, m_goButtonTwenty, m_goButtonCatch, m_goRadioGroup;
    public GameObject m_goPrefabGameTwenty, m_goPrefabGameRelay, m_goPrefabGameBan, m_goPrefabGameCatch;

    //1// 싱글톤
    private static IngameManager m_instance;
    public static IngameManager m_Instance
    {
        get
        {
            if(m_instance == null)
            {
                var obj = FindObjectOfType<IngameManager>();

                if(obj != null)
                {
                    m_instance = obj;
                }
                else
                {
                    var newIngameManager = new GameObject("IngameManager").AddComponent<IngameManager>();
                    m_instance = newIngameManager;
                }
            }

            return m_instance;
        }

        private set
        {
            m_instance = value;
        }
    }
    //1//

    private void Awake()
    {
        // 스크린 사이즈 고정(임시)

        var objs = FindObjectsOfType<IngameManager>();

        if(objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }

        m_eventBuffer = EventBuffer.m_Instance;
        
    }

    // GameType, StructType 4byte(int)로 판별했을 때 사용
    //private int CheckPacketType(byte[] _baBuffer, int _iStartIdx)
    //{
    //    if(_baBuffer.Length < 4)
    //    {
    //        Debug.Log("버퍼 크기가 4보다 작음");
    //        return -1;
    //    }

    //    byte[] bytes = new byte[4];
    //    int k = 0;

    //    for(int i = _iStartIdx; i < _iStartIdx + 4; ++i)
    //    {
    //        bytes[k] = _baBuffer[i];

    //        ++k;
    //    }

    //    if (!BitConverter.IsLittleEndian)
    //        Array.Reverse(bytes);

    //    return BitConverter.ToInt32(bytes, 0);
    //}

    public Client GetClient()
    {
        return m_client;
    }

    public void Event(byte[] _baBuffer)
    {
        //1// 첫 1바이트를 이용해 game type 판별
        int iGameTypeStartIdx = 0;
        //GameData.EnumGameType eGameType = (GameData.EnumGameType)CheckPacketType(_baBuffer, iGameTypeStartIdx);
        GameData.EnumGameType eGameType = (GameData.EnumGameType)_baBuffer[iGameTypeStartIdx];
        //1//

        //2// 다음 1바이트를 이용해 struct type 판별
        int iStructTypeStartIdx = 1;
        GameData.EnumGameTwentyStructType eGameTwentyStructType = new GameData.EnumGameTwentyStructType();
        GameData.EnumGameRelayStructType eGameRelayStructType = new GameData.EnumGameRelayStructType();
        GameData.EnumGameBanStructType eGameBanStructType = new GameData.EnumGameBanStructType();
        GameData.EnumGameCatchStructType eGameCatchStructType = new GameData.EnumGameCatchStructType();

        switch (eGameType)
        {
            case GameData.EnumGameType.TWENTY:
                //eGameTwentyStructType = (GameData.EnumGameTwentyStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                eGameTwentyStructType = (GameData.EnumGameTwentyStructType)_baBuffer[iStructTypeStartIdx];
                break;

            case GameData.EnumGameType.RELAY:
                //eGameRelayStructType = (GameData.EnumGameRelayStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                eGameRelayStructType = (GameData.EnumGameRelayStructType)_baBuffer[iStructTypeStartIdx];
                break;

            case GameData.EnumGameType.BAN:
                //eGameBanStructType = (GameData.EnumGameBanStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                eGameBanStructType = (GameData.EnumGameBanStructType)_baBuffer[iStructTypeStartIdx];
                break;

            case GameData.EnumGameType.CATCH:
                //eGameCatchStructType = (GameData.EnumGameCatchStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                eGameCatchStructType = (GameData.EnumGameCatchStructType)_baBuffer[iStructTypeStartIdx];
                break;

            default:
                break;
        }        
        //2//

        switch(eGameType)
        {
            case GameData.EnumGameType.TWENTY:
                {
                    switch (eGameTwentyStructType)
                    {
                        case GameData.EnumGameTwentyStructType.RECEIVE_PLAYER_POSITION_PACKET:
                            Debug.Log("Receive Game Twenty Player Position Packet");
                            LoadGameTwenty(_baBuffer);
                            break;

                        case GameData.EnumGameTwentyStructType.RECEIVE_POPUP_TIME_START_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_POPUP_TIME_END_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_EXAMINER_ANSWERS_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_SELECT_END_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_INGAME_TIME_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_QUESTION_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_YES_OR_NO_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_CHECK_ANSWER_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_NEXT_CHALLENGER_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_ROUND_END_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_SOCRE_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_ANSWER_POPUP_ANSWER_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_ANSWER_POPUP_TIME_START_PACKET:                        
                        case GameData.EnumGameTwentyStructType.RECEIVE_GAME_END_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_EXAMINER_EXIT_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_NON_EXAMINER_EXIT_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_EXIT_PACKET:
                        case GameData.EnumGameTwentyStructType.RECEIVE_RESERVE_EXIT_OR_CANCEL_NICKNAME_PACEKT:
                        case GameData.EnumGameTwentyStructType.RECEIVE_QUESTION_COUNT_PACKET:
                            m_miniGameTwenty.Event(_baBuffer, eGameTwentyStructType);
                            break;
                            
                        default:
                            break;
                    }
                }
                break;

            case GameData.EnumGameType.RELAY:
                {
                    switch(eGameRelayStructType)
                    {
                        default:
                            break;
                    }
                }
                break;

            case GameData.EnumGameType.BAN:
                {
                    switch (eGameBanStructType)
                    {
                        default:
                            break;
                    }
                }
                break;

            case GameData.EnumGameType.CATCH:
                {
                    switch (eGameCatchStructType)
                    {
                        case GameData.EnumGameCatchStructType.RECEIVE_PLAYER_POSITION_PACKET:
                            Debug.Log("Receive Game Catch Player Position Packet");
                            LoadGameCatch(_baBuffer);
                            break;

                        case GameData.EnumGameCatchStructType.RECEIVE_POPUP_TIME_START_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_POPUP_TIME_END_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_EXAMINER_ANSWERS_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_SELECT_END_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_INGAME_TIME_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_POINT_PACKET:
                        case GameData.EnumGameCatchStructType.RECEIVE_CHAT_PACKET:
                            m_miniGameCatch.Event(_baBuffer, eGameCatchStructType);
                            break;

                        default:
                            break;
                    }
                }
                break;

            default:
                break;
        }
    }
    
    public void Error(string _strErrorMsg)
    {
        Debug.Log(_strErrorMsg);
    }

    
    public void SendPacket(byte[] packet)
    {
        GameManager.m_Instance.makePacket(packet);
    }


    private Color SetColor(int _iColorData)
    {
        Color color = new Color();

        switch (_iColorData)
        {
            case (int)GameData.EnumPlayerColor.RED:
                color = Color.red;
                break;

            case (int)GameData.EnumPlayerColor.ORANGE:
                color = new Color(1, 0.5f, 0, 1);
                break;

            case (int)GameData.EnumPlayerColor.YELLOW:
                color = Color.yellow;
                break;

            case (int)GameData.EnumPlayerColor.GREEN:
                color = Color.green;
                break;

            case (int)GameData.EnumPlayerColor.BLUE:
                color = Color.blue;
                break;

            case (int)GameData.EnumPlayerColor.PURPLE:

                float fR = (float)98 / 255;
                float fG = (float)27 / 255;
                float fB = (float)155 / 255;

                color = new Color(fR, fG, fB);
                break;
        }

        return color;
    }

    public void SendGameCatchReady()
    {

        //1// 메인 메뉴에서 스무고개 버튼 누르면 서버에 플레이어 정보 세팅
        m_sendGameStartPacket.byteGameType = (byte)GameData.EnumGameType.CATCH;
        m_sendGameStartPacket.iRoomNum = 1;
        m_sendGameStartPacket.iPlayerColor = m_iPlayerColor;
        m_sendGameStartPacket.strPlayerID = m_strNickname;
        //1//

        //2// 서버에 보낼 패킷 Serializing 후 send
        byte[] packet = Serializer.StructureToByte(m_sendGameStartPacket);

        SendPacket(packet);
        //2//

        Debug.Log("Send Game Catch Ready Packet");
                
        m_goNickNameInputField.SetActive(false);
        m_goRadioGroup.SetActive(false);
        m_goButtonCatch.SetActive(false);
    }

    public byte[] GetSendGameReadyPacket()
    {
        return m_baSendGameReadyPacket;
    }

    private void SendInGameReady(GameData.EnumGameType _eGameType)
    {
        //1// 게임 준비 완료 패킷 세팅
        m_sendGameReadyPacket.byteGameType = (byte)_eGameType;

        if(_eGameType.Equals(GameData.EnumGameType.TWENTY))
            m_sendGameReadyPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_GAME_READY_PACKET;
        else if(_eGameType.Equals(GameData.EnumGameType.CATCH))
            m_sendGameReadyPacket.byteStructType = (byte)GameData.EnumGameCatchStructType.SEND_GAME_READY_PACKET;
        //1//

        //2// 서버에 보낼 패킷 Serializing 후 send
        m_baSendGameReadyPacket = Serializer.StructureToByte(m_sendGameReadyPacket);

        SendPacket(m_baSendGameReadyPacket);
        //2//

        Debug.Log("Send Ingame Ready Packet");
    }

    private void LoadGameTwenty(byte[] _baBuffer)
    {
        m_strNickname = GameManager.m_Instance.getUserNickName();
        //1// 서버에서 받은 패킷 deserializing - 플레이어 위치 순서
        GameData.ReceivePlayerPositionTwentyPacket playerPositionPacket = new GameData.ReceivePlayerPositionTwentyPacket();
        playerPositionPacket = Serializer.ByteToStructure<GameData.ReceivePlayerPositionTwentyPacket>(_baBuffer);

        playerPositionPacket.strPlayersID_Position1 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position2 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position3 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 2), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position4 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 3), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position5 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 4), GameData.iStringNicknameSize);
        //1//

        // GameTwenty 오브젝트 생성(프리팹 불러오는 것)
        m_goGameTwenty = Instantiate(m_goPrefabGameTwenty, m_goCanvas.transform);

        // 기본 세팅이 완료되기 전까지 활성화 x
        m_goGameTwenty.SetActive(false);

        // MiniGameTwenty 컴포넌트 가져옴
        m_miniGameTwenty = m_goGameTwenty.GetComponent<MiniGameTwenty>();

        // 게임 타입 세팅
        m_miniGameTwenty.SetGameType(GameData.EnumGameType.TWENTY);

        // 플레이어 수 세팅
        m_miniGameTwenty.SetPlayerCount(playerPositionPacket.iPlayerCount);

        //2// 서버에서 받은 플레이어 순서 데이터로 플레이어 위치, 컬러 세팅 -> 컬러는 추후에 이미지로 변경될 예정
        m_miniGameTwenty.m_textNickname1.text = playerPositionPacket.strPlayersID_Position1;
        m_miniGameTwenty.m_textNickname2.text = playerPositionPacket.strPlayersID_Position2;
        m_miniGameTwenty.m_textNickname3.text = playerPositionPacket.strPlayersID_Position3;
        m_miniGameTwenty.m_textNickname4.text = playerPositionPacket.strPlayersID_Position4;
        m_miniGameTwenty.m_textNickname5.text = playerPositionPacket.strPlayersID_Position5;

        //2//

        // 출제자 표시 텍스트, 질문자 표시 텍스트 위치 세팅
        m_miniGameTwenty.SetExaminerAndChallengerTextPosition(playerPositionPacket.iPlayerCount);

        // 리스트에 플레이어 인포 저장
        m_miniGameTwenty.SetPlayerInfo(playerPositionPacket.iPlayerCount);
        
        //3// 받은 위치 정보를 통해 플레이어 역할 설정
        if (m_strNickname.Equals(playerPositionPacket.strPlayersID_Position1))
        {
            m_miniGameTwenty.SetPlayerState(GameData.EnumPlayerState.EXAMINER);
        }
        else if (m_strNickname.Equals(playerPositionPacket.strPlayersID_Position2))
        {
            m_miniGameTwenty.SetPlayerState(GameData.EnumPlayerState.CHALLENGER);
        }
        else
        {
            m_miniGameTwenty.SetPlayerState(GameData.EnumPlayerState.AWAITER);
        }
        
        m_miniGameTwenty.SetNickname(m_strNickname);
        //3//

        //4// 기본 세팅 완료되면 메인메뉴 닫고, GameTwenty 활성화
        m_goGameTwenty.SetActive(true);
        //4//
        
        // 게임 준비 완료 패킷 전송
        SendInGameReady(GameData.EnumGameType.TWENTY);
    }

    private void LoadGameCatch(byte[] _baBuffer)
    {
        //1// 서버에서 받은 패킷 deserializing - 플레이어 위치 순서
        GameData.ReceivePlayerPositionCatchPacket playerPositionPacket = new GameData.ReceivePlayerPositionCatchPacket();
        playerPositionPacket = Serializer.ByteToStructure<GameData.ReceivePlayerPositionCatchPacket>(_baBuffer);

        playerPositionPacket.strPlayersID_Position1 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position2 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position3 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 2), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position4 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 3), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position5 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 4), GameData.iStringNicknameSize);
        playerPositionPacket.strPlayersID_Position6 = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringNicknameSize * 5), GameData.iStringNicknameSize);
        //1//

        // GameTwenty 오브젝트 생성(프리팹 불러오는 것)
        m_goGameCatch = Instantiate(m_goPrefabGameCatch, m_goCanvas.transform);

        // 기본 세팅이 완료되기 전까지 활성화 x
        m_goGameCatch.SetActive(false);

        // MiniGameTwenty 컴포넌트 가져옴
        m_miniGameCatch = m_goGameCatch.GetComponent<MiniGameCatch>();

        // 게임 타입 세팅
        m_miniGameCatch.SetGameType(GameData.EnumGameType.CATCH);

        // 플레이어 수 세팅
        m_miniGameCatch.SetPlayerCount(playerPositionPacket.iPlayerCount);

        //2// 서버에서 받은 플레이어 순서 데이터로 플레이어 위치, 컬러 세팅 -> 컬러는 추후에 이미지로 변경될 예정
        m_miniGameCatch.m_textNickname1.text = playerPositionPacket.strPlayersID_Position1;
        m_miniGameCatch.m_textNickname2.text = playerPositionPacket.strPlayersID_Position2;
        m_miniGameCatch.m_textNickname3.text = playerPositionPacket.strPlayersID_Position3;
        m_miniGameCatch.m_textNickname4.text = playerPositionPacket.strPlayersID_Position4;
        m_miniGameCatch.m_textNickname5.text = playerPositionPacket.strPlayersID_Position5;
        m_miniGameCatch.m_textNickname6.text = playerPositionPacket.strPlayersID_Position6;

        m_miniGameCatch.m_imgTempCharacter1.color = SetColor(playerPositionPacket.iPlayersColor_Position1);
        m_miniGameCatch.m_imgTempCharacter2.color = SetColor(playerPositionPacket.iPlayersColor_Position2);
        m_miniGameCatch.m_imgTempCharacter3.color = SetColor(playerPositionPacket.iPlayersColor_Position3);
        m_miniGameCatch.m_imgTempCharacter4.color = SetColor(playerPositionPacket.iPlayersColor_Position4);
        m_miniGameCatch.m_imgTempCharacter5.color = SetColor(playerPositionPacket.iPlayersColor_Position5);
        m_miniGameCatch.m_imgTempCharacter6.color = SetColor(playerPositionPacket.iPlayersColor_Position6);
        //2//

        // 리스트에 플레이어 인포 저장
        m_miniGameCatch.SetPlayerInfo(playerPositionPacket.iPlayerCount);

        //3// 받은 위치 정보를 통해 플레이어 역할 설정
        if (m_strNickname.Equals(playerPositionPacket.strPlayersID_Position1))
        {
            m_miniGameCatch.SetPlayerState(GameData.EnumPlayerState.EXAMINER);
        }
        else
        {
            m_miniGameCatch.SetPlayerState(GameData.EnumPlayerState.CHALLENGER);
        }

        m_miniGameCatch.SetNickname(m_strNickname);
        //3//

        //4// 기본 세팅 완료되면 메인메뉴 닫고, GameCatch 활성화
        m_goGameCatch.SetActive(true);
        //4//

        // 게임 준비 완료 패킷 전송
        SendInGameReady(GameData.EnumGameType.CATCH);
    }

    public MiniGameTwenty GetMiniGameTwenty()
    {
        return m_miniGameTwenty;
    }

    public void LoadGameRelay()
    {
        m_goGameRelay = Instantiate(m_goPrefabGameRelay, m_goCanvas.transform);
    }

    public void LoadGameBan()
    {
        m_goGameBan = Instantiate(m_goPrefabGameBan, m_goCanvas.transform);
    }

    public void LoadGameCatch()
    {
        m_goGameCatch = Instantiate(m_goPrefabGameCatch, m_goCanvas.transform);
    }

    public void ExitGame(GameData.EnumGameType _eGameType)
    {
        if (_eGameType.Equals(GameData.EnumGameType.TWENTY))
        {
            Destroy(m_goGameTwenty.gameObject);
        }
        else if (_eGameType.Equals(GameData.EnumGameType.CATCH))
        {
            Destroy(m_goGameCatch.gameObject);
        }
        GameManager.m_Instance.endGame();
    }

    public void SetPlyaerColorRed()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.RED;
    }

    public void SetPlyaerColorOrange()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.ORANGE;
    }

    public void SetPlyaerColorYellow()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.YELLOW;
    }

    public void SetPlyaerColorGreen()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.GREEN;
    }

    public void SetPlyaerColorBlue()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.BLUE;
    }

    public void SetPlyaerColorPurple()
    {
        m_iPlayerColor = (int)GameData.EnumPlayerColor.PURPLE;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("종료");
    }
}