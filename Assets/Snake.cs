using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class Snake : Agent
{
    private Vector2 direction = Vector2.right;

    public List<Transform> body;

    public Transform prefab;

    public Transform TargetTransform;

    private const float MAX_DISTANCE = 42.047592f;
    private enum ACTIONS {
        LEFT = 1,
        UP = 2,
        RIGHT = 3,
        DOWN = 4
    }

    // Start is called before the first frame update
    void Start()
    {
        body = new List<Transform>();
        body.Add(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)){
            direction = direction == Vector2.down ? Vector2.down : Vector2.up;
        }else if(Input.GetKeyDown(KeyCode.S)){
            direction = direction == Vector2.up ? Vector2.up : Vector2.down;
        }else if(Input.GetKeyDown(KeyCode.D)){
            direction = direction == Vector2.left ? Vector2.left : Vector2.right;
        }else if(Input.GetKeyDown(KeyCode.A)){
            direction = direction == Vector2.right ? Vector2.right : Vector2.left;
        }

        float distance_scaled = Vector3.Distance(TargetTransform.localPosition, transform.localPosition) / MAX_DISTANCE;

        AddReward(-distance_scaled / 10);
    }


    private void FixedUpdate(){

        for(int i = body.Count - 1; i > 0; i--){
            body[i].position = body[i - 1].position;
        }

        this.transform.localPosition = new Vector3(
            Mathf.Round(this.transform.localPosition.x) + direction.x,
            Mathf.Round(this.transform.localPosition.y) + direction.y,
            1.0f
        );
    }

    private void Grow(){
        Transform segment = Instantiate(this.prefab);
        segment.position = body[body.Count - 1].position;

        body.Add(segment);
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Food"){
            AddReward(1);
            Grow();
        }
        if(other.tag == "Wall"){
            AddReward(-1);
            EndEpisode();
            Reset();
        }
    }

    private void Reset(){
        for(int i=1; i<body.Count; i++){
            Destroy(body[i].gameObject);
        }
        direction = Vector2.zero;
        body.Clear();
        body.Add(this.transform);

        this.transform.localPosition = new Vector3(
            0,
            0,
            1.0f
        );
    }

    public override void OnEpisodeBegin() {
        //Reset();
    }

    public override void CollectObservations(VectorSensor sensor) {

        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        sensor.AddObservation(TargetTransform.localPosition.x);
        sensor.AddObservation(TargetTransform.localPosition.y);

        sensor.AddObservation(Vector3.Distance(TargetTransform.localPosition, transform.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        var action = actions.DiscreteActions[0];
        switch (action) {
            case (int)ACTIONS.UP:
                direction = direction == Vector2.down ? Vector2.down : Vector2.up;
                break;
            case (int)ACTIONS.DOWN:
                direction = direction == Vector2.up ? Vector2.up : Vector2.down;
                break;
            case (int)ACTIONS.RIGHT:
                direction = direction == Vector2.left ? Vector2.left : Vector2.right;
                break;
            case (int)ACTIONS.LEFT:
                direction = direction == Vector2.right ? Vector2.right : Vector2.left;
                break;   
        }

        
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var actions = actionsOut.DiscreteActions;

        if(Input.GetKeyDown("w")){
            actions[0] = (int)ACTIONS.UP;
        }
        if(Input.GetKeyDown("s")){
            actions[0] = (int)ACTIONS.DOWN;
        }
        if(Input.GetKeyDown("a")){
            actions[0] = (int)ACTIONS.LEFT;
        }
        if(Input.GetKeyDown("d")){
            actions[0] = (int)ACTIONS.RIGHT;
        }

    }
}
