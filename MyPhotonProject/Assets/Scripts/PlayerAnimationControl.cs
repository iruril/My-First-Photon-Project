using UnityEngine;
using Photon.Pun;

public class PlayerAnimationControl : MonoBehaviourPun
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("PlayAnimatorManager가 Animator 컴포넌트가 없습니다.", this);
        }
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!animator)
        {
            return;
        }

        float x = this.GetComponent<PlayerControl>().joy.Horizontal;
        float z = this.GetComponent<PlayerControl>().joy.Vertical;
        
        animator.SetFloat("Speed", x * x + z * z);

        if (x == 0 && z == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }
}
