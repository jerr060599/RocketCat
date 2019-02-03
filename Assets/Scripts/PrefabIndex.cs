using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabIndex : MonoBehaviour {
    public GameObject explosion = null;

    public static PrefabIndex main = null;
    void Awake() {
        main = this;
    }
}
