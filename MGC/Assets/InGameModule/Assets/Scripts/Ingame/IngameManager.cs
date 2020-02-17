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

    public GameObject m_goButtonAllReady, m_goNickNameInputField, m_goCanvas, m_goMainMenu, m_goButtonTwenty, m_goButtonCatch, m_goRadioGroup;
    public GameObject m_goPrefabGameTwenty, m_goPrefabGameRelay, m_goPrefabGameBan, m_goPrefabGameCatch;
    public Text m_textNickName;

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
        //Screen.SetResolution(1024, 768, false);

        var objs = FindObjectsOfType<IngameManager>();

        if(objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }

        //1// 소켓 생성 및 연결
        //m_client = new Client();
        //m_client.Init();
        //m_client.Connect(IP, PORT);
        //1//

        m_eventBuffer = EventBuffer.m_Instance;

        // event queue 검사 코루틴
        //StartCoroutine(CheckEventQueue());

        // errorMsg queue 검사 코루틴
        //StartCoroutine(CheckErrorMsgQueue());
    }

    private IEnumerator CheckEventQueue()
    {   
        while (true)
        {
            // 이벤트 큐에 있는 데이터를 가져온다.
            byte[] data = m_eventBuffer.GetData();

            //1// 가져온 데이터의 길이가 1이 아니라면 Event 호출하여 인게임 처리
            if (data.Length != 1)
            {
                Event(data);
            }
            //1//

            // 0.01초 간격
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator CheckErrorMsgQueue()
    {
        while (true)
        {
            // 에러메세지 큐에 있는 데이터를 가져온다.
            string strErrorMsg = m_eventBuffer.GetErrorMsg();

            //1// 가져온 메세지가 ""이 아니면 출력
            if (strErrorMsg != "")
            {
                Debug.Log(strErrorMsg);
            }
            //1//

            // 1초 간격
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int CheckPacketType(byte[] _baBuffer, int _iStartIdx)
    {
        if(_baBuffer.Length < 4)
        {
            Debug.Log("버퍼 크기가 4보다 작음");
            return -1;
        }

        byte[] bytes = new byte[4];
        int k = 0;

        for(int i = _iStartIdx; i < _iStartIdx + 4; ++i)
        {
            bytes[k] = _baBuffer[i];

            ++k;
        }

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt32(bytes, 0);
    }

    public Client GetClient()
    {
        return m_client;
    }

    public void Event(byte[] _baBuffer)
    {
        //1// 첫 4바이트를 이용해 game type 판별
        int iGameTypeStartIdx = 0;
        GameData.EnumGameType eGameType = (GameData.EnumGameType)CheckPacketType(_baBuffer, iGameTypeStartIdx);
        //1//

        //2// 다음 4바이트를 이용해 struct type 판별
        int iStructTypeStartIdx = 4;
        GameData.EnumGameTwentyStructType eGameTwentyStructType = new GameData.EnumGameTwentyStructType();
        GameData.EnumGameRelayStructType eGameRelayStructType = new GameData.EnumGameRelayStructType();
        GameData.EnumGameBanStructType eGameBanStructType = new GameData.EnumGameBanStructType();
        GameData.EnumGameCatchStructType eGameCatchStructType = new GameData.EnumGameCatchStructType();

        switch (eGameType)
        {
            case GameData.EnumGameType.TWENTY:
                eGameTwentyStructType = (GameData.EnumGameTwentyStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                break;

            case GameData.EnumGameType.RELAY:
                eGameRelayStructType = (GameData.EnumGameRelayStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                break;

            case GameData.EnumGameType.BAN:
                eGameBanStructType = (GameData.EnumGameBanStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
                break;

            case GameData.EnumGameType.CATCH:
                eGameCatchStructType = (GameData.EnumGameCatchStructType)CheckPacketType(_baBuffer, iStructTypeStartIdx);
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
                        case GameData.EnumGameTwentyStructType.RECEIVE_ROOM_MASTER_PACKET:
                            Debug.Log("Receive Game Twenty Room Master Packet");
                            m_goButtonAllReady.SetActive(true);
                            break;

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
                        case GameData.EnumGameCatchStructType.RECEIVE_ROOM_MASTER_PACKET:
                            Debug.Log("Receive Game Catch Room Master Packet");
                            m_goButtonAllReady.SetActive(true);
                            break;

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

    public void SendAllReady()
    {
        GameData.SendGameAllReadyPacket allReady = new GameData.SendGameAllReadyPacket();

        if (m_sendGameStartPacket.iGameType.Equals((int)GameData.EnumGameType.TWENTY))
        {
            allReady.iGameType = (int)GameData.EnumGameType.TWENTY;
            allReady.iStructType = (int)GameData.EnumGameTwentyStructType.SEND_GAME_ALL_READY_PACKET;
        }
        else if (m_sendGameStartPacket.iGameType.Equals((int)GameData.EnumGameType.CATCH))
        {
            allReady.iGameType = (int)GameData.EnumGameType.CATCH;
            allReady.iStructType = (int)GameData.EnumGameCatchStructType.SEND_GAME_ALL_READY_PACKET;
        }

        byte[] packet = Serializer.StructureToByte(allReady);

        m_client.SendPacket(packet);

        Debug.Log("Send All Ready Packet");

        m_goButtonAllReady.SetActive(false);
    }

    Color SetColor(int _iColorData)
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
                color = new Color(98 / 255, 27 / 255, 155 / 255);
                break;
        }

        return color;
    }

    public void SendGameTwentyReady()
    {
        m_strNickname = m_textNickName.text;

        //1// 메인 메뉴에서 스무고개 버튼 누르면 서버에 플레이어 정보 세팅
        m_sendGameStartPacket.iGameType = (int)GameData.EnumGameType.TWENTY;
        m_sendGameStartPacket.iStructType = (int)GameData.EnumGameTwentyStructType.SEND_GAME_START_PACKET;
        m_sendGameStartPacket.iRoomNum = 1;
        m_sendGameStartPacket.iPlayerColor = m_iPlayerColor;
        m_sendGameStartPacket.strPlayerID = m_strNickname;
        //1//

        //2// 서버에 보낼 패킷 Serializing 후 send
        byte[] packet = Serializer.StructureToByte(m_sendGameStartPacket);

        m_client.SendPacket(packet);
        //2//

        Debug.Log("Send Game Twenty Ready Packet");
                
        m_goNickNameInputField.SetActive(false);
        m_goRadioGroup.SetActive(false);
        m_goButtonTwenty.SetActive(false);
    }

    public void SendGameCatchReady()
    {
        m_strNickname = m_textNickName.text;

        //1// 메인 메뉴에서 스무고개 버튼 누르면 서버에 플레이어 정보 세팅
        m_sendGameStartPacket.iGameType = (int)GameData.EnumGameType.CATCH;
        m_sendGameStartPacket.iStructType = (int)GameData.EnumGameCatchStructType.SEND_GAME_START_PACKET;
        m_sendGameStartPacket.iRoomNum = 1;
        m_sendGameStartPacket.iPlayerColor = m_iPlayerColor;
        m_sendGameStartPacket.strPlayerID = m_strNickname;
        //1//

        //2// 서버에 보낼 패킷 Serializing 후 send
        byte[] packet = Serializer.StructureToByte(m_sendGameStartPacket);

        m_client.SendPacket(packet);
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
        m_sendGameReadyPacket.iGameType = (int)_eGameType;

        if(_eGameType.Equals(GameData.EnumGameType.TWENTY))
            m_sendGameReadyPacket.iStructType = (int)GameData.EnumGameTwentyStructType.SEND_GAME_READY_PACKET;
        else if(_eGameType.Equals(GameData.EnumGameType.CATCH))
            m_sendGameReadyPacket.iStructType = (int)GameData.EnumGameCatchStructType.SEND_GAME_READY_PACKET;
        //1//

        //2// 서버에 보낼 패킷 Serializing 후 send
        m_baSendGameReadyPacket = Serializer.StructureToByte(m_sendGameReadyPacket);

        m_client.SendPacket(m_baSendGameReadyPacket);
        //2//

        Debug.Log("Send Ingame Ready Packet");
    }

    private void LoadGameTwenty(byte[] _baBuffer)
    {
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

        m_miniGameTwenty.m_imgTempCharacter1.color = SetColor(playerPositionPacket.iPlayersColor_Position1);
        m_miniGameTwenty.m_imgTempCharacter2.color = SetColor(playerPositionPacket.iPlayersColor_Position2);
        m_miniGameTwenty.m_imgTempCharacter3.color = SetColor(playerPositionPacket.iPlayersColor_Position3);
        m_miniGameTwenty.m_imgTempCharacter4.color = SetColor(playerPositionPacket.iPlayersColor_Position4);
        m_miniGameTwenty.m_imgTempCharacter5.color = SetColor(playerPositionPacket.iPlayersColor_Position5);
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
        m_goMainMenu.SetActive(false);
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
        m_goMainMenu.SetActive(false);
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
        m_goMainMenu.SetActive(false);
        m_goGameRelay = Instantiate(m_goPrefabGameRelay, m_goCanvas.transform);
    }

    public void LoadGameBan()
    {
        m_goMainMenu.SetActive(false);
        m_goGameBan = Instantiate(m_goPrefabGameBan, m_goCanvas.transform);
    }

    public void LoadGameCatch()
    {
        m_goMainMenu.SetActive(false);
        m_goGameCatch = Instantiate(m_goPrefabGameCatch, m_goCanvas.transform);
    }

    public void ExitGame(GameData.EnumGameType _eGameType)
    {
        if (_eGameType.Equals(GameData.EnumGameType.TWENTY))
        {
            Destroy(m_goGameTwenty.gameObject);
            m_goMainMenu.SetActive(true);
        }
        else if (_eGameType.Equals(GameData.EnumGameType.CATCH))
        {
            Destroy(m_goGameCatch.gameObject);
            m_goMainMenu.SetActive(true);
        }
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