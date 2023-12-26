using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvas : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - playerTransform.position);
        Quaternion currentRotation = transform.rotation;

        // Ignore rotation around X-axis
        //targetRotation.eulerAngles = new Vector3(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
        targetRotation.eulerAngles = new Vector3(0f, targetRotation.eulerAngles.y, 0f);

        transform.rotation = targetRotation;
    }
}
