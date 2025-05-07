using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorActiveHandler : MonoBehaviour
{
    public List<GameObject> allFloors;
    private float gameTime;
    [SerializeField] private LevelDetails localLevelDetails;
    private bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform floor in transform)
        {
            allFloors.Add(floor.gameObject);
        }
        gameTime = 0;
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused) return;
        gameTime += Time.deltaTime;
        if(gameTime % 8 <= 6 && !isActive)
        {
            localLevelDetails.SetFloorActiveServerRpc(true);
            isActive = true;
        }
        else if (gameTime % 8 > 6 && isActive)
        {
            localLevelDetails.SetFloorActiveServerRpc(false);
            isActive = false;
        }
    }
}
