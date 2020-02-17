#define DEBUGMODE




using PACKET;
using PROTOCOL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;



// 10.255.252.83 내 컴
// 정환님 컴 10.255.252.95
public class GameManager : MonoBehaviour {

    static GameManager m_instance;
    public static GameManager m_Instance { get { return m_instance; } }
    bool                    m_bInit = false;

    // 매니저 목록
    public socketManager    m_socketMGR;
    public uiManager        m_uiMGR;
    public ingameManager    m_ingameMGR;
    public resourceManager  m_resourceMGR;
    public inputKeyManager  m_inputKeyMGR;

    // 세팅 파일 path
    string                  m_settingServerFilePath = "\\StreamingAssets\\SetFile\\SettingServer.txt";
    StreamReader            m_settingFileReader;


    [SerializeField]
    Text                    m_TestText;

    // 재연결
    float                   m_fReConnectTime = 2.0f;
    float                   m_fCurrentReConnectTime = 0.0f;
    // 연결 여부
    bool                    m_bConnectedLoginServer;
    bool                    m_bConnectedMainServer;
    // 서버 정보
    public string           m_serverIP;
    public string           m_main_serverIP;
    public int              m_nPortNUM;
    public int              m_nMain_PortNUM;

    // 총 게임 진행 시간
    float                   m_fGameTime;

    // 오류 로그
    StreamWriter            m_StreamWriter;
    public string           m_logFilePath;

    // 화면 사이즈
    public  int             m_nWidth;
    public  int             m_nHeight;


    #region IN GAME OBJECT

    bool                    m_bNowInGame;
    [SerializeField]
    GameObject              m_inGameOBJ;
    [SerializeField]
    GameObject              m_outUIOBJ;

    #endregion


    #region LOGIN DATA
    bool m_bLogin;
    S_UserData              m_userData;
    S_UserAccessData        m_accessData;
    #endregion

    // Use this for initialization
    void Start () {
        m_bInit = false;
        if (initGameMGR() == false)
            return;
        initManagement();
    }
	


	// Update is called once per frame
	void Update () {
        if (m_bInit == false)
            return;
        try
        {
            m_fGameTime += Time.deltaTime;
            if(m_fCurrentReConnectTime>0)
                m_fCurrentReConnectTime -= Time.deltaTime;

            if (m_socketMGR)
                m_socketMGR.updateManager();
            if (m_uiMGR)
                m_uiMGR.updateManager();
            //m_ingameMGR.updateManager();
            if (m_inputKeyMGR)
                ((inputKeyManager)m_inputKeyMGR).checkInputKey();
        }
        catch(Exception ex)
        {
            onErrorClient("Update Error");
        }
    }
    #region INIT
    bool initGameMGR()
    {
        try
        {
            if (m_instance == null)
            {
                m_instance = this;
                DontDestroyOnLoad(this);
                return true;
            }
            else
            {
                DestroyImmediate(this);
                return false;
            }
        }
        catch(Exception ex)
        {
            writeErrorLog(ex.Message);
            return false;
        }
    }

    void initManagement()
    {
        Screen.SetResolution(m_nWidth, m_nHeight, false);

        m_fGameTime = 0;

        if (Directory.GetDirectories(".\\", "log", SearchOption.TopDirectoryOnly).Length <= 0)
            Directory.CreateDirectory(".\\log");
        // init error log writer

        //server set
        settingServer();

        if (m_uiMGR)
            m_uiMGR.init();
        if(m_ingameMGR)
            m_ingameMGR.init();
        if (m_resourceMGR)
            m_resourceMGR.init();
        if (m_inputKeyMGR)
            m_inputKeyMGR.init();
        try
        {
            if (m_socketMGR)
            {
                m_socketMGR.init();
                connect_loginServer();
            }
        }
        catch(Exception ex)
        {
            writeErrorLog(ex.Message);
        }
        openOutGameUI();

        m_bInit = true;
    }

    void settingServer()
    {
        m_settingFileReader = new StreamReader(Application.dataPath+m_settingServerFilePath);

        // login server
        string  curLine;
        string  ip = "";
        int     port = 0;

        curLine = m_settingFileReader.ReadLine();
        string[] splits = curLine.Split(' ');

        ip = splits[1];
        port = Int32.Parse(splits[2]);

        setLoginServerInfo(ip,port);


        curLine = m_settingFileReader.ReadLine();
        splits = curLine.Split(' ');

        ip = splits[1];
        port = Int32.Parse(splits[2]);

        setMainServerInfo(ip, port);



        m_settingFileReader.Close();
    }

    private void OnDestroy()
    {
        release();
    }

    void release()
    {
        if(m_socketMGR)
            m_socketMGR.release();
        if(m_uiMGR)
            m_uiMGR.release();
        if (m_ingameMGR)
            m_ingameMGR.release();
        if (m_resourceMGR)
            m_resourceMGR.release();
        if (m_inputKeyMGR)
            m_inputKeyMGR.release();
    }

    #endregion

    public void inputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        ((uiManager)m_uiMGR).inputKey(type);
    }

    /// <summary>
    /// 에러나 예외 처리시 log기록
    /// </summary>
    /// <param name="errorlog"></param>
    public void writeErrorLog(string errorlog)
    {
        if (errorlog == null)
            return;
        m_StreamWriter = new StreamWriter(m_logFilePath, true);
        using (m_StreamWriter)
        {
            //Debug.Log(errorlog);
            m_StreamWriter.WriteLine(new StringBuilder(System.DateTime.Now.ToString()).Append(" : ").Append( errorlog).ToString());
            m_StreamWriter.Close();
        }
    }

    #region SERVER FUNC

    /// <summary>
    /// 클라이언트의 에러 상황에서 서버를 초기화 하는 작업
    /// </summary>
    public void onErrorClient(string errorMsg)
    {
        if (GameManager.m_Instance.isConnectedMainServer())
        {
            if(m_bNowInGame)
            {
                openOutGameUI();
            }
            // 공통사항
            GameManager.m_Instance.disconnect_mainServer();
            GameManager.m_Instance.setConnectStateMainServer(false, true);
        }
        else if (GameManager.m_Instance.isConnectedLoginServer())
        {
            GameManager.m_Instance.disconnect_loginServer();
            GameManager.m_Instance.setConnectStateLoginServer(false, true);
        }
        m_uiMGR.onErrorClient(errorMsg);
    }
    public void setLoginServerInfo(string ip, int nPortNUM)
    {
        m_serverIP = ip;
        m_nPortNUM = nPortNUM;
    }

    public void setMainServerInfo(string ip, int nPortNUM)
    {
        m_main_serverIP = ip;
        m_nMain_PortNUM = nPortNUM;
    }
    public bool connect_loginServer()
    {
        if (m_bConnectedLoginServer) return false;
        setConnectStateLoginServer(m_socketMGR.connect_loginServer(m_serverIP, m_nPortNUM),true);
        return m_bConnectedLoginServer;
    }
    public bool connect_mainServer()
    {
        if (m_bConnectedMainServer) return false;
        setConnectStateMainServer(m_socketMGR.connect_mainServer(m_main_serverIP, m_nMain_PortNUM), true);
        return m_bConnectedMainServer;
    }

    public void disconnect_loginServer()
    {
        m_socketMGR.disconnect_loginServer();
        setConnectStateLoginServer(false,false);
    }
    public void disconnect_mainServer()
    {
        m_socketMGR.disconnect_mainServer();
        setConnectStateMainServer(false, false);
    }
    public bool isConnectedLoginServer()
    {
        return m_bConnectedLoginServer;
    }
    public bool isConnectedMainServer()
    {
        return m_bConnectedMainServer;
    }

    public void setConnectStateLoginServer(bool bConnected, bool bAnomaly)
    {
        //m_Text.text += ("Login::"+ bConnected+"//");
        m_bConnectedLoginServer = bConnected;
        if (bAnomaly)
        {
            C_Anomaly data = new C_Anomaly();
            AnomalyType anomalyType;
            if (m_bConnectedLoginServer)
            {
                // wait connect ui close
                anomalyType = AnomalyType.loginServer_Reconnect;
            }
            else
            {
                // wait connect ui open
                anomalyType = AnomalyType.loginServer_Disconnect;
                setLoginState(false);
            }
            data.setType(anomalyType);
            makeUiEvent(data);
        }
    }
    public void setConnectStateMainServer(bool bConnected, bool bAnomaly)
    {
        //m_Text.text += ("Main::" + bConnected + "//");
        m_bConnectedMainServer = bConnected;
        if (bAnomaly)
        {
            C_Anomaly data = new C_Anomaly();
            AnomalyType anomalyType;
            if (m_bConnectedMainServer)
            {
                // wait connect ui close
                anomalyType =  AnomalyType.mainServer_Reconnect;
            }
            else
            {
                // wait connect ui open
                anomalyType = AnomalyType.mainServer_Disconnect;
                setLoginState(false);
            }
            data.setType(anomalyType);
            makeUiEvent(data);
        }
    }
    public void reconnectLoginServer()
    {
        if (m_fCurrentReConnectTime <= 0)
        {
            m_fCurrentReConnectTime = m_fReConnectTime;
            connect_loginServer();
        }
    }
    #endregion

    #region MAKE EVENT
   

    public void makePacket(PACKET.C_BasePacket packet)
    {
        m_socketMGR.enqueueEvent(packet);
    }
    public void makePacket(byte[] buf)
    {
        C_InGamePacket packet = new C_InGamePacket();
        packet.m_gameData = buf;
        m_socketMGR.enqueueEvent(packet);
    }


    /// <summary>
    /// 해당 이벤트를 생성하여 event buffer에 enqueue
    /// </summary>
    public void makeUiEvent(C_BasePacket eventData)
    {
        m_uiMGR.enqueueEvent(eventData);
    }

    /// <summary>
    /// 해당 이벤트를 생성하여 event buffer에 enqueue
    /// </summary>
    public void makeInGameEvent(C_BasePacket eventData)
    {
        m_ingameMGR.enqueueEvent(eventData);
    }

    #endregion

    #region GET / SET
    public float getGameTime()
    {
        return m_fGameTime;
    }

    /// <summary>
    /// 로그인 서버에서 로그인 완료시 계정 정보를 세팅
    /// </summary>
    /// <param name="accessData"></param>
    public void setAccessData(S_UserAccessData accessData)
    {
        m_accessData = accessData;
        setLoginState(true);
    }
    /// <summary>
    /// 게임 서버에 접속 후, 로딩 시에 받은 데이터로 유저 데이터를 세팅
    /// </summary>
    /// <param name="data"></param>
    public void setUserData(C_PreLoadPacketLoadPlayerInfo data)
    {
        m_userData.m_nickName = data.m_playerName;
    }
    public void setUserGuildData(bool bExistGuild, object guildInfo)
    {
        //m_userData.m_bExistGuild = bExistGuild;
        //if(m_userData.m_bExistGuild)
        //    m_userData.m_guildInfo = guildInfo;
        //else
        //    m_userData.m_guildInfo = new S_GUILD_INFO();
    }
    public bool getExistGuild()
    {
        //return m_userData.m_bExistGuild;
        return false;
    }
    public object getGuildInfo()
    {
        return null;
    }
    public string getUserID()
    {
        return m_accessData.m_accessID;
    }
    public string getUserNickName()
    {
        return m_userData.m_nickName;
    }
    #endregion

    #region LOGIN
    /// <summary>
    /// 현재 로그인 상태를 바꾼다.
    /// 로그인 시 -> true
    /// 로그아웃 혹은 로그인 서버 끊김 or 메인 서버 끊김 시 -> false
    /// </summary>
    /// <param name="bLogin"></param>
    public void setLoginState(bool bLogin)
    {
        m_bLogin = bLogin;
    }
    public bool isLogin()
    {
        return m_bLogin;
    }
    #endregion

    #region RESOURCE

    public Sprite getCharacterSprite(int nIndex)
    {
        return m_resourceMGR.getCharacterSprite(nIndex);
    }

    public Sprite getUIsprite(resourceManager.UIspriteINDEX index)
    {
        return m_resourceMGR.getUIsprite((int)index);
    }


    #endregion


    #region In GAME / UI

    public void startGame()
    {
        openInGameManager();

        // ingame manager 세팅
    }

    public void endGame()
    {
        openOutGameUI();
    }

    /// <summary>
    /// out game ui를 활성화 시킨다.
    /// in game를 비 활성화 시킨다.
    /// </summary>
    void openOutGameUI()
    {
        m_bNowInGame = false;
        m_inGameOBJ.SetActive(false);
        m_outUIOBJ.SetActive(true);
    }

    void openInGameManager()
    {
        m_bNowInGame = true;
        m_inGameOBJ.SetActive(true);
        m_outUIOBJ.SetActive(false);
    }


    #endregion



    public void DEBUG(string msg)
    {
        m_TestText.text += msg+'\n';
    }
}
