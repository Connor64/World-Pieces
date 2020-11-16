using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool grounded = false;

    public GameObject cube;
    public Material material;

    private void Awake() {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 120;
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Locked;
        crosshair = new Vector2(Screen.width / 2, Screen.height / 2);
        _yRot = 0.0f;
    }

    void LateUpdate() {
        grounded = isGrounded();
        if (grounded) {
            jumped = false;
        }

        //cameraRotation();

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
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.fixedDeltaTime;
        _yRot -= mouseY;
        _yRot = Mathf.Clamp(_yRot, -90, 90);
        Vector3 currentRotation = Camera.main.transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(_yRot, currentRotation.y, currentRotation.z));
        //Camera.main.transform.Rotate(_xInput, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void movement() {
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

        if (Input.GetKey(KeyCode.Space) && grounded) {
            rb.AddForce(transform.up * jumpSpeed);
            jumped = true;
        }

        if (!jumped && isGrounded() && grounded) {
            rb.AddForce(transform.up * 7.5f);
        }

        Vector3 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude > speedLimit) {
            rb.velocity = currentVelocity.normalized * speedLimit;
            if (currentVelocity.y < 0.01f) {
                rb.velocity = new Vector3(rb.velocity.x, currentVelocity.y, rb.velocity.z);
            }
        }

        // reloads mesh you are looking at (for debugging)
        if (Input.GetKeyDown(KeyCode.K)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(crosshair);
            if (Physics.Raycast(ray, out hit, 5)) {
                GameObject swpObject = hit.collider.gameObject;
                if (swpObject.layer == 8) {
                    print("test");
                    SmoothWP swp = swpObject.GetComponent<SmoothWP>();
                    //swp.setup();
                    swp.renderMesh();
                    swp.finalize();
                }
            }
        }
    }

    private bool isGrounded() {
        Debug.DrawRay(transform.position, -Vector3.up, Color.white, 2f);
        return Physics.Raycast(transform.position, -Vector3.up, 2f);
    }

    void blockInteract(bool breaking) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            GameObject awpObject = hit.collider.gameObject;
            if (awpObject.layer == 8) {
                AngularWP awp = awpObject.GetComponent<AngularWP>();
                int _x = Mathf.FloorToInt(hit.point.x);
                int _y = Mathf.FloorToInt(hit.point.y);
                int _z = Mathf.FloorToInt(hit.point.z);
                if (breaking && !breakPause) {
                    //TerrainModifier.destroyVoxel(_x - Mathf.FloorToInt(awpObject.transform.position.x), _y, _z - Mathf.FloorToInt(awpObject.transform.position.z), hit.collider.gameObject.GetComponent<AngularWP>());
                    //TerrainModifier.hideVoxel(hit.point.x - Mathf.FloorToInt(awpObject.transform.position.x), hit.point.y, hit.point.z - Mathf.FloorToInt(awpObject.transform.position.z), hit.collider.gameObject.GetComponent<AngularWP>());
                    string cornerValues = "";
                    foreach (int thingy in awpObject.GetComponent<AngularWP>().voxelData[_x - Mathf.FloorToInt(awpObject.transform.position.x), _z - Mathf.FloorToInt(awpObject.transform.position.z), _y].cornersExposed) {
                        cornerValues += thingy + ", ";
                    }
                    print(cornerValues);
                    TerrainModifier.updateVoxel(_x - Mathf.FloorToInt(awpObject.transform.position.x), _y - 1, _z - Mathf.FloorToInt(awpObject.transform.position.z), awp, true);
                } else if (!breaking && !placePause) {
                    _y++;
                    TerrainModifier.placeVoxel(_x - Mathf.FloorToInt(awpObject.transform.position.x), _y, _z - Mathf.FloorToInt(awpObject.transform.position.z), awpObject.GetComponent<AngularWP>());
                }
            }
        }
    }
    //void destroyVoxel(float x, float y, float z, SmoothWP swp) {
    //    int _x = Mathf.FloorToInt(x);
    //    int _y = Mathf.FloorToInt(y);
    //    int _z = Mathf.FloorToInt(z);
    //    float scalar = swp.voxelScalar;
    //    Vector3[] voxelCorners = new Vector3[] {
    //        new Vector3(_x, _y, _z),
    //        new Vector3(_x + scalar, _y, _z),
    //        new Vector3(_x, _y + scalar, _z),
    //        new Vector3(_x + scalar, _y + scalar, _z),

    //        new Vector3(_x + scalar, _y, _z + scalar),
    //        new Vector3(_x + scalar, _y + scalar, _z + scalar),
    //        new Vector3(_x, _y, _z + scalar),
    //        new Vector3(_x, _y + scalar, _z + scalar)
    //    };
    //    HashSet<int> corners = swp.data[_x, _z, _y].cornersExposed;
    //    Vector3 cursorPosition = new Vector3(x, y, z);

    //    int cornerSelected = 10;
    //    float distance = 1;

    //    for (int i = 0; i < voxelCorners.Length; i++) {
    //        float newDistance = Vector3.Distance(voxelCorners[i], cursorPosition);
    //        if (newDistance < distance && !corners.Contains(i)) {
    //            cornerSelected = i;
    //            distance = newDistance;
    //        }
    //    }

    //    if (cornerSelected != 10) {
    //        TerrainModifier.removeCorners(_x, _y, _z, cornerSelected, swp);
    //    } else {
    //        print("No corner able to be selected");
    //    }
    //    swp.renderMesh();
    //    swp.finalize();
    //    swp.updatePhysics();
    //}


    //void placeBlock(float x, float y, float z, SmoothWP swp) {
    //    int _x = Mathf.FloorToInt(x);
    //    int _y = Mathf.FloorToInt(y);
    //    int _z = Mathf.FloorToInt(z);

    //    if (swp.data[_x, _z, _y].landType == LandType.Air) {
    //        print(new Vector3(x, y, z));
    //        swp.data[_x, _z, _y].landType = LandType.Dirt;
    //        swp.data[_x, _z, _y].position = new Vector3(x, y, z);
    //        swp.data[_x, _z, _y].cornersExposed = new HashSet<int> { 1, 3, 7, 5 };
    //        //swp.setup();
    //        swp.renderMesh();
    //        swp.finalize();
    //    } else {
    //        print("NOT AIR");
    //    }
    //}
}