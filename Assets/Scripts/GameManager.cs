using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm{ get; private set; }

    public Transform deadPivot;

    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null)
            gm = this;
        else
            Destroy(this.gameObject);
    }
}
