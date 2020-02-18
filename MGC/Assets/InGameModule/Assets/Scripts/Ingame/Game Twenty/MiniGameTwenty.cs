using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MiniGameTwenty : MiniGame
{
    private bool m_bChallengeAnswer;
    private int m_iScore1, m_iScore2, m_iScore3, m_iScore4, m_iScore5, m_iQuestionCount;   
    private GameData.SendQuestionPacket m_sendQuestionPacket;
    private GameData.SendChallengeAnswerPacket m_sendChallengeAnswerPacket;
    private GameData.SendYesOrNoPacket m_sendYesOrNoPacket;
    private GameData.SendReserveExitOrCancelPacket m_sendReserveExitOrCancelPacket;
        
    public GameObject m_goStateText, m_goContent, m_goChallengeAnswer, m_goYesButton, m_goNoButton, m_goAnswerPopup, m_goResultPopup;
    public GameObject m_goPrefabIngameText, m_goPositionExaminerText, m_goPositionChallengerText, m_goQuestionCountText;
    public Text m_textChallengeAnswer, m_textState, m_textChatBoxPlaceHolder;
    public Text m_textAnswerPopupNickname, m_textAnswerPopupAnswer, m_textAnswerPopupTime, m_textScore1, m_textScore2, m_textScore3, m_textScore4, m_textScore5;
    public Text m_textResultPopupNickname1, m_textResultPopupNickname2, m_textResultPopupNickname3, m_textResultPopupNickname4, m_textResultPopupNickname5;
    public Text m_textPlayerExitPopupNickname, m_textPlayerExitMsg, m_textPlayerExitPopupTime, m_textQuestionCount;
    
    private void Start()
    {
        m_fLimitPopupTime = 3;
        m_bStartPopupTime = false;
        m_bChallengeAnswer = false;
        m_bRoundStart = false;
        m_iIngameLimitTime = 30;
        m_bStartAnswerPopupTime = false;
        m_bStartChoiceAnswerPopupTime = false;
        m_bStartExaminerExitPopupTime = false;
        m_bReserveExit = false;
        m_iQuestionCount = 20;
    }

    private void Update()
    {
        //1// input field 채팅
        if (m_ifChatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (m_bChallengeAnswer)
                {
                    SendChallengeAnswer();
                }
                else
                {
                    SendQuestion();
                }
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
        //1//

        // 채팅 수 카운팅
        CountChat();

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
            m_textIngameTime.text = "시간 : " + m_iIngameLimitTime;
            m_textQuestionCount.text = "남은 질문 횟수 : " + m_iQuestionCount;
        }
        //4//

        //5// 정답 팝업 타이머 -> 도전 실패 혹은 플레이어가 정답 맞췄을 때 정답 전체 공개해주는 팝업 타이머
        if (m_bStartAnswerPopupTime)
        {
            m_fLimitPopupTime -= Time.deltaTime;

            if (m_fLimitPopupTime < 0)
            {
                m_fLimitPopupTime = 0;
                m_bStartAnswerPopupTime = false;
            }

            // 올림하여 숫자 표시
            m_textAnswerPopupTime.text = "" + Mathf.Ceil(m_fLimitPopupTime);
        }
        //5//

        //6// "잠시 후 게임이 시작됩니다." 출제자가 나간 경우, 2명 남은 상태에서 질문자가 나간 경우 알려주는 팝업 타이머
        if (m_bStartExaminerExitPopupTime)
        {
            m_fLimitPopupTime -= Time.deltaTime;

            if (m_fLimitPopupTime < 0)
            {
                m_fLimitPopupTime = 0;
                m_bStartExaminerExitPopupTime = false;
            }

            // 올림하여 숫자 표시
            m_textPlayerExitPopupTime.text = "" + Mathf.Ceil(m_fLimitPopupTime);
        }
        //6//
    }

    public void Event(byte[] _baBuffer, GameData.EnumGameTwentyStructType _eStructType)
    {
        switch (_eStructType)
        {
            case GameData.EnumGameTwentyStructType.RECEIVE_POPUP_TIME_START_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 모든 플레이어 준비 완료 시 "잠시 후 게임이 시작됩니다." -> 타이머 start
                    GameData.ReceivePopupTimeStartPacket timeStart = new GameData.ReceivePopupTimeStartPacket();
                    timeStart = Serializer.ByteToStructure<GameData.ReceivePopupTimeStartPacket>(_baBuffer);
                    //1//

                    StopAllPopupTime();
                    m_goPlayerExitPopup.SetActive(false);
                    m_goReserveExitOrCancel.SetActive(false);
                    m_goAnswerPopup.SetActive(false);
                    m_goPopup.SetActive(true);

                    StartTime();
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_POPUP_TIME_END_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - "잠시 후 게임이 시작됩니다." -> 타이머 end
                    GameData.ReceivePopupTimeEndPacket timeEnd = new GameData.ReceivePopupTimeEndPacket();
                    timeEnd = Serializer.ByteToStructure<GameData.ReceivePopupTimeEndPacket>(_baBuffer);
                    //1//

                    // "잠시 후 게임이 시작됩니다." -> 팝업 내리고 게임 시작
                    EndTime();
                    ChoiceAnswer();
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_EXAMINER_ANSWERS_PACKET:
                {
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

            case GameData.EnumGameTwentyStructType.RECEIVE_SELECT_END_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 출제자 선택 완료 신호
                    GameData.ReceiveSelectCompletePacket examinerAnswer = new GameData.ReceiveSelectCompletePacket();
                    examinerAnswer = Serializer.ByteToStructure<GameData.ReceiveSelectCompletePacket>(_baBuffer);
                    //1//

                    RoundStart();
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_INGAME_TIME_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 인게임 타이머
                    GameData.ReceiveIngameTimePacket ingameTime = new GameData.ReceiveIngameTimePacket();
                    ingameTime = Serializer.ByteToStructure<GameData.ReceiveIngameTimePacket>(_baBuffer);
                    //1//

                    m_iIngameLimitTime = ingameTime.iIngameTime;
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_QUESTION_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 서버에서 받은 채팅 정보
                    GameData.ReceiveQuestionPacket chat = new GameData.ReceiveQuestionPacket();
                    chat = Serializer.ByteToStructure<GameData.ReceiveQuestionPacket>(_baBuffer);
                    //1//

                    //2// 문자열은 UTF8로 따로 인코딩
                    string strNickname = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);
                    string strQuestion = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + GameData.iStringNicknameSize, GameData.iStringChatSize);
                    //2//

                    bool bExitMsg = false;
                    InputChatToMainScreen(strNickname + " : " + strQuestion, Color.black, bExitMsg);
                    ChoiceYesOrNo();
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_YES_OR_NO_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 서버에서 받은 정답 확인 정보
                    GameData.ReceiveYesOrNoPacket yesOrNo = new GameData.ReceiveYesOrNoPacket();
                    yesOrNo = Serializer.ByteToStructure<GameData.ReceiveYesOrNoPacket>(_baBuffer);
                    //1//

                    // 문자열은 UTF8로 따로 인코딩
                    string strNickname = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);

                    if (yesOrNo.iYesOrNo == 1)
                    {
                        bool bExitMsg = false;

                        // 출제자가 Yes 대답을 했을 경우
                        InputChatToMainScreen(strNickname + " : Yes", Color.blue, bExitMsg);
                    }
                    else
                    {
                        bool bExitMsg = false;

                        // 출제자가 No 대답을 했을 경우
                        InputChatToMainScreen(strNickname + " : No", Color.red, bExitMsg);
                    }
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_CHECK_ANSWER_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 서버에서 받은 정답 확인 정보
                    GameData.ReceiveCheckAnswerPacket receiveCheckAnswer = new GameData.ReceiveCheckAnswerPacket();
                    receiveCheckAnswer = Serializer.ByteToStructure<GameData.ReceiveCheckAnswerPacket>(_baBuffer);
                    //1//

                    //2// 문자열은 UTF8로 따로 인코딩
                    string strNickname = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);
                    string strAnswer = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx + GameData.iStringNicknameSize, GameData.iStringAnswerSize);
                    //2//

                    bool bExitMsg = false;
                    InputChatToMainScreen(strNickname + " : " + strAnswer + " <정답 도전>", Color.black, bExitMsg);
                    CheckAnswer(receiveCheckAnswer.iResult, strNickname, strAnswer);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_NEXT_CHALLENGER_PACKET:
                {
                    //1// 서버에서 받은 패킷 deserializing - 다음 질문자 정보
                    GameData.ReceiveNextChallengerPacket nextChallenger = new GameData.ReceiveNextChallengerPacket();
                    nextChallenger = Serializer.ByteToStructure<GameData.ReceiveNextChallengerPacket>(_baBuffer);
                    //1//

                    // 문자열은 UTF8로 따로 인코딩
                    string strNickname = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);

                    // 다음 질문자로 넘어감
                    ChangeChallenger(strNickname, nextChallenger.iChangeType);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_ROUND_END_PACKET:
                {
                    GameData.ReceiveRoundEndPacket roundEnd = new GameData.ReceiveRoundEndPacket();
                    roundEnd = Serializer.ByteToStructure<GameData.ReceiveRoundEndPacket>(_baBuffer);

                    RoundEnd(roundEnd.strNextExaminer, roundEnd.strNextChallenger, roundEnd.iRound);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_SOCRE_PACKET:
                {
                    GameData.ReceiveScorePacket score = new GameData.ReceiveScorePacket();
                    score = Serializer.ByteToStructure<GameData.ReceiveScorePacket>(_baBuffer);

                    // 문자열은 UTF8로 따로 인코딩
                    string strNickname = Serializer.EncodingByteArrayToString(_baBuffer, GameData.iStringStartIdx, GameData.iStringNicknameSize);

                    UpdateScore(strNickname, score.iScore);                   
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_ANSWER_POPUP_TIME_START_PACKET:
                {
                    GameData.ReceiveAnswerPopupTimePacket answerPopupTime = new GameData.ReceiveAnswerPopupTimePacket();
                    answerPopupTime = Serializer.ByteToStructure<GameData.ReceiveAnswerPopupTimePacket>(_baBuffer);

                    StopAllPopupTime();
                    m_fLimitPopupTime = 3;
                    m_bStartAnswerPopupTime = true;
                    m_goAnswerPopup.SetActive(true);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_ANSWER_POPUP_ANSWER_PACKET:
                {
                    GameData.ReceiveAnswerPopupAnswerPacket answerPopupAnswer = new GameData.ReceiveAnswerPopupAnswerPacket();
                    answerPopupAnswer = Serializer.ByteToStructure<GameData.ReceiveAnswerPopupAnswerPacket>(_baBuffer);

                    m_textAnswerPopupNickname.text = "도전 실패";
                    m_textAnswerPopupAnswer.text = "정답 : " + answerPopupAnswer.strAnswer;
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_GAME_END_PACKET:
                {
                    GameData.ReceiveGameEndPacket gameEnd = new GameData.ReceiveGameEndPacket();
                    gameEnd = Serializer.ByteToStructure<GameData.ReceiveGameEndPacket>(_baBuffer);

                    //1// 서버가 준 1등 ~ 5등 이름, 점수를 출력
                    m_textResultPopupNickname1.text = gameEnd.strNickname1;
                    m_textResultPopupNickname2.text = gameEnd.strNickname2;
                    m_textResultPopupNickname3.text = gameEnd.strNickname3;
                    m_textResultPopupNickname4.text = gameEnd.strNickname4;
                    m_textResultPopupNickname5.text = gameEnd.strNickname5;

                    m_textScore1.text = "" + gameEnd.iScore1;

                    if (m_textResultPopupNickname2.text.Equals(""))
                        m_textScore2.text = "";
                    else
                        m_textScore2.text = "" + gameEnd.iScore2;

                    if (m_textResultPopupNickname3.text.Equals(""))
                        m_textScore3.text = "";
                    else
                        m_textScore3.text = "" + gameEnd.iScore3;

                    if (m_textResultPopupNickname4.text.Equals(""))
                        m_textScore4.text = "";
                    else
                        m_textScore4.text = "" + gameEnd.iScore4;

                    if (m_textResultPopupNickname5.text.Equals(""))
                        m_textScore5.text = "";
                    else
                        m_textScore5.text = "" + gameEnd.iScore5;
                    //1//

                    GameEnd();
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_EXAMINER_EXIT_PACKET:
                {
                    GameData.ReceiveExaminerExitPacket examinerExit = new GameData.ReceiveExaminerExitPacket();
                    examinerExit = Serializer.ByteToStructure<GameData.ReceiveExaminerExitPacket>(_baBuffer);

                    ExaminerExit(examinerExit.strNickname, examinerExit.iGameEnd);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_NON_EXAMINER_EXIT_PACKET:
                {
                    GameData.ReceiveNonExaminerExitPacket nonExaminerExit = new GameData.ReceiveNonExaminerExitPacket();
                    nonExaminerExit = Serializer.ByteToStructure<GameData.ReceiveNonExaminerExitPacket>(_baBuffer);

                    NonExaminerExit(nonExaminerExit.strNickname, nonExaminerExit.iGameEnd, nonExaminerExit.iChallenger);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_EXIT_PACKET:
                {
                    GameData.ReceiveExitPacket exit = new GameData.ReceiveExitPacket();
                    exit = Serializer.ByteToStructure<GameData.ReceiveExitPacket>(_baBuffer);

                    // 게임 종료 -> 게임 선택 메인 메뉴 활성
                    ExitGame(GameData.EnumGameType.TWENTY);
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_RESERVE_EXIT_OR_CANCEL_NICKNAME_PACEKT:
                {
                    GameData.ReceiveReserveExitOrCancelNicknamePacket reserveExitOrCancelNickname = new GameData.ReceiveReserveExitOrCancelNicknamePacket();
                    reserveExitOrCancelNickname = Serializer.ByteToStructure<GameData.ReceiveReserveExitOrCancelNicknamePacket>(_baBuffer);

                    m_goReserveExitOrCancel.SetActive(true);

                    string strNickname = reserveExitOrCancelNickname.strNickname;

                    if (reserveExitOrCancelNickname.iReserve.Equals(1))
                    {
                        if(strNickname.Equals(m_textNickname1.text))
                        {
                            m_goReserveExit1.SetActive(true);
                        }
                        else if (strNickname.Equals(m_textNickname2.text))
                        {
                            m_goReserveExit2.SetActive(true);
                        }
                        else if (strNickname.Equals(m_textNickname3.text))
                        {
                            m_goReserveExit3.SetActive(true);
                        }
                        else if (strNickname.Equals(m_textNickname4.text))
                        {
                            m_goReserveExit4.SetActive(true);
                        }
                        else if (strNickname.Equals(m_textNickname5.text))
                        {
                            m_goReserveExit5.SetActive(true);
                        }
                    }
                    else
                    {
                        if (strNickname.Equals(m_textNickname1.text))
                        {
                            m_goReserveExit1.SetActive(false);
                        }
                        else if (strNickname.Equals(m_textNickname2.text))
                        {
                            m_goReserveExit2.SetActive(false);
                        }
                        else if (strNickname.Equals(m_textNickname3.text))
                        {
                            m_goReserveExit3.SetActive(false);
                        }
                        else if (strNickname.Equals(m_textNickname4.text))
                        {
                            m_goReserveExit4.SetActive(false);
                        }
                        else if (strNickname.Equals(m_textNickname5.text))
                        {
                            m_goReserveExit5.SetActive(false);
                        }
                    }
                }
                break;

            case GameData.EnumGameTwentyStructType.RECEIVE_QUESTION_COUNT_PACKET:
                {
                    GameData.ReceiveQuestionCountPacket questionCount = new GameData.ReceiveQuestionCountPacket();
                    questionCount = Serializer.ByteToStructure<GameData.ReceiveQuestionCountPacket>(_baBuffer);

                    m_iQuestionCount = questionCount.iQuestionCount;
                }
                break;

            default:
                break;
        }
    }

    public void SetPlayerInfo(int _iPlayerCnt)
    {
        float fPlayerPositionY = m_goPosition1.transform.localPosition.y;
        float fPlayerPositionZ = m_goPosition1.transform.localPosition.z;

        switch (_iPlayerCnt)
        {
            case 2:
                m_goPosition1.transform.localPosition = new Vector3(-200, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition2.transform.localPosition = new Vector3(200, fPlayerPositionY, fPlayerPositionZ);

                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                break;

            case 3:
                m_goPosition1.transform.localPosition = new Vector3(-400, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition2.transform.localPosition = new Vector3(0, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition3.transform.localPosition = new Vector3(400, fPlayerPositionY, fPlayerPositionZ);

                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                break;

            case 4:
                m_goPosition1.transform.localPosition = new Vector3(-400, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition2.transform.localPosition = new Vector3(-133, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition3.transform.localPosition = new Vector3(133, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition4.transform.localPosition = new Vector3(400, fPlayerPositionY, fPlayerPositionZ);

                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);
                m_listPlayerInfo.Add(m_goPosition4);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                m_goPosition4.SetActive(true);
                break;

            case 5:
                m_goPosition1.transform.localPosition = new Vector3(-400, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition2.transform.localPosition = new Vector3(-200, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition3.transform.localPosition = new Vector3(0, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition4.transform.localPosition = new Vector3(200, fPlayerPositionY, fPlayerPositionZ);
                m_goPosition5.transform.localPosition = new Vector3(400, fPlayerPositionY, fPlayerPositionZ);

                m_listPlayerInfo.Add(m_goPosition1);
                m_listPlayerInfo.Add(m_goPosition2);
                m_listPlayerInfo.Add(m_goPosition3);
                m_listPlayerInfo.Add(m_goPosition4);
                m_listPlayerInfo.Add(m_goPosition5);

                m_goPosition1.SetActive(true);
                m_goPosition2.SetActive(true);
                m_goPosition3.SetActive(true);
                m_goPosition4.SetActive(true);
                m_goPosition5.SetActive(true);
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

    public void InputChatToMainScreen(string _strText, Color _chatColor, bool _bExitMsg)
    {
        GameObject goNewIngameText = Instantiate(m_goPrefabIngameText, m_goContent.transform);
        Text textIngameText = goNewIngameText.GetComponent<Text>();

        textIngameText.text = _strText;
        textIngameText.color = _chatColor;

        m_listText.Add(textIngameText);

        if(!_bExitMsg)
            m_ifChatBox.text = "";
    }

    private void SendChallengeAnswer()
    {
        //1// 스무고개 정답도전 상태에서 채팅 입력
        m_sendChallengeAnswerPacket.byteGameType = (byte)m_eGameType;
        m_sendChallengeAnswerPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_CHALLENGE_ANSWER_PACKET;
        m_sendChallengeAnswerPacket.strChat = m_ifChatBox.text;
        //1//

        //2// 패킷 보내고 ChatBox 숨김
        byte[] packet = Serializer.StructureToByte(m_sendChallengeAnswerPacket);

        IngameManager.m_Instance.GetClient().SendPacket(packet);
        HideChatBox();
        //2//
    }

    private void SendQuestion()
    {
        //1// 스무고개 질문하기 상태에서 채팅 입력
        m_sendQuestionPacket.byteGameType = (byte)m_eGameType;
        m_sendQuestionPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_QUESTION_PACKET;
        m_sendQuestionPacket.strChat = m_ifChatBox.text;
        //1//

        //2// 패킷 보내고 ChatBox 숨김
        byte[] packet = Serializer.StructureToByte(m_sendQuestionPacket);

        IngameManager.m_Instance.GetClient().SendPacket(packet);
        HideChatBox();
        //2//
    }

    private void NonExaminerExit(string _strNickname, int _iGameEnd, int _iChallenger)
    {
        m_goRadioGroupAnswer.SetActive(false);
        m_goWaitChoiceAnswerPopup.SetActive(false);

        //0// 나가기 예약 표시 지움
        if (_strNickname.Equals(m_textNickname1.text))
        {
            m_goReserveExit1.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname2.text))
        {
            m_goReserveExit2.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname3.text))
        {
            m_goReserveExit3.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname4.text))
        {
            m_goReserveExit4.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname5.text))
        {
            m_goReserveExit5.SetActive(false);
        }
        //0//

        //1//  플레이어가 나가면 'x' 표시
        if (_strNickname.Equals(m_textNickname1.text))
        {            
            m_goCharacterExit1.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname2.text))
        {
            m_goCharacterExit2.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname3.text))
        {
            m_goCharacterExit3.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname4.text))
        {
            m_goCharacterExit4.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname5.text))
        {
            m_goCharacterExit5.SetActive(true);
        }
        //1//

        //2// 채팅창에 나간 플레이어 표시
        string strMsg = "'" + _strNickname + "'님이 나갔습니다.";
        bool bExitMsg = true;

        InputChatToMainScreen(strMsg, Color.green, bExitMsg);
        //2//

        //3// 플레이어가 나갔다는 팝업 띄움
        if (_iGameEnd.Equals(1))
        {
            StopAllPopupTime();
            m_fLimitPopupTime = 3;
            m_textPlayerExitPopupNickname.text = strMsg;
            m_textPlayerExitMsg.text = "게임을 종료합니다.";

            m_bStartExaminerExitPopupTime = true;
            m_goPlayerExitPopup.SetActive(true);
        }
        else
        {
            if(_iChallenger.Equals(1))
            {
                StopAllPopupTime();
                m_fLimitPopupTime = 3;
                m_textPlayerExitPopupNickname.text = "질문자 " + strMsg;
                m_textPlayerExitMsg.text = "질문자를 다시 선정합니다.";

                m_bStartExaminerExitPopupTime = true;
                m_goPlayerExitPopup.SetActive(true);
            }
        }
        //3//
    }

    private void ExaminerExit(string _strNickname, int _iGameEnd)
    {        
        m_goRadioGroupAnswer.SetActive(false);
        m_goWaitChoiceAnswerPopup.SetActive(false);

        //0// 나가기 예약 표시 지움
        if (_strNickname.Equals(m_textNickname1.text))
        {
            m_goReserveExit1.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname2.text))
        {
            m_goReserveExit2.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname3.text))
        {
            m_goReserveExit3.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname4.text))
        {
            m_goReserveExit4.SetActive(false);
        }
        else if (_strNickname.Equals(m_textNickname5.text))
        {
            m_goReserveExit5.SetActive(false);
        }
        //0//

        //1//  플레이어가 나가면 'x' 표시
        if (_strNickname.Equals(m_textNickname1.text))
        {
            m_goCharacterExit1.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname2.text))
        {
            m_goCharacterExit2.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname3.text))
        {
            m_goCharacterExit3.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname4.text))
        {
            m_goCharacterExit4.SetActive(true);
        }
        else if (_strNickname.Equals(m_textNickname5.text))
        {
            m_goCharacterExit5.SetActive(true);
        }
        //1//

        //2// 채팅창에 나간 플레이어 표시
        string strMsg = "'" + _strNickname + "'님이 나갔습니다.";
        bool bExitMsg = true;

        InputChatToMainScreen(strMsg, Color.green, bExitMsg);
        //2//

        //3// 출제자가 나갔다는 팝업 띄움
        StopAllPopupTime();
        m_fLimitPopupTime = 3;        

        if (_iGameEnd.Equals(1))
        {
            m_textPlayerExitPopupNickname.text = strMsg;
            m_textPlayerExitMsg.text = "게임을 종료합니다.";
        }
        else
        {
            m_textPlayerExitPopupNickname.text = "출제자 " + strMsg;
            m_textPlayerExitMsg.text = "출제자를 다시 선정합니다.";
        }

        m_bStartExaminerExitPopupTime = true;
        m_goPlayerExitPopup.SetActive(true);
        //3//
    }

    private void SetExaminerPosition(string _strNickname)
    {
        float fExaminerTextPosY = m_goPositionExaminerText.transform.localPosition.y;
        float fExaminerTextPosZ = m_goPositionExaminerText.transform.localPosition.z;

        for (int i = 0; i < m_listPlayerInfo.Count; ++i)
        {
            if (_strNickname.Equals(m_listPlayerInfo[i].transform.GetChild(0).GetComponent<Text>().text))
            {
                m_goPositionExaminerText.transform.localPosition = new Vector3(m_listPlayerInfo[i].transform.localPosition.x, fExaminerTextPosY, fExaminerTextPosZ);
                break;
            }
        }
    }

    private void SetChallengerPosition(string _strNickname)
    {
        float fChallengerTextPosY = m_goPositionChallengerText.transform.localPosition.y;
        float fChallengerTextPosZ = m_goPositionChallengerText.transform.localPosition.z;

        for (int i = 0; i < m_listPlayerInfo.Count; ++i)
        {
            if (_strNickname.Equals(m_listPlayerInfo[i].transform.GetChild(0).GetComponent<Text>().text))
            {
                m_goPositionChallengerText.transform.localPosition = new Vector3(m_listPlayerInfo[i].transform.localPosition.x, fChallengerTextPosY, fChallengerTextPosZ);
                break;
            }
        }
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

            case GameData.EnumPlayerState.AWAITER:
                {
                    m_goWaitChoiceAnswerPopup.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

    private void ChangeChallenger(string _strNickname, int _iChangeType)
    {
        m_goPlayerExitPopup.SetActive(false);

        //1// 선정된 다음 질문자에 따라 상태와 텍스트 수정
        switch (m_ePlayerState)
        {
            case GameData.EnumPlayerState.EXAMINER:
                {
                    HideYesAndNoButton();
                }
                break;

            case GameData.EnumPlayerState.CHALLENGER:
                {
                    //1// 2명이 게임할 때는 질문자가 바로 다음 질문자가 되므로 대기자가 될 필요가 없다. -> 사라졌던 chatbox 다시 보이게 한다.
                    if (!_strNickname.Equals(m_strNickname))
                    {
                        m_ePlayerState = GameData.EnumPlayerState.AWAITER;
                        m_textState.text = "대기중";

                        HideChatBox();
                    }
                    else
                    {
                        ShowChatBox();
                    }
                    //1//
                }
                break;

            case GameData.EnumPlayerState.AWAITER:
                {
                    //1// 내가 다음 질문자일 경우
                    if (_strNickname.Equals(m_strNickname))
                    {
                        m_ePlayerState = GameData.EnumPlayerState.CHALLENGER;
                        m_textState.text = "질문중";

                        ShowChatBox();
                    }
                    //1//
                }
                break;

            default:
                break;
        }
        //1//

        //2// 0 : 정상 진행, 1 : 질문자 타임 아웃, 2 : 출제자 타임 아웃
        if(_iChangeType.Equals(1))
        {
            string strMsg = "<<질문 제한 시간 초과>>";
            bool bExitMsg = false;

            InputChatToMainScreen(strMsg, Color.red, bExitMsg);
        }
        else if (_iChangeType.Equals(2))
        {
            string strMsg = "<<정답 확인 제한 시간 초과>>";
            bool bExitMsg = false;

            InputChatToMainScreen(strMsg, Color.red, bExitMsg);
        }
        //2//

        // 질문자 텍스트 위치 조정
        SetChallengerPosition(_strNickname);
    }

    public void SetExaminerAndChallengerTextPosition(int iPlayerCnt)
    {
        float fExaminerTextPosY = m_goPositionExaminerText.transform.localPosition.y;
        float fExaminerTextPosZ = m_goPositionExaminerText.transform.localPosition.z;
        float fChallengerTextPosY = m_goPositionChallengerText.transform.localPosition.y;
        float fChallengerTextPosZ = m_goPositionChallengerText.transform.localPosition.z;

        switch (iPlayerCnt)
        {
            case 2:
                {
                    m_goPositionExaminerText.transform.localPosition = new Vector3(-200, fExaminerTextPosY, fExaminerTextPosZ);
                    m_goPositionChallengerText.transform.localPosition = new Vector3(200, fChallengerTextPosY, fChallengerTextPosZ);
                }
                break;

            case 3:
                {
                    m_goPositionExaminerText.transform.localPosition = new Vector3(-400, fExaminerTextPosY, fExaminerTextPosZ);
                    m_goPositionChallengerText.transform.localPosition = new Vector3(0, fChallengerTextPosY, fChallengerTextPosZ);
                }
                break;

            case 4:
                {
                    m_goPositionExaminerText.transform.localPosition = new Vector3(-400, fExaminerTextPosY, fExaminerTextPosZ);
                    m_goPositionChallengerText.transform.localPosition = new Vector3(-133, fChallengerTextPosY, fChallengerTextPosZ);
                }
                break;

            case 5:
                {
                    m_goPositionExaminerText.transform.localPosition = new Vector3(-400, fExaminerTextPosY, fExaminerTextPosZ);
                    m_goPositionChallengerText.transform.localPosition = new Vector3(-200, fChallengerTextPosY, fChallengerTextPosZ);
                }
                break;

            default:
                break;
        }
    }

    private void UpdateScore(string _strNickname, int _iScore)
    {
        if(_strNickname.Equals(m_textNickname1.text))
        {
            m_iScore1 = _iScore;
            m_textNickname1.transform.GetChild(1).GetComponent<Text>().text = "" + m_iScore1;
        }
        else if (_strNickname.Equals(m_textNickname2.text))
        {
            m_iScore2 = _iScore;
            m_textNickname2.transform.GetChild(1).GetComponent<Text>().text = "" + m_iScore2;
        }
        else if (_strNickname.Equals(m_textNickname3.text))
        {
            m_iScore3 = _iScore;
            m_textNickname3.transform.GetChild(1).GetComponent<Text>().text = "" + m_iScore3;
        }
        else if (_strNickname.Equals(m_textNickname4.text))
        {
            m_iScore4 = _iScore;
            m_textNickname4.transform.GetChild(1).GetComponent<Text>().text = "" + m_iScore4;
        }
        else if (_strNickname.Equals(m_textNickname5.text))
        {
            m_iScore5 = _iScore;
            m_textNickname5.transform.GetChild(1).GetComponent<Text>().text = "" + m_iScore5;
        }
    }

    public void SendYes()
    {
        m_sendYesOrNoPacket.byteGameType = (byte)m_eGameType;
        m_sendYesOrNoPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_YES_OR_NO_PACKET;
        m_sendYesOrNoPacket.iYesOrNo = 1;
        
        //1// yes 패킷 보내고 yes, no button 숨김
        byte[] packet = Serializer.StructureToByte(m_sendYesOrNoPacket);

        IngameManager.m_Instance.GetClient().SendPacket(packet);
        HideYesAndNoButton();
        //1//
    }

    public void SendNo()
    {
        m_sendYesOrNoPacket.byteGameType = (byte)m_eGameType;
        m_sendYesOrNoPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_YES_OR_NO_PACKET;
        m_sendYesOrNoPacket.iYesOrNo = 0;

        //1// no 패킷 보내고 yes, no button 숨김
        byte[] packet = Serializer.StructureToByte(m_sendYesOrNoPacket);

        IngameManager.m_Instance.GetClient().SendPacket(packet);
        HideYesAndNoButton();
        //1//
    }

    private void CheckAnswer(int _iResult, string _strNickname, string _strAnswer)
    {
        if(_iResult == 1)
        {
            //1// 서버에서 정답이 맞다고 보낸 경우
            string strMsg = "정답 확인 : Yes";
            bool bExitMsg = false;

            InputChatToMainScreen(strMsg, Color.blue, bExitMsg);
            //1//

            m_textAnswerPopupNickname.text = "정답자 : " + _strNickname;
            m_textAnswerPopupAnswer.text = "정답 : " + _strAnswer;
        }
        else
        {
            //1// 서버에서 정답이 아니라고 보낸 경우
            string strMsg = "정답 확인 : No";
            bool bExitMsg = false;

            InputChatToMainScreen(strMsg, Color.red, bExitMsg);
            //1//
        }
    }

    private void HideYesAndNoButton()
    {
        m_goYesButton.SetActive(false);
        m_goNoButton.SetActive(false);
    }

    private void ShowYesAndNoButton()
    {
        m_goYesButton.SetActive(true);
        m_goNoButton.SetActive(true);
    }

    private void HideChatBox()
    {
        m_goChatCountBox.SetActive(false);
        m_goChatBox.SetActive(false);
        m_goChallengeAnswer.SetActive(false);

        m_ifChatBox.text = "";

        m_textChatBoxText.color = Color.black;
        m_textChatBoxText.text = "";

        m_textChatBoxPlaceHolder.color = Color.black;
        m_textChatBoxPlaceHolder.text = "'질문하기' 모드입니다.";

        m_textChallengeAnswer.text = "정답도전";

        SetChatBoxCharacterLimit(30);

        m_bChallengeAnswer = false;
    }

    private void ShowChatBox()
    {
        m_goChatCountBox.SetActive(true);
        m_goChatBox.SetActive(true);
        m_goChallengeAnswer.SetActive(true);
    }

    private void ChoiceYesOrNo()
    {
        switch (m_ePlayerState)
        {
            case GameData.EnumPlayerState.EXAMINER:
                {
                    ShowYesAndNoButton();
                }
                break;

            case GameData.EnumPlayerState.CHALLENGER:
                {   
                }
                break;

            case GameData.EnumPlayerState.AWAITER:
                {
                }
                break;

            default:
                break;
        }        
    }

    private void RoundStart()
    {
        m_bRoundStart = true;
        m_goIngameTimeText.SetActive(true);
        m_goQuestionCountText.SetActive(true);
        m_goStateText.SetActive(true);
        m_goPositionChallengerText.SetActive(true);
        m_goPositionExaminerText.SetActive(true);
        m_textRound.gameObject.SetActive(true);
        m_goReserveExitOrCancel.SetActive(true);

        switch (m_ePlayerState)
        {
            case GameData.EnumPlayerState.EXAMINER:
                {
                    m_goRadioGroupAnswer.SetActive(false);

                    m_textState.text = "출제중";
                    m_textAnswer.text = "< " + m_strAnswer + " >";
                    m_goAnswerText.SetActive(true);

                    SendAnswer();
                }
                break;

            case GameData.EnumPlayerState.CHALLENGER:
                {
                    m_goWaitChoiceAnswerPopup.SetActive(false);

                    m_textState.text = "질문중";

                    ShowChatBox();
                }
                break;

            case GameData.EnumPlayerState.AWAITER:
                {
                    m_goWaitChoiceAnswerPopup.SetActive(false);

                    m_textState.text = "대기중";
                }
                break;

            default:
                break;
        }
    }

    private void RoundEnd(string _strNextExaminer, string _strNextChallenger, int _iRound)
    {
        m_bRoundStart = false;
        m_goIngameTimeText.SetActive(false);
        m_goQuestionCountText.SetActive(false);
        m_goStateText.SetActive(false);
        m_goPositionChallengerText.SetActive(false);
        m_goPositionExaminerText.SetActive(false);
        m_textRound.gameObject.SetActive(false);
        m_goAnswerText.SetActive(false);
        HideChatBox();
        HideYesAndNoButton();

        //1// 출제자, 질문자 변경
        if (_strNextExaminer.Equals(m_strNickname))
        {
            m_ePlayerState = GameData.EnumPlayerState.EXAMINER;
        }
        else if(_strNextChallenger.Equals(m_strNickname))
        {
            m_ePlayerState = GameData.EnumPlayerState.CHALLENGER;
        }
        else
        {
            m_ePlayerState = GameData.EnumPlayerState.AWAITER;
        }
        //1//

        // 출제자, 질문자 텍스트 위치 변경
        SetExaminerPosition(_strNextExaminer);
        SetChallengerPosition(_strNextChallenger);

        // 라운드 변경
        m_textRound.text = "Round" + _iRound;

        //2// 스크린에 있는 텍스트를 모두 지운다.
        int iCount = m_listText.Count;

        for(int i = 0; i < iCount; ++i)
        {
            Destroy(m_listText[0].gameObject);
            m_listText.RemoveAt(0);
        }
        //2//
        
        //3// 준비완료 패킷 전송
        byte[] packet = IngameManager.m_Instance.GetSendGameReadyPacket();
        IngameManager.m_Instance.GetClient().SendPacket(packet);
        //3//
    }

    private void GameEnd()
    {
        m_bRoundStart = false;
        m_goIngameTimeText.SetActive(false);
        m_goQuestionCountText.SetActive(false);
        m_goStateText.SetActive(false);
        m_goPositionChallengerText.SetActive(false);
        m_goPositionExaminerText.SetActive(false);
        m_textRound.gameObject.SetActive(false);
        m_goAnswerText.SetActive(false);
        m_goPlayerExitPopup.SetActive(false);
        m_goReserveExitOrCancel.SetActive(false);

        m_goResultPopup.SetActive(true);
    }

    public bool Get_bChallengeAnswer()
    {
        return m_bChallengeAnswer;
    }

    private void SetChatBoxCharacterLimit(int _iLimitCount)
    {
        m_ifChatBox.characterLimit = _iLimitCount;
    }

    private void ChallengeAnswerOrQuestion()
    {
        if (!m_bChallengeAnswer)
        {
            m_ifChatBox.text = "";

            m_textChatBoxText.color = Color.red;
            m_textChatBoxText.text = "";

            m_textChatBoxPlaceHolder.color = new Color(1, 0, 0, 0.5f);
            m_textChatBoxPlaceHolder.text = "'정답도전' 모드입니다.";

            m_textChallengeAnswer.text = "질문하기";
            
            SetChatBoxCharacterLimit(10);

            m_bChallengeAnswer = true;
        }
        else
        {
            m_ifChatBox.text = "";

            m_textChatBoxText.color = Color.black;
            m_textChatBoxText.text = "";

            m_textChatBoxPlaceHolder.color = new Color(0, 0, 0, 0.5f);
            m_textChatBoxPlaceHolder.text = "'질문하기' 모드입니다.";

            m_textChallengeAnswer.text = "정답도전";
            
            SetChatBoxCharacterLimit(30);

            m_bChallengeAnswer = false;
        }
    }

    public void SendReserveExitOrCancel()
    {
        if(m_bReserveExit)
        {
            m_sendReserveExitOrCancelPacket.byteGameType = (byte)GameData.EnumGameType.TWENTY;
            m_sendReserveExitOrCancelPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_RESERVE_EXIT_OR_CANCEL_PACKET;
            m_sendReserveExitOrCancelPacket.iReserve = 0;

            byte[] packet = Serializer.StructureToByte(m_sendReserveExitOrCancelPacket);

            IngameManager.m_Instance.GetClient().SendPacket(packet);

            m_goReserveExitOrCancel.SetActive(false);
            m_textReserveExitOrCancel.text = "나가기 예약";

            m_bReserveExit = false;
        }
        else
        {
            m_sendReserveExitOrCancelPacket.byteGameType = (byte)GameData.EnumGameType.TWENTY;
            m_sendReserveExitOrCancelPacket.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_RESERVE_EXIT_OR_CANCEL_PACKET;
            m_sendReserveExitOrCancelPacket.iReserve = 1;
            
            byte[] packet = Serializer.StructureToByte(m_sendReserveExitOrCancelPacket);

            IngameManager.m_Instance.GetClient().SendPacket(packet);

            m_goReserveExitOrCancel.SetActive(false);
            m_textReserveExitOrCancel.text = "나가기 취소";

            m_bReserveExit = true;
        }
    }

    public void HideAnswer()
    {
        if(m_ePlayerState.Equals(GameData.EnumPlayerState.EXAMINER))
        {
            if (m_goAnswerText.activeSelf.Equals(false))
                m_goAnswerText.SetActive(true);
            else
                m_goAnswerText.SetActive(false);
        }
    }

    public void ExitGameTwenty()
    {
        ExitGame(m_eGameType);
    }
}