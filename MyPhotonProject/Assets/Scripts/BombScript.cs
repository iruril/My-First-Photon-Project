using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BombScript : MonoBehaviourPun
{

    public float detonateTime = 2.0f;
    public float throwPower = 5.0f;
    public GameObject Explosion;
    bool isDetonated = false;
    GameObject _gameObject;

    void Start()
    {
        _gameObject = this.transform.Find("Model").gameObject;
        _gameObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * throwPower, ForceMode.Impulse);

        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        StartCoroutine(destroyBullet());
    }
    
    IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(detonateTime + 0.4f);
        this.GetComponent<PhotonView>().RPC("destroy", RpcTarget.AllBuffered);
    }

    void Update()
    {
        detonateTime -= Time.deltaTime;
        if(detonateTime < 0)
        {
            _gameObject.SetActive(false);
            if (!isDetonated)
            {
                detonate();
                isDetonated = true;
            }
        }
    }

    void detonate()
    {
        this.transform.position = _gameObject.transform.position;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
        Instantiate(Explosion, this.transform.position, Quaternion.LookRotation(Vector3.up));
    }

    [PunRPC]
    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
