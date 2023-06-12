using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public static ChatManager ChatInstance;

    public List<string> playerList = new List<string>();
    public Button sendButton;
    public Text chattingLog;
    public Text playerListTxt;
    public InputField chatInput;
    ScrollRect scroll_rect = null;
    string chatters;

    void Start()
    {
        ChatInstance = this;
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
        DontDestroyOnLoad(chattingLog);
    }

    public void SendButtonOnClicked()
    {
        if (chatInput.text.Equals(""))
        {
            Debug.Log("Empty"); return;
        }

        string msg = string.Format("[{0}] : {1}", PhotonNetwork.LocalPlayer.NickName, chatInput.text);
        photonView.RPC("ReceiveMsg", RpcTarget.Others, msg);
        ReceiveMsg(msg);
        chatInput.ActivateInputField();
        chatInput.text = "";
    }

    public void SendKillLog(string msg)
    {
        photonView.RPC("ReceiveKillMsg", RpcTarget.Others, msg);
        ReceiveKillMsg(msg);
    }
        void Update()
    {
        chatterUpdate();
    }
    void chatterUpdate()
    {
        chatters = "";
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            chatters += p.NickName + "\n";
        }
        playerListTxt.text = chatters;
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        ChatInstance.chattingLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    [PunRPC]
    public void ReceiveKillMsg(string msg)
    {
        ChatInstance.chattingLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
