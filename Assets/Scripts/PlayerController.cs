using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public float Speed, Gravity, JumpHeight;
    private CharacterController cc;
    public GameObject Cameraman, RippleCamera;
    private float CameraY, GravityForce, Zoom = -7;
    private RaycastHit isGround;
    public ParticleSystem ripple;
    [SerializeField]private float VelocityXZ, VelocityY;
    private Vector3 PlayerPos;
    private bool inWater;
    public GameObject Cube;
    Vector3 MousePos;
    private Vector3 offset;
    public Text _touchCounter;
    // Start is called before the first frame update yup
    void Start()
    {
        Application.targetFrameRate = 60;
        cc = GetComponent<CharacterController>();
        // Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
     // Update is called once per frame
    void Update()
    { 
 #if UNITY_EDITOR_WIN
        if (Input.GetKey(KeyCode.Mouse0))
        {
              RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                 if (hit.collider.gameObject.tag == "Plane")
                {
                    MousePos = hit.point;
                      Cube.transform.position = MousePos;
                     print("Hited with plane");
                 }
            } 
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
             Cube.transform.position = new Vector3(MousePos.x, MousePos.y +2, MousePos.z);
         }
#endif

#if UNITY_ANDROID

    

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                 // Construct a ray from the current touch coordinates
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);  
                // Create a particle if hit
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.collider.gameObject.tag == "Plane")
                    {
                        _touchCounter.text = "plane hitted";
                        MousePos = hit.point;
                        Cube.transform.position = MousePos;
                    }    
                }   
            }  

            //if (hit.collider.gameObject.tag == "Plane")
            //{
            //    _touchCounter.text = "hited with plane";
            //    switch (touch.phase)
            //    {
            //        case TouchPhase.Began:
            //            Cube.transform.position = hit.point;
            //          

            //            break;
            //        case TouchPhase.Moved:
            //            Cube.transform.position = hit.point;
            //            _touchCounter.text = "Moving";
            //            break;
            //    }
            //}
        }
        else
        {
            Cube.transform.position = new Vector3(MousePos.x, MousePos.y + 2, MousePos.z);
        }
 #endif
        // PlayerMovement();
        //  CameraControl();
        //  if (Input.GetKeyDown(KeyCode.Space)) Jumping();
        /* 
    if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
     VelocityXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(PlayerPos.x, 0, PlayerPos.z));
    VelocityY = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, PlayerPos.y, 0));
    PlayerPos = transform.position;
     //RippleCamera.transform.position = transform.position + Vector3.up * 10;
    if(isGround.collider) ripple.transform.position = transform.position + transform.forward;
    else ripple.transform.position = transform.position;
    Shader.SetGlobalVector("_Player", transform.position);
    */
    }
    void PlayerMovement()
    {
        Vector3 camRight = Cameraman.transform.right;
        Vector3 camForward = Cameraman.transform.forward;
        camRight.y = 0;
        camForward.y = 0;
         Vector3 move = camRight.normalized * Input.GetAxis("Horizontal") + camForward.normalized * Input.GetAxis("Vertical");
        cc.Move(move.normalized * Time.deltaTime * 10 * Speed * ((Input.GetKey(KeyCode.LeftShift)?2:1)));
        if(move.magnitude > 0) transform.forward = move.normalized;

        GravityForce -= Gravity * Time.deltaTime * 5;
        if(isGround.collider && GravityForce < -2) GravityForce = -2;
        else if(GravityForce < -99) GravityForce = -99;
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));

        if (inWater) ripple.gameObject.SetActive(true);
        else ripple.gameObject.SetActive(false);
        Physics.Raycast(transform.position, Vector3.down, out isGround, 2.7f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector3.down* 2.7f);

        //
        float height = cc.height + cc.radius;
        inWater = Physics.Raycast(transform.position + Vector3.up * height, Vector3.down, height*2, LayerMask.GetMask("Water"));
        Debug.DrawRay(transform.position + Vector3.up * height, Vector3.down* height);
    }
    void Jumping()
    {
        // || !isGround.collider
        if(GravityForce > 2) return;
        float JumpMutify = 2;
        GravityForce = Mathf.Sqrt(JumpHeight * Gravity * JumpMutify);
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));
    }
    void CameraControl()
    {
        Cameraman.transform.position = transform.position;
        CameraY -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * 300;
        CameraY = Mathf.Clamp(CameraY, -45, 45);

        Zoom += Input.mouseScrollDelta.y * Time.fixedDeltaTime * 100;
        Zoom = Mathf.Clamp(Zoom, -12, -4);

        Cameraman.transform.Rotate(0, Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150, 0);
        Cameraman.transform.eulerAngles = new Vector3(CameraY, Cameraman.transform.eulerAngles.y, 0);
        Cameraman.transform.GetChild(0).transform.localPosition = new Vector3(0, 1.15f, Zoom);
    }
    void CreateRipple(int Start, int End, int Delta, float Speed, float Size, float Lifetime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = Start;
        ripple.transform.eulerAngles = forward;

        for (int i = Start; i < End; i+=Delta)
        {
            ripple.Emit(transform.position + ripple.transform.forward * 1.15f, ripple.transform.forward * Speed, Size, Lifetime, Color.white);
            ripple.transform.Rotate(Vector3.up * Delta, Space.World);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        // && !isGround.collider && Mathf.Abs(VelocityY) > 0.1f  && Mathf.Abs(VelocityY) > 0.1f
        if(other.gameObject.layer == 4)
        {
            //CreateRipple(-180, 180, 2, 5, 3f, 3);
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 4 && VelocityXZ > 0.025f && Time.renderedFrameCount % 3 == 0)
        {
            int y = (int)transform.eulerAngles.y;
            CreateRipple(y-100, y+100, 3, 5f, 2.65f, 3f);
        }
    }
    */
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            //CreateRipple(-180, 180, 2, 5, 3f, 3);
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }
}
