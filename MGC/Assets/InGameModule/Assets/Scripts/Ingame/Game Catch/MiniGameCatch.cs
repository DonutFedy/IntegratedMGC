using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MiniGameCatch : MiniGame
{
    private int m_iRecvPointCount;   // 임시
    private int m_iLineColor;
    private Line m_line = null;
    private List<Line> m_listLine;
    private List<bool> m_listIsChatBoxOn = new List<bool>();
    private List<float> m_listFChatBoxLimitTime = new List<float>();
    private bool m_bLastPointInSketchBook;
    private float m_fSketchBookWidthHalf, m_fSketchBookHeightHalf;
    private Vector2 m_vMousePos, m_vSketchBookPos, m_vPointOutOfSketchBook;
    private Camera m_mainCamera;
    private GameData.SendChatPacket m_sendChatPacket;

    public Text m_textSendPointCount, m_textRecvPointCount;     // 임시
    public List<GameObject> m_listGoChatBubble;
    public List<RectTransform> m_listRtChatBubbleBorder;
    public List<RectTransform> m_listRtChatBubbleField;
    public List<Text> m_listTextChatBubble;
    public GameObject m_goLinePrefab, m_goIngameTime, m_goRadioGroupColor, m_goEraseAllButton, m_goGiveUpButton, m_goTextExaminer;
    public RectTransform m_rtSketchBook;

    private void Start()
    {
        m_mainCamera = Camera.main;

        m_vSketchBookPos.x = m_rtSketchBook.position.x;
        m_vSketchBookPos.y = m_rtSketchBook.position.y;
        m_fSketchBookWidthHalf = m_rtSketchBook.rect.width / 2;
        m_fSketchBookHeightHalf = m_rtSketchBook.rect.height / 2;

        m_bStartPopupTime = false;
        m_bStartAnswerPopupTime = false;
        m_bStartChoiceAnswerPopupTime = false;
        m_bStartExaminerExitPopupTime = false;

        m_bLastPointInSketchBook = true;
        m_fLimitPopupTime = 3;
        m_iIngameLimitTime = 100;

        m_listLine = new List<Line>();
    }

    private void Update()
    {
        //temp//
        if (m_line == null)
            m_textSendPointCount.text = "send\n0";
        else
            m_textSendPointCount.text = "send\n" + m_line.m_iSendPointCount;

        m_textRecvPointCount.text = "recv\n" + m_iRecvPointCount;
        //temp//
        
        ////0// 라인 생성 -> 그릴 차례일 때만 활성화
        //m_vMousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    GameObject goLine = Instantiate(m_goLinePrefab, m_rtSketchBook.transform);
        //    m_line = goLine.GetComponent<Line>();
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    m_line = null;
        //}

        //if (m_line != null)
        //{
        //    if (Math.IsPointInRect(m_vMousePos, m_vSketchBookPos, m_fSketchBookWidthHalf, m_fSketchBookHeightHalf))
        //    {
        //        m_line.UpdateLine(m_vMousePos);
        //    }
        //    else
        //    {

        //        m_line = null;
        //    }
        //}
        ////0//
        ///

        //0// input field 채팅
        if (m_ifChatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SendChat();
                //Chat(m_ifChatBox.text);
            }
        }
        else
        {
            //1// input field 포커스 = false 이면 true로 변경
            if (!m_ifChatBox.isFocused)
            {
                m_ifChatBox.ActivateInputField();
            }
            //1//
        }
        //0//

        // 채팅 수 카운팅
        CountChat();

        // 입력된 채팅 3초 후에 사라짐
        ChatBoxOnAndOff();

        //1// 스케치북에 선 그리기 -> 라운드 시작 && 출제자일 때 활성화
        if (m_bRoundStart && m_ePlayerState.Equals(GameData.EnumPlayerState.EXAMINER))
        {
            m_vMousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Math.IsPointInRect(m_vMousePos, m_vSketchBookPos, m_fSketchBookWidthHalf, m_fSketchBookHeightHalf))
            {
                //1// 스케치북 안에서 마우스 누르면 라인 프리팹 생성
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject goLine = Instantiate(m_goLinePrefab, m_rtSketchBook.transform);
                    goLine.GetComponent<LineRenderer>().startColor = SetColor(m_iLineColor);
                    goLine.GetComponent<LineRenderer>().endColor = SetColor(m_iLineColor);
                    m_line = goLine.GetComponent<Line>();
                    
                    m_listLine.Add(m_line);
                }
                //1//

                //2// 마우스 누른 상태로
                if (Input.GetMouseButton(0))
                {
                    //1// 바깥에서 안으로 들어온 경우
                    if (m_bLastPointInSketchBook.Equals(false))
                    {
                        //1// 라인 프리팹 생성
                        GameObject goLine = Instantiate(m_goLinePrefab, m_rtSketchBook.transform);
                        goLine.GetComponent<LineRenderer>().startColor = SetColor(m_iLineColor);
                        goLine.GetComponent<LineRenderer>().endColor = SetColor(m_iLineColor);
                        m_line = goLine.GetComponent<Line>();
                        m_listLine.Add(m_line);
                        //1//

                        //2// 선분과 선분 교점 구해서 선 그림
                        DrawLineWithIntersection(m_vMousePos, m_listLine.Count - 1, m_iLineColor);
                        //2//
                    }
                    //1//

                    m_bLastPointInSketchBook = true;
                }
                //2//
            }
            else
            {
                //1// 바깥에서 마우스 누르면 m_bInSketchBook = false;
                if (Input.GetMouseButtonDown(0))
                {
                    m_bLastPointInSketchBook = false;
                }
                //1//

                //2// 마우스 누른 상태
                if (Input.GetMouseButton(0))
                {
                    m_vPointOutOfSketchBook = m_vMousePos;

                    //1// 안에서 바깥으로 나간 경우
                    if (m_bLastPointInSketchBook.Equals(true))
                    {
                        //1// 선분과 선분 교점 구해서 선 그림
                        DrawLineWithIntersection(m_listLine.Count - 1, m_iLineColor);
                        //1//

                        // 라인 null
                        m_line = null;
                    }
                    //1//

                    m_bLastPointInSketchBook = false;
                }
                //2//
            }

            //1// 마우스 버튼 떼면 라인 null
            if (Input.GetMouseButtonUp(0))
            {
                m_bLastPointInSketchBook = true;

                m_line = null;
            }
            //1//

            //2// 라인 생성되었으면 선 그림
            if (m_line != null)
            {
                m_line.UpdateLine(m_vMousePos, m_listLine.Count - 1, m_iLineColor);
            }
            //2//
        }
        //1//

        //2// 대기 팝업 타이머
        if (m_bStartPopupTime)
        {
            m_fLimitPopupTime -= Time.deltaTime;

            if (m_fLimitPopupTime < 0)
            {
                m_fLimitPopupTime = 0;
                m_bStartPopupTime = false;
            }

            // 올림하여 숫자 표시
            m_textPopupTime.text = "" + Mathf.Ceil(m_fLimitPopupTime);
        }
        //2//

        //3// 정답 선택 팝업 타이머
        if (m_bStartChoiceAnswerPopupTime)
        {
            m_fLimitPopupTime -= Time.deltaTime;

            if (m_fLimitPopupTime < 0)
            {
                m_fLimitPopupTime = 0;
                m_bStartChoiceAnswerPopupTime = false;
            }

            // 올림하여 숫자 표시
            m_textChoiceAnswerPopupTime.text = "" + Mathf.Ceil(m_fLimitPopupTime);
            m_textWaitChoiceAnswerPopupTime.text = "" + Mathf.Ceil(m_fLimitPopupTime);
        }
        //3//

        //4// 인게임 타이머, 남은 질문 횟수
        if (m_bRoundStart)
        {
            m_textIngameTime.text = "" + m_iIngameLimitTime;
        }
        //4//
    }

    public void Event(byte[] _baBuffer, GameData.EnumGameCatchStructType _eStructType)
    {
        switch (_eStructType)
        {
            case GameData.EnumGameCatchStructType.RECEIVE_POPUP_TIME_START_PACKET:
                {
                    Debug.Log("Receive Popup Time Start Packet");

                    //1// 서버에서 받은 패킷 deserializing - 모든 플레이어 준비 완료 시 "잠시 후 게임이 시작됩니다." -> 타이머 start
                    GameData.ReceivePopupTimeStartPacket timeStart = new GameData.ReceivePopupTimeStartPacket();
                    timeStart = Serializer.ByteToStructure<GameData.ReceivePopupTimeStartPacket>(_baBuffer);
                    //1//

                    StopAllPopupTime();
                    m_goPopup.SetActive(true);

                    StartTime();
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_POPUP_TIME_END_PACKET:
                {
                    Debug.Log("Receive Popup Time End Packet");

                    //1// 서버에서 받은 패킷 deserializing - "잠시 후 게임이 시작됩니다." -> 타이머 end
                    GameData.ReceivePopupTimeEndPacket timeEnd = new GameData.ReceivePopupTimeEndPacket();
                    timeEnd = Serializer.ByteToStructure<GameData.ReceivePopupTimeEndPacket>(_baBuffer);
                    //1//

                    // "잠시 후 게임이 시작됩니다." -> 팝업 내리고 게임 시작
                    EndTime();
                    ChoiceAnswer();
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_EXAMINER_ANSWERS_PACKET:
                {
                    Debug.Log("Receive Examiner Answer Packet");

                    //1// 서버에서 받은 패킷 deserializing - 출제자에게 5개의 정답 선택지 제공
                    GameData.ReceiveExaminerAnswersPacket examinerAnswers = new GameData.ReceiveExaminerAnswersPacket();
                    examinerAnswers = Serializer.ByteToStructure<GameData.ReceiveExaminerAnswersPacket>(_baBuffer);
                    //1//

                    //2// 문자열은 UTF8로 따로 인코딩
                    m_textAnswer1.text = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringAnswerSize);
                    m_textAnswer2.text = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + GameData.iStringAnswerSize, GameData.iStringAnswerSize);
                    m_textAnswer3.text = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringAnswerSize * 2), GameData.iStringAnswerSize);
                    m_textAnswer4.text = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringAnswerSize * 3), GameData.iStringAnswerSize);
                    m_textAnswer5.text = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + (GameData.iStringAnswerSize * 4), GameData.iStringAnswerSize);
                    //2//

                    m_strAnswer = m_textAnswer1.text;
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_SELECT_END_PACKET:
                {
                    Debug.Log("Receive Select End Packet");

                    //1// 서버에서 받은 패킷 deserializing - 출제자 선택 완료 신호
                    GameData.ReceiveSelectCompletePacket examinerAnswer = new GameData.ReceiveSelectCompletePacket();
                    examinerAnswer = Serializer.ByteToStructure<GameData.ReceiveSelectCompletePacket>(_baBuffer);
                    //1//

                    RoundStart();
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_INGAME_TIME_PACKET:
                {
                    Debug.Log("Receive Ingame Time Packet");

                    //1// 서버에서 받은 패킷 deserializing - 인게임 타이머
                    GameData.ReceiveIngameTimePacket ingameTime = new GameData.ReceiveIngameTimePacket();
                    ingameTime = Serializer.ByteToStructure<GameData.ReceiveIngameTimePacket>(_baBuffer);
                    //1//

                    m_iIngameLimitTime = ingameTime.iIngameTime;

                    if (_baBuffer.Length > GameData.iReceiveIngameTimePacketStructSize)
                    {
                        byte[] baNewArray = ResizeArray(_baBuffer, GameData.iReceiveIngameTimePacketStructSize);

                        IngameManager.m_Instance.Event(baNewArray);
                    }
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_POINT_PACKET:
                {
                    Debug.Log("Receive Point Packet");

                    //1// 서버에서 받은 패킷 deserializing - 출제자가 그린 점 정보
                    GameData.ReceivePointPacket point = new GameData.ReceivePointPacket();
                    point = Serializer.ByteToStructure<GameData.ReceivePointPacket>(_baBuffer);
                    //1//

                    Vector2 vPoint;
                    vPoint.x = point.fX;
                    vPoint.y = point.fY;

                    DrawLine(point.iLineIdx, vPoint, point.iColor);

                    ++m_iRecvPointCount;    // 임시

                    if (_baBuffer.Length > GameData.iReceivePointPacketStructSize)
                    {
                        byte[] baNewArray = ResizeArray(_baBuffer, GameData.iReceivePointPacketStructSize);

                        IngameManager.m_Instance.Event(baNewArray);
                    }
                }
                break;

            case GameData.EnumGameCatchStructType.RECEIVE_CHAT_PACKET:
                {
                    Debug.Log("Receive Chat Packet");

                    //1// 서버에서 받은 패킷 deserializing - 채팅
                    GameData.ReceiveChatPacket chat = new GameData.ReceiveChatPacket();
                    chat = Serializer.ByteToStructure<GameData.ReceiveChatPacket>(_baBuffer);
                    //1//

                    Chat(chat.strChat, chat.iPlayerIdx, chat.iAnswer);
                }
                break;

            default:
                break;
        }
    }

    private void SendChat()
    {
        m_sendChatPacket.byteGameType = (byte)m_eGameType;
        m_sendChatPacket.byteStructType = (byte)GameData.EnumGameCatchStructType.SEND_CHAT_PACKET;
        m_sendChatPacket.strChat = m_ifChatBox.text;

        byte[] packet = Serializer.StructureToByte(m_sendChatPacket);

        GameManager.m_Instance.makePacket(packet);

        m_ifChatBox.text = "";
        m_textChatBoxText.text = "";

        Debug.Log("Send Chat Packet");
    }

    private byte[] ResizeArray(byte[] _baBuffer, int _iSize)
    {
        byte[] baArray = new byte[_baBuffer.Length - _iSize];

        //_baBuffer.CopyTo(baArray, _iSize);
        Array.Copy(_baBuffer, _iSize, baArray, 0, _baBuffer.Length - _iSize);        

        return baArray;
    }

    private void Chat(string _strChat, int _iPlayerIdx, int _iAnswer)
    {
        //1// 텍스트 박스 크기를 텍스트가 얼마나 들어왔는지에 따라 판별한 후 텍스트 출력
        m_listTextChatBubble[_iPlayerIdx].text = _strChat;

        float fMaxWidth = 135f;             // 한글 8글자 기준 텍스트 박스 너비
        float fPreferredHeight = 21f;       // font size 20 기준 텍스트 박스 폭
        float fAddLengthForField = 5;       // 텍스트를 그려줄 필드(Image)
        float fAddLegthForBorder = 15;      // 필드를 감싸는 테두리

        if (m_listTextChatBubble[_iPlayerIdx].preferredWidth > fMaxWidth * 3f)                // 텍스트 박스 너비가 135 * 3를 넘겼다면 4줄로 처리
        {
            fPreferredHeight *= 4f;
            m_listTextChatBubble[_iPlayerIdx].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(fMaxWidth, fPreferredHeight);

            m_listRtChatBubbleField[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLengthForField, fPreferredHeight + fAddLengthForField);
            m_listRtChatBubbleBorder[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLegthForBorder, fPreferredHeight + fAddLegthForBorder);
        }
        else if (m_listTextChatBubble[0].preferredWidth > fMaxWidth * 2f)           // 텍스트 박스 너비가 135 * 2를 넘겼다면 3줄로 처리
        {
            fPreferredHeight *= 3f;
            m_listTextChatBubble[_iPlayerIdx].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(fMaxWidth, fPreferredHeight);

            m_listRtChatBubbleField[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLengthForField, fPreferredHeight + fAddLengthForField);
            m_listRtChatBubbleBorder[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLegthForBorder, fPreferredHeight + fAddLegthForBorder);
        }
        else if(m_listTextChatBubble[_iPlayerIdx].preferredWidth > fMaxWidth)                 // 텍스트 박스 너비가 135를 넘겼다면 2줄로 처리
        {
            fPreferredHeight *= 2f;
            m_listTextChatBubble[_iPlayerIdx].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(fMaxWidth, fPreferredHeight);

            m_listRtChatBubbleField[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLengthForField, fPreferredHeight + fAddLengthForField);
            m_listRtChatBubbleBorder[_iPlayerIdx].sizeDelta = new Vector2(fMaxWidth + fAddLegthForBorder, fPreferredHeight + fAddLegthForBorder);
        }
        else                                                                        // 텍스트 박스 너비가 135를 넘기지 않았다면 텍스트 크기만큼 박스 크기 고정
        {
            m_listTextChatBubble[_iPlayerIdx].gameObject.GetComponent<RectTransform>().sizeDelta =  new Vector2(m_listTextChatBubble[_iPlayerIdx].preferredWidth, fPreferredHeight);

            m_listRtChatBubbleField[_iPlayerIdx].sizeDelta = new Vector2(m_listTextChatBubble[_iPlayerIdx].preferredWidth + fAddLengthForField, fPreferredHeight + fAddLengthForField);
            m_listRtChatBubbleBorder[_iPlayerIdx].sizeDelta = new Vector2(m_listTextChatBubble[_iPlayerIdx].preferredWidth + fAddLegthForBorder, fPreferredHeight + fAddLegthForBorder);
        }

        m_listGoChatBubble[_iPlayerIdx].SetActive(true);

        m_listIsChatBoxOn[_iPlayerIdx] = true;
        m_listFChatBoxLimitTime[_iPlayerIdx] = 3f;
        //1//

        //2// 플레이어가 정답을 맞춘 경우
        //if(_iAnswer.Equals(1))
        //{

        //}
        //2//
    }

    private void ChatBoxOnAndOff()
    {
        for (int i = 0; i < m_iPlayerCnt; ++i)
        {
            if (m_listIsChatBoxOn[i])
            {
                m_listFChatBoxLimitTime[i] -= Time.deltaTime;

                if (m_listFChatBoxLimitTime[i] < 0)
                {
                    m_listIsChatBoxOn[i] = false;
                    m_listFChatBoxLimitTime[i] = 3f;
                }
            }
        }
    }

    private void DrawLine(int _iLineIdx, Vector2 _vPoint, int _iLineColor)
    {
        if (_iLineIdx > m_listLine.Count - 1)
        {
            GameObject goLine = Instantiate(m_goLinePrefab, m_rtSketchBook.transform);
            goLine.GetComponent<LineRenderer>().startColor = SetColor(_iLineColor);
            goLine.GetComponent<LineRenderer>().endColor = SetColor(_iLineColor);
            m_line = goLine.GetComponent<Line>();
            m_listLine.Add(m_line);

            Debug.Log("새로운 라인 생성, _iLineIdx : " + _iLineIdx + "_vPoint : " + _vPoint);

            m_iRecvPointCount = 0;
            m_line.SetPoint(_vPoint);
        }
        else
        {
            Debug.Log("기존 라인에 이음, _iLineIdx : " + _iLineIdx + "_vPoint : " + _vPoint);

            m_listLine[_iLineIdx].SetPoint(_vPoint);
        }
    }

    private void RoundStart()
    {
        m_bRoundStart = true;
        m_goTextExaminer.SetActive(true);
        m_goIngameTime.SetActive(true);
        m_textRound.gameObject.SetActive(true);
        m_goReserveExitOrCancel.SetActive(true);

        switch (m_ePlayerState)
        {
            case GameData.EnumPlayerState.EXAMINER:
                {
                    m_goRadioGroupAnswer.SetActive(false);

                    SendAnswer();

                    m_textAnswer.text = "제시어 : " + m_strAnswer;
                    m_goAnswerText.SetActive(true);
                    m_goRadioGroupColor.SetActive(true);
                    m_goEraseAllButton.SetActive(true);
                    m_goGiveUpButton.SetActive(true);
                }
                break;

            case GameData.EnumPlayerState.CHALLENGER:
                {
                    m_goWaitChoiceAnswerPopup.SetActive(false);

                    ShowChatBox();
                }
                break;

            default:
                break;
        }
    }

    private void ShowChatBox()
    {
        m_goChatCountBox.SetActive(true);
        m_goChatBox.SetActive(true);
    }

    private void ChoiceAnswer()
    {
        m_goPlayerExitPopup.SetActive(false);
        StopAllPopupTime();
        m_fLimitPopupTime = 5;
        m_bStartChoiceAnswerPopupTime = true;

        switch (m_ePlayerState)
        {
            case GameData.EnumPlayerState.EXAMINER:
                {
                    m_goRadioGroupAnswer.SetActive(true);
                }
                break;

            case GameData.EnumPlayerState.CHALLENGER:
                {
                    m_goWaitChoiceAnswerPopup.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

    // 안에서 밖으로 나갈 때 교점 연산 함수 호출
    private void DrawLineWithIntersection(int _iIdx, int _iLineColor)
    {
        if (m_vPointOutOfSketchBook.x > m_vSketchBookPos.x + m_fSketchBookWidthHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _iIdx, _iLineColor);
        }
        else if (m_vPointOutOfSketchBook.x < m_vSketchBookPos.x - m_fSketchBookWidthHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _iIdx, _iLineColor);
        }

        if (m_vPointOutOfSketchBook.y > m_vSketchBookPos.y + m_fSketchBookHeightHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _iIdx, _iLineColor);
        }
        else if (m_vPointOutOfSketchBook.y < m_vSketchBookPos.y - m_fSketchBookHeightHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _iIdx, _iLineColor);
        }
    }

    // 밖에서 안으로 들어올 때 교점 연산 함수 호출
    private void DrawLineWithIntersection(Vector2 _vPointInSketchBook, int _iIdx, int _iLineColor)
    {
        if (m_vPointOutOfSketchBook.x > m_vSketchBookPos.x + m_fSketchBookWidthHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _vPointInSketchBook, _iIdx, _iLineColor);
        }
        else if (m_vPointOutOfSketchBook.x < m_vSketchBookPos.x - m_fSketchBookWidthHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _vPointInSketchBook, _iIdx, _iLineColor);
        }

        if (m_vPointOutOfSketchBook.y > m_vSketchBookPos.y + m_fSketchBookHeightHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y + m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _vPointInSketchBook, _iIdx, _iLineColor);
        }
        else if (m_vPointOutOfSketchBook.y < m_vSketchBookPos.y - m_fSketchBookHeightHalf)
        {
            Vector2 vAP1;
            vAP1.x = m_vSketchBookPos.x - m_fSketchBookWidthHalf;
            vAP1.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            Vector2 vAP2;
            vAP2.x = m_vSketchBookPos.x + m_fSketchBookWidthHalf;
            vAP2.y = m_vSketchBookPos.y - m_fSketchBookHeightHalf;

            m_line.SetIntersectionPoint(vAP1, vAP2, m_vPointOutOfSketchBook, _vPointInSketchBook, _iIdx, _iLineColor);
        }
    }

    public void SetPlayerInfo(int _iPlayerCnt)
    {
        switch (_iPlayerCnt)
        {
            case 2:
                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);

                m_listIsChatBoxOn = new List<bool>();
                m_listFChatBoxLimitTime = new List<float>();

                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);

                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                break;

            case 3:
                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);

                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);

                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                
                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                break;

            case 4:
                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);
                m_listPlayerInfo.Add(m_goPosition4);

                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);

                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                m_goPosition4.SetActive(true);
                break;

            case 5:
                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);
                m_listPlayerInfo.Add(m_goPosition4);
                m_listPlayerInfo.Add(m_goPosition5);

                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);

                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                m_goPosition4.SetActive(true);
                m_goPosition5.SetActive(true);
                break;

            case 6:
                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);
                m_listPlayerInfo.Add(m_goPosition4);
                m_listPlayerInfo.Add(m_goPosition5);
                m_listPlayerInfo.Add(m_goPosition6);

                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);
                m_listIsChatBoxOn.Add(false);

                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);
                m_listFChatBoxLimitTime.Add(3f);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                m_goPosition4.SetActive(true);
                m_goPosition5.SetActive(true);
                m_goPosition6.SetActive(true);
                break;

            default:
                break;
        }
    }

    private void StopAllPopupTime()
    {
        m_bStartPopupTime = false;
        m_bStartAnswerPopupTime = false;
        m_bStartChoiceAnswerPopupTime = false;
        m_bStartExaminerExitPopupTime = false;
    }

    private Color SetColor(int _iColorData)
    {
        Color color = new Color();

        switch (_iColorData)
        {
            case (int)GameData.EnumLineColor.BLACK:
                color = Color.black;
                break;

            case (int)GameData.EnumLineColor.WHITE:
                color = Color.white;
                break;

            case (int)GameData.EnumLineColor.RED:
                color = Color.red;
                break;

            case (int)GameData.EnumLineColor.ORANGE:
                color = new Color(1, 0.5f, 0, 1);
                break;

            case (int)GameData.EnumLineColor.YELLOW:
                color = Color.yellow;
                break;

            case (int)GameData.EnumLineColor.GREEN:
                color = Color.green;
                break;

            case (int)GameData.EnumLineColor.BLUE:
                color = Color.blue;
                break;

            case (int)GameData.EnumLineColor.PURPLE:

                float fR = (float)98 / 255;
                float fG = (float)27 / 255;
                float fB = (float)155 / 255;

                color = new Color(fR, fG, fB);
                break;
        }

        return color;
    }

    public void SetLineColorBlack()
    {
        m_iLineColor = (int)GameData.EnumLineColor.BLACK;
    }

    public void SetLineColorWhite()
    {
        m_iLineColor = (int)GameData.EnumLineColor.WHITE;
    }

    public void SetLineColorRed()
    {
        m_iLineColor = (int)GameData.EnumLineColor.RED;
    }

    public void SetLineColorOrange()
    {
        m_iLineColor = (int)GameData.EnumLineColor.ORANGE;
    }

    public void SetLineColorYellow()
    {
        m_iLineColor = (int)GameData.EnumLineColor.YELLOW;
    }

    public void SetLineColorGreen()
    {
        m_iLineColor = (int)GameData.EnumLineColor.GREEN;
    }

    public void SetLineColorBlue()
    {
        m_iLineColor = (int)GameData.EnumLineColor.BLUE;
    }

    public void SetLineColorPurple()
    {
        m_iLineColor = (int)GameData.EnumLineColor.PURPLE;
    }

    public void ExitGameCatch()
    {
        ExitGame(m_eGameType);
    }
}