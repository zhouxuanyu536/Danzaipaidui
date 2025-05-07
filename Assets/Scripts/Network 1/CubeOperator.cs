using Unity.Netcode;
using UnityEngine;

public class CubeOperator : NetworkBehaviour
{
    private Renderer renderer;

    [SerializeField] private Material OriginalYellowMaterial;
    [SerializeField] private Material BlueMaterial;
    [SerializeField] private Material PurpleMaterial;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = OriginalYellowMaterial;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangeCubeColorServerRpc()
    {
        ChangeCubeColorClientRpc();
    }

    [ClientRpc]
    public void ChangeCubeColorClientRpc()
    {
        if(renderer.sharedMaterial == BlueMaterial)
        {
            renderer.sharedMaterial = PurpleMaterial;
        }
        else
        {
            renderer.sharedMaterial = BlueMaterial;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void MoveCubeServerRpc()
    {
        MoveCubeClientRpc();
    }
    [ClientRpc]
    public void MoveCubeClientRpc()
    {
        Vector3 pos = transform.position;
        pos.x -= 0.3f;
        transform.position = pos;
    }
}
