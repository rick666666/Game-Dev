using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Piece;
    public bool end = false;

    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        Piece = GameObject.FindGameObjectsWithTag("piece");
        
        for (int i = 0; i < Piece.Length; i++) {
            if (Piece[i].transform.position.y < 0.5) {
                Debug.Log("yoyoyo");
            }
        }
    }

    public void Reset()
    {
        for (int i = 0; i < Piece.Length; i++)
        {
            Piece[i] = null;
        }
        end = false;
    }
}
