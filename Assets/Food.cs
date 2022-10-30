using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public BoxCollider2D Grid;

    // Start is called before the first frame update
    void Start()
    {
        RandomizePosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate(){
    }

    private void RandomizePosition(){
        Bounds bounds = this.Grid.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 1.0f);

        //Debug.Log(bounds.min.x);
    }

    private void OnTriggerEnter2D(Collider2D other){
        RandomizePosition();
    }
}
