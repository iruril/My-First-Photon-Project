using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ClassPanel : MonoBehaviourPun
{
    private Hashtable CP;

    public Text className;

    public Image class_1;
    public Image class_2;
    public Image class_3;
    public Image class_4;

    int playerClassNumber = 0; // 0 ~ 3
    private bool onChange = false;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Class", 0 } });
        CP = PhotonNetwork.LocalPlayer.CustomProperties;
        Portrait();
        SetClass();
    }

    // Update is called once per frame
    void Update()
    {
        if (onChange)
        {
            Portrait();
            onChange = false;
        }
    }

    void Portrait()
    {
        string name = string.Empty;
        switch (playerClassNumber)
        {
            case 0:
                class_1.gameObject.SetActive(true);
                class_2.gameObject.SetActive(false);
                class_3.gameObject.SetActive(false);
                class_4.gameObject.SetActive(false);

                name = class_1.sprite.name;
                break;
            case 1:
                class_1.gameObject.SetActive(false);
                class_2.gameObject.SetActive(true);
                class_3.gameObject.SetActive(false);
                class_4.gameObject.SetActive(false);

                name = class_2.sprite.name;
                break;
            case 2:
                class_1.gameObject.SetActive(false);
                class_2.gameObject.SetActive(false);
                class_3.gameObject.SetActive(true);
                class_4.gameObject.SetActive(false);

                name = class_3.sprite.name;
                break;
            case 3:
                class_1.gameObject.SetActive(false);
                class_2.gameObject.SetActive(false);
                class_3.gameObject.SetActive(false);
                class_4.gameObject.SetActive(true);

                name = class_4.sprite.name;
                break;
        }
        className.text = name;
    }

    public void Right()
    {
        if (playerClassNumber < 3) playerClassNumber++;

        onChange = true;
    }

    public void Left()
    {
        if (playerClassNumber > 0) playerClassNumber--;

        onChange = true;
    }

    public void SetClass()
    {
        CP["Class"] = playerClassNumber;
    }
}
