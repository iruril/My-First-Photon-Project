using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields
    public static GameManager Instance;
    [Tooltip("플레이어 표시에 쓸 프리펩")]
    public GameObject playerPrefab;
    #endregion

    private Hashtable CP;

    #region Photon Callbacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadGameScene();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadGameScene();
        }
    }
    #endregion

    #region Private Methods
    void LoadGameScene()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("우리는 마스터 클라이언트가 아니지만, 일단 레벨을 로드합니다.");
            return;
        }
        //Debug.LogFormat("로드 레벨 : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.LogFormat("로드 레벨 : 1", PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("GameField " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("GameField 1");
    }
    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Respawn()
    {
        GameObject playerTemp = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        int classNum = (int)CP["Class"];
        playerTemp.GetComponent<PlayerControl>().meshChange(classNum);
    }

    #endregion

    #region MonoBehaviourPun Callbacks
    void Start()
    {
        CP = PhotonNetwork.LocalPlayer.CustomProperties;
        Instance = this;
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerControl.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                GameObject playerTemp = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                int classNum = (int)CP["Class"];
                playerTemp.GetComponent<PlayerControl>().meshChange(classNum);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    void Update()
    {
        
    }
    #endregion
}
