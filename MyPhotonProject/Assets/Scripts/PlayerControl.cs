using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion

    #region Private Fields
    Rigidbody rigid;
    Animator anim;
    Vector3 moveVec;
    WaitForSeconds attackCoolDown;
    #endregion

    #region Public Fields
    public float speed = 8.0f;
    public float jumpPower = 5.0f;
    [Tooltip("플레이어의 현재 체력")]
    public float Health = 1f;
    public float attackCoolTime = 1.0f;
    public static GameObject LocalPlayerInstance;
    public GameObject Bomb;
    public bool isJumping;
    public bool isDead;
    public bool isAttacking;

    public int mesh;

    public FloatingJoystick joy;
    public FixedButton jumpButton;
    public BombButton bombButton;

    public HPBarController HPBar;
    #endregion


    #region MonoBehaviour Callbacks
    void Awake()
    {
        attackCoolDown = new WaitForSeconds(attackCoolTime);
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        isDead = false;
        
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

        if (photonView.IsMine)
        {
            HPBar.nickName.text = this.photonView.Owner.NickName;
        }

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
                Die();
            }
        }
    }

    void LateUpdate()
    {
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            if (this.anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.80f)
            {
                anim.SetBool("Attack", false);
            }
        }
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
        Health -= 0.4f; //나중에 IDamageable 만들어서 TakeHit 호출할 수 있도록 하자.
        this.HPBar.TakeHit(0.4f); //얘는 이후에 인터페이스 구축 이후에 인터페이스의 TakeHit 안으로 옮기자.
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
        if (this.isDead)
        {
            return;
        }
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

    void Die()
    {
        isDead = true;
        joy.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        bombButton.gameObject.SetActive(false);

        this.anim.SetBool("Die", isDead);
        Invoke("Respawn", 5.0f);
    }

    void Respawn()
    {
        this.GetComponent<PhotonView>().RPC("destroy", RpcTarget.AllBuffered);
        GameManager.Instance.Respawn();

        isDead = false;
        joy.gameObject.SetActive(true);
        jumpButton.gameObject.SetActive(true);
        bombButton.gameObject.SetActive(true);
        Health = 1.0f;
    }

    private IEnumerator AtkCoolDown()
    {
        isAttacking = true;
        anim.SetBool("Attack", isAttacking);
        yield return attackCoolDown;
        isAttacking = false;
    }
    #endregion

    #region Public Methods
    public void Jump()
    {
        if (this.isDead)
        {
            return;
        }
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
        if (this.isDead || this.isAttacking)
        {
            return;
        }
        
        StartCoroutine(AtkCoolDown());
        GameObject bombThrow =  PhotonNetwork.Instantiate(Bomb.name, this.transform.position + Vector3.up * 3.0f, this.transform.rotation);
    }
    public void meshChange(int mesh)
    {
        this.GetComponent<PlayerMesh>().GetComponent<PhotonView>().RPC("ChangeClass", RpcTarget.AllBuffered, mesh);
    }
    #endregion

    [PunRPC]
    void destroy()
    {
        Destroy(this.gameObject);
    }
}
