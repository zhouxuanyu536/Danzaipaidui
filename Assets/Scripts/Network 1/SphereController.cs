using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] private GameObject SphereNoticeImage;
    [SerializeField] private NoticeType sphereNoticeType;
    [SerializeField] private float sphereRadius = 3f;

    // Start is called before the first frame update
    void Start()
    {
        SphereNoticeImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, LayerMask.GetMask("Player"));
        bool isIt = false;
        if (Player.LocalInstance == null) return;
        foreach(Collider collider in colliders)
        {
            if(collider.transform.parent.GetComponent<Player>() == Player.LocalInstance)
            {
                isIt = true;
                
            }
        }
        if (!isIt)
        {
        }
    }
}
