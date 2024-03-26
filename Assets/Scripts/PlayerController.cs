using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
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
    public Text _touchCounter;


    private Vector3 offset;
     private Vector3 originalPosition;
    public bool isDragging;
    public float dragThreshold = 0.1f; // Adjust as needed


     // Start is called before the first frame update yup




    private void Awake()
    {
        Instance = this;
        isDragging = false;
    }
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
       
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.tag == "Plane")
                {
                    isDragging = false; 
                    MousePos = hit.point;
                    Cube.transform.position = MousePos;
                    originalPosition = Cube.transform.position;
                     print("Hited with plane");
                }
            }
         }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnMouseUp();
        }
 
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
                       OnMouseDrag(MousePos);
                       print("Hited with plane");
                 }
            } 
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
             Cube.transform.position = new Vector3(MousePos.x, MousePos.y +2, MousePos.z);
        }
#endif


        /*
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

         }
        else
        {
            Cube.transform.position = new Vector3(MousePos.x, MousePos.y + 2, MousePos.z);
        }
 #endif
          */
          
     }
 
    void OnMouseDrag(Vector3 newPosition)
    {
       // Vector3 NewPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) + offset;
       //  transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        if (!isDragging && Vector3.Distance(originalPosition,Cube.transform.position) > dragThreshold)
        {
            isDragging = true;
            Debug.Log("Started Dragging");
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
              Debug.Log("Stopped Dragging");
        }
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
