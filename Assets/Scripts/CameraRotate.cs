using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotate : MonoBehaviour {
    public GameObject player;
    private Quaternion originalRot;
    private Vector2 originalPos;
    private bool beginRotation = false;
    private Rigidbody rb;

    private Transform camTransform;

    public bool invertVertical = false;

    public float followSpeed_x = 10f;
    public float followSpeed_y = 20f;

    void Start() {
        originalPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        rb = GetComponent<Rigidbody>();

        camTransform = Camera.main.transform;
    }

    void LateUpdate() {
        camTransform.LookAt(player.transform.position + Vector3.up * 2);
        cameraFollow();
        cameraPan();
    }

    void cameraFollow() {
        transform.position = Vector3.Slerp(transform.position, player.transform.position + Vector3.up * 2, Time.fixedDeltaTime);
    }

    void cameraPan() {
        if (Input.GetMouseButton(2)) {
            //float yMove = Mathf.Clamp((Input.GetAxis("Mouse Y") * followSpeed_y * Time.fixedDeltaTime) + transform.rotation.eulerAngles.x, 0, 75);
            float xMove = (Input.GetAxis("Mouse X") * followSpeed_x * Time.fixedDeltaTime) + transform.rotation.eulerAngles.y;

            //transform.rotation = Quaternion.Euler(yMove, xMove, 0);
            transform.rotation = Quaternion.Euler(0, xMove, 0);
            player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 45, 0);
        }

        if (Input.mouseScrollDelta.y > 0) {
            Camera.main.orthographicSize -= 0.25f;
            //camTransform.position = camTransform.position + camTransform.forward;
        } else if (Input.mouseScrollDelta.y < 0) {
            //camTransform.position = camTransform.position - camTransform.forward;
            Camera.main.orthographicSize += 0.25f;
        }
    }
}