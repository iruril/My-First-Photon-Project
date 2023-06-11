using UnityEngine;
using Photon.Pun;

public class PlayerMesh : MonoBehaviourPun
{
    public Material[] CharacterMaterial = new Material[4];
    public Mesh[] CharacterMesh = new Mesh[4];
    public int skinIndex = 0;

    SkinnedMeshRenderer skinRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        skinRenderer = this.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void ChangeClass(int idx)
    {
        skinIndex = idx;
        skinRenderer.material = CharacterMaterial[skinIndex]; // Change Material
        skinRenderer.sharedMesh = CharacterMesh[skinIndex];   // Change Mesh
    }
}
