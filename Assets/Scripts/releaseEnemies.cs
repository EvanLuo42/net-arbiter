using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class releaseEnemies : MonoBehaviour
{
    [SerializeField] private GameObject child;
    private Vector3 ChildPosition;
    private Quaternion ChildRotation;
    public float time = 4;
    // Start is called before the first frame update

    public void SetChildPositionAndRotationAndComBackForLife(Vector3 position,Quaternion rotation)
    {
        ChildPosition = position;
        ChildRotation = rotation;
        StartCoroutine(InvokePro(child, position, rotation, time));
    }
    public IEnumerator InvokePro(GameObject child, Vector3 position,Quaternion rotation,float time)
    {
        //Debug.Log("invokepro");
        yield return new WaitForSeconds(time);
        Instantiate(child, position, rotation, transform);
        yield return null;
    }
}
