using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cunchuqi : MonoBehaviour
{
    public Vector3 playerpositioncollected;
    // Start is called before the first frame update
    void Start()
    {
        playerpositioncollected = transform.position;
        DontDestroyOnLoad(this);
    }
    public void setPositionToWhatIsCollected(GameObject player)
    {
        player.transform.position = playerpositioncollected;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
