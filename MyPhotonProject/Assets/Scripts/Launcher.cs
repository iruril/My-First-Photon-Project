using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Sereializable Fields
    [Tooltip("방 하나당 허용 가능한 플레이어 수. 꽉 차면, 다음 플레이어 접속 시 새로운 방을 생성한다.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    #endregion

    #region Private Fields
    string gameVersion = "1";
    bool isConnecting;
    #endregion

    #region Public Fields
    [Tooltip("플레이어가 이름을 입력하고 플레이할 수 있는 UI패널입니다.")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("플레이어에게 연결 진행 정보를 알려주는 UI라벨입니다.")]
    [SerializeField]
    private GameObject progressLabel;
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("PUN에 의해 OnConnectedToMaster()호출");
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("{0}과 같은 이유로 PUN에 의해 OnDisconnected()호출", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN에 의해 OnJoinRandomFailed() 호출. 유효한 방이 없으므로 하나 생성함." +
            "\n호출: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN에 의해 OnJoinedRoom() 호출. 이 클라이언트는 방에 접속해 있음.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'GameField 1' ");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("GameField 1");
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion

    #region Public Methods
    public void Connect()
    {
        isConnecting = true;
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion
}
