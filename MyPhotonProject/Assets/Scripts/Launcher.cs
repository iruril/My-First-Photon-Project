using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Sereializable Fields
    [Tooltip("�� �ϳ��� ��� ������ �÷��̾� ��. �� ����, ���� �÷��̾� ���� �� ���ο� ���� �����Ѵ�.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    #endregion

    #region Private Fields
    string gameVersion = "1";
    bool isConnecting;
    #endregion

    #region Public Fields
    [Tooltip("�÷��̾ �̸��� �Է��ϰ� �÷����� �� �ִ� UI�г��Դϴ�.")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("�÷��̾�� ���� ���� ������ �˷��ִ� UI���Դϴ�.")]
    [SerializeField]
    private GameObject progressLabel;
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("PUN�� ���� OnConnectedToMaster()ȣ��");
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
        Debug.LogWarningFormat("{0}�� ���� ������ PUN�� ���� OnDisconnected()ȣ��", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN�� ���� OnJoinRandomFailed() ȣ��. ��ȿ�� ���� �����Ƿ� �ϳ� ������." +
            "\nȣ��: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN�� ���� OnJoinedRoom() ȣ��. �� Ŭ���̾�Ʈ�� �濡 ������ ����.");
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
