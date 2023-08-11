using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cunchuqi : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerpositioncollected;
    public Transform origin;
    // Start is called before the first frame update
    void Awake()
    {
        playerpositioncollected = origin.position;
        DontDestroyOnLoad(this);
    }
    public void setPositionToWhatIsCollected()
    {
        player.transform.position = playerpositioncollected;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
