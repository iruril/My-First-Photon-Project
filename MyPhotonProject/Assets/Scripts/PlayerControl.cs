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
    FloatingJoystick joy;
    FixedButton jumpButton;
    //Animator anim;
    Vector3 moveVec;
    bool isJumping;
    #endregion

    #region Public Fields
    public float speed = 8.0f;
    public float jumpPower = 5.0f;
    [Tooltip("플레이어의 현재 체력")]
    public float Health = 1f;
    public static GameObject LocalPlayerInstance;
    #endregion


    #region MonoBehaviour Callbacks
    void Awake()
    {
        joy = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
        jumpButton = GameObject.Find("Jump Button").GetComponent<FixedButton>();
        jumpButton.SetPlayer(this.gameObject.GetComponent<PlayerControl>());
        rigid = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            PlayerControl.LocalPlayerInstance = this.gameObject;
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
                GameManager.Instance.LeaveRoom();
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
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
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
        }
        else
        {
            return;
        }
    }
    #endregion
}
