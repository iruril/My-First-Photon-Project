using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class HPBarController : MonoBehaviourPun, IPunObservable
{
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fillAmount);
            stream.SendNext(HP.fillAmount);
            stream.SendNext(postHP.fillAmount);
            stream.SendNext(realHpHit);
            stream.SendNext(nickName.text);
        }
        else
        {
            this.fillAmount = (float)stream.ReceiveNext();
            this.HP.fillAmount = (float)stream.ReceiveNext();
            this.postHP.fillAmount = (float)stream.ReceiveNext();
            this.realHpHit = (bool)stream.ReceiveNext();
            this.nickName.text = (string)stream.ReceiveNext();
        }
    }

    #endregion

    public Image HPbar;
    public Image HP;
    public Image postHP;
    public Text nickName;

    public bool realHpHit = false;
    public float fillAmount = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        HPbar = this.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        nickName = this.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        postHP = HPbar.transform.GetChild(0).GetComponent<Image>();
        HP = HPbar.transform.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.fillAmount > 0)
        {
            HP.fillAmount = Mathf.Lerp(HP.fillAmount, this.fillAmount, Time.deltaTime * 5f);
            if (realHpHit)
            {
                postHP.fillAmount = Mathf.Lerp(postHP.fillAmount, HP.fillAmount, Time.deltaTime * 10f);
                if(HP.fillAmount >= postHP.fillAmount - 0.01f)
                {
                    realHpHit = false;
                    postHP.fillAmount = this.fillAmount;
                }
            }
        }
        else
        {
            HP.fillAmount = 0;
            postHP.fillAmount = 0;
        }
    }

    public void TakeHit(float damage)
    {
        this.fillAmount -= damage;
        Invoke("realHpAction", 0.5f);
    }

    void realHpAction()
    {
        realHpHit = true;
    }
}
