using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWater : MonoBehaviour
{
    [SerializeField]
    private GameObject water; //Plane 20 x 20
    // Start is called before the first frame update
    void Start()
    {
        Vector2 OriginalPos = new Vector2(-400, -400);
        if(water != null)
        {
            for(int i = 0;i < 50; i++)
            {
                for(int j = 0;j < 50; j++)
                {
                    Object.Instantiate(water, new Vector3(OriginalPos.x + 18 * i, 0,OriginalPos.y + 18 * j), Quaternion.identity,this.transform);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
