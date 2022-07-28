using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float MovementSpeed = 1.0f;

    [SerializeField]
    private float ZoomSpeed = 1.0f;

    [SerializeField]
    private float SprintMultiplier = 2.0f;

    [SerializeField]
    private Vector2 ZoomLim = new Vector2(1.0f, 10.0f);

    private float Theta = 0.0f;
    private float Phi = Mathf.PI / 2 - Mathf.Deg2Rad * 30.0f;
    private float Zoom = 3.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        float dz = Input.GetAxis("Zoom");
        if (Input.GetAxis("Sprint") > 0)
        {
            dx *= SprintMultiplier;
            dy *= SprintMultiplier;
            dz *= SprintMultiplier;
        }

        Theta += dx * MovementSpeed * Time.fixedDeltaTime;
        Phi -= dy * MovementSpeed * Time.fixedDeltaTime;
        Phi = Mathf.Clamp(Phi, 0.1f, Mathf.PI - 0.1f);
        Zoom -= dz * ZoomSpeed * Time.fixedDeltaTime;
        Zoom = Mathf.Clamp(Zoom, ZoomLim.x, ZoomLim.y);

        gameObject.transform.position = new Vector3(Mathf.Cos(Theta) * Mathf.Sin(Phi),  
                                                    Mathf.Cos(Phi),
                                                    Mathf.Sin(Theta) * Mathf.Sin(Phi));
        gameObject.transform.position *= Zoom;

        gameObject.transform.LookAt(Vector3.zero, Vector3.up);
    }
}
