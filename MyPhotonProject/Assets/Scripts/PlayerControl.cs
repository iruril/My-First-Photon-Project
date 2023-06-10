using UnityEngine;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            //this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion

    #region Private Fields
    Rigidbody rigid;
    Animator anim;
    Vector3 moveVec;
    #endregion

    #region Public Fields
    public float speed = 8.0f;
    public float jumpPower = 5.0f;
    [Tooltip("플레이어의 현재 체력")]
    public float Health = 1f;
    public static GameObject LocalPlayerInstance;
    public GameObject Bomb;
    public bool isJumping;

    public FloatingJoystick joy;
    public FixedButton jumpButton;
    public BombButton bombButton;
    #endregion


    #region MonoBehaviour Callbacks
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        if (photonView.IsMine)
        {
            PlayerControl.LocalPlayerInstance = this.gameObject;
            this.transform.Find("Canvas").GetComponent<Canvas>().gameObject.SetActive(true);
            Transform _gameObject = this.transform.Find("Canvas").GetComponent<Canvas>().transform;
            joy = _gameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
            jumpButton = _gameObject.Find("Jump Button").GetComponent<FixedButton>();
            jumpButton.SetPlayer(this.gameObject.GetComponent<PlayerControl>());
            bombButton = _gameObject.Find("Bomb Button").GetComponent<BombButton>();
            bombButton.SetPlayer(this.gameObject.GetComponent<PlayerControl>());
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        isJumping = false;
        
        CameraControl _cameraControl = this.gameObject.GetComponent<CameraControl>();
        if (_cameraControl != null)
        {
            if (photonView.IsMine)
            {
                _cameraControl.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move();
            if (Health <= 0f)
            {
                this.GetComponent<PhotonView>().RPC("destroy", RpcTarget.AllBuffered);
                GameManager.Instance.Respawn();
                Health = 1.0f;
            }
        }
    }

    void LateUpdate()
    {
        //anim.SetFloat("Move", moveVec.sqrMagnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            anim.SetBool("Jump", isJumping);

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!other.name.Contains("Bomb"))
        {
            return;
        }
        Health -= 0.4f;
    }
    void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }
    #endregion

    #region Private Methods
    void Move()
    {
        // 1. Input Value
        float x = joy.Horizontal;
        float z = joy.Vertical;

        // 2. Move Position 
        moveVec = new Vector3(x, 0, z) * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVec);

        if (moveVec.sqrMagnitude == 0)
            return; // #. No input = No Rotation

        // 3. Move Rotation
        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);
        rigid.MoveRotation(moveQuat);
    }
    #endregion

    #region Public Methods
    public void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("Jump", isJumping);
        }
        else
        {
            return;
        }
    }
    public void Attack()
    {
        GameObject bombThrow =  PhotonNetwork.Instantiate(Bomb.name, this.transform.position + Vector3.up * 3.0f, this.transform.rotation);
        //Instantiate(Bomb, this.transform.position + Vector3.up * 3.0f, this.transform.rotation);
    }
    #endregion

    [PunRPC]
    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
