/*
 * A script for contorlling the camera to follow a GameObject
 * 
 */
using UnityEngine;

public class CamController : MonoBehaviour {
    [Range(0, 10)]                          //This tag tells the inspector to add a slider for the following variable. 
    public float followAccel = 3f;          //A value greater than 0 for how fast the camera moves;
    public GameObject follow = null;        //The target the camera will be following.
    public Vector3 offset = Vector3.zero;   //The the distance the camera will try to maintain with the target.

    void Update() {
        if (follow) { //If the object is not null.
            Vector3 dpos = follow.transform.position + offset - transform.position; //Find the delta to the target position
            dpos *= 1 - Mathf.Exp(-followAccel * Time.deltaTime);   //Lerp
            transform.position += dpos;                             //Move the camera position
        }
    }

    private void OnValidate() {
        followAccel = Mathf.Max(0, followAccel);                    //Input validation
    }
}
