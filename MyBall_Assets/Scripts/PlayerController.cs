using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerController : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Debug.Log(Random.value);
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0f)
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, 0.5f, -4f);
        }

        target.localPosition = new Vector3(Random.value * 5 - 1, 0.5f, Random.value * 5 - 1);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 signal = Vector3.zero;
        signal.x = actions.ContinuousActions[0];
        signal.z = actions.ContinuousActions[1];

        rb.AddForce(signal * speed);

        // 거리 구하기
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= 1.4f)
        {
            SetReward(1f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0f)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxis("Horizontal");
        continuous[1] = Input.GetAxis("Vertical");
    }

}
