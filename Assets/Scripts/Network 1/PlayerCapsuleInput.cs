using UnityEngine;

public class PlayerCapsuleInput : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    private float jumpForce;
    private Vector3 speedVec;
    private float hor, ver;
    private bool OnGround;
    // Start is called before the first frame update
    void Start()
    {
        speedVec = Vector3.zero;
        OnGround = true;
        jumpForce = 5f;
    }

    // Update is called once per frame
    void Update() //FixedUpdate下用GetKey,Update用GetKeyDown
    {
        if (GameManager.Instance.GetGameState() != 2) return;
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        speedVec.x = speed * hor;
        speedVec.z = speed * ver;

        if (Input.GetKeyDown(KeyCode.Space) && OnGround)
        {
            rb.AddForce(Vector3.up * rb.mass * jumpForce, ForceMode.Impulse);
            OnGround = false;
        }
        //受重力影响
        if (Physics.Raycast(transform.GetChild(0).position, Vector3.down, 1.01f, LayerMask.GetMask("Floor")))
        {
            OnGround = true;
        }
        else
        {
            OnGround = false;
        }

        transform.position += speedVec * Time.deltaTime;
        
        
    }
}
