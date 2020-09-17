using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public float lookSensitivity;
    public float playerSpeed;
    public float jumpSpeed;
    public float speedLimit;
    public LayerMask mask;

    private float _yRot;
    private Rigidbody rb;
    private bool breakPause = false;
    private bool placePause = false;
    private bool jumped = false;
    private Vector2 crosshair;

    public GameObject cube;
    public Material material;

    void Start() {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        crosshair = new Vector2(Screen.width / 2, Screen.height / 2);
        _yRot = 0.0f;
    }

    void Update() {
        cameraRotation();
        if (Input.GetMouseButtonDown(0)) {
            blockInteract(true);
        } else if (Input.GetMouseButtonUp(0)) {
            breakPause = false;
        }

        if (Input.GetMouseButtonDown(1)) {
            blockInteract(false);
        } else if (Input.GetMouseButtonUp(1)) {
            placePause = false;
        }
    }

    void FixedUpdate() {
        movement();
    }

    void cameraRotation() {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        _yRot -= mouseY;
        _yRot = Mathf.Clamp(_yRot, -90, 90);
        Vector3 currentRotation = Camera.main.transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(_yRot, currentRotation.y, currentRotation.z));
        //Camera.main.transform.Rotate(_xInput, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void movement() {
        //float forwardMovement = Input.GetAxis("Vertical");
        //float sideMovement = Input.GetAxis("Horizontal");
        //Vector3 movement = new Vector3(sideMovement, 0, forwardMovement);
        //rb.AddForce(movement * 100 * playerSpeed * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) {
            rb.AddForce(transform.forward * playerSpeed);
        } else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) {
            rb.AddForce(transform.forward * -playerSpeed);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            rb.AddForce(transform.right * -playerSpeed);
        } else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
            rb.AddForce(transform.right * playerSpeed);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded()) {
            rb.AddForce(transform.up * jumpSpeed);
            jumped = true;
        }

        Vector3 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude > speedLimit) {
            rb.velocity = currentVelocity.normalized * speedLimit;
            if (currentVelocity.y < 0.01f) {
                rb.velocity = new Vector3(rb.velocity.x, currentVelocity.y, rb.velocity.z);
            }
        }
    }

    private bool isGrounded() {
        Debug.DrawRay(transform.position, -Vector3.up, Color.white, 2f);
        return Physics.Raycast(transform.position, -Vector3.up, 2f);
    }

    void blockInteract(bool breaking) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(crosshair);
        if (Physics.Raycast(ray, out hit, 5)) {
            if (hit.collider.gameObject.layer == 8) {
                if (breaking && !breakPause) {
                    hit.collider.gameObject.GetComponent<Cube>().destoryCube();
                } else if (!breaking && !placePause) {
                    //print(hit.triangleIndex);
                    int triangle = hit.triangleIndex;
                    //if (triangle == 0 || triangle == 1) {
                    //    placeBlock(hit.collider.gameObject.transform.position - transform.forward);
                    //} else if (triangle == 4 || triangle == 7) {
                    //    placeBlock(hit.collider.gameObject.transform.position + transform.right);
                    //} else if (triangle == 10 || triangle == 11) {
                    //    placeBlock(hit.collider.gameObject.transform.position + transform.forward);
                    //} else if (triangle == 8 || triangle == 2) {
                    //    placeBlock(hit.collider.gameObject.transform.position - transform.forward);
                    //} else if (triangle == 3 || triangle == 6) {
                    //    placeBlock(hit.collider.gameObject.transform.position + transform.up);
                    //} else if (triangle == 5 || triangle == 9) {
                    //    placeBlock(hit.collider.gameObject.transform.position - transform.up);
                    //}
                    //Vector3 position = new Vector3(Mathf.FloorToInt(hit.transform.position.x), Mathf.FloorToInt(hit.transform.position.y), Mathf.FloorToInt(hit.transform.position.z));
                    int _x = Mathf.FloorToInt(hit.point.x);
                    int _y = Mathf.FloorToInt(hit.point.y);
                    int _z = Mathf.FloorToInt(hit.point.z);

                    placeBlock(_x, _y, _z, hit.collider.gameObject.GetComponent<WorldPiece>());
                    //switch (hit.triangleIndex) {
                    //    case 0:
                    //    case 1:
                    //        placeBlock(hit.collider.gameObject.transform.position - Vector3.forward);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    case 4:
                    //    case 7:
                    //        placeBlock(hit.collider.gameObject.transform.position + Vector3.right);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    case 10:
                    //    case 11:
                    //        placeBlock(hit.collider.gameObject.transform.position + Vector3.forward);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    case 8:
                    //    case 2:
                    //        placeBlock(hit.collider.gameObject.transform.position - Vector3.right);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    case 3:
                    //    case 6:
                    //        placeBlock(hit.collider.gameObject.transform.position + Vector3.up);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    case 5:
                    //    case 9:
                    //        placeBlock(hit.collider.gameObject.transform.position - Vector3.up);
                    //        //print(hit.collider.gameObject.transform.position);
                    //        break;
                    //    default:
                    //        Debug.LogError("Unable to find Triangle Index at: " + hit.collider.gameObject.transform.position);
                    //        break;
                    //}
                    ////placeBlock(hit.collider.gameObject.transform.position + transform.up);
                }
            }
        }
    }

    void placeBlock(int x, int y, int z, WorldPiece wp) {
        print(new Vector3(x, y, z));
        wp.data[x, z, y] = new Block(BlockType.Dirt, new Vector3(x, y, z));
        wp.renderMesh();
        wp.finalize();
        //var cubeObject = GameObject.Instantiate(cube).GetComponent<Cube>();
        //cubeObject.generateCube(position, material);
    }
}
