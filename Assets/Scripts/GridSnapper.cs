/*
 * A script that snaps obejcts to an invisible grid
 */
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour {
    public Vector3 offset = Vector3.zero;
    public Vector3 gridSize = Vector3.one;
    void Awake() {
        if (Application.isPlaying)
            DestroyImmediate(this);
    }

    void Update() {
        if (transform.hasChanged) {
            OnValidate();
            transform.hasChanged = false;
        }
    }

    private void OnValidate() {//This is called when a value in the inspector is changed
        transform.localPosition = new Vector3(Mathf.Round((transform.localPosition.x - offset.x) * gridSize.x) / gridSize.x,
                                                Mathf.Round((transform.localPosition.y - offset.y) * gridSize.y) / gridSize.y,
                                                Mathf.Round((transform.localPosition.z - offset.z) * gridSize.z) / gridSize.z) + offset;
    }
}
#endif