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
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                // Create a particle if hit
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.collider.gameObject.tag == "Plane")
                    {
                        _touchCounter.text = "plane hitted";
                        isDragging = false;
                        MousePos = hit.point;
                        Cube.transform.position = MousePos;
                        originalPosition = Cube.transform.position;
                    }
                }
            }

          else  if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
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
                        OnMouseDrag(MousePos);
                        print("Hited with plane");
                    } 
                }
            }
         }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Cube.transform.position = new Vector3(MousePos.x, MousePos.y + 2, MousePos.z);
         }   
 #endif


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
            isDragging = false;
            StartCoroutine(waitAndturnOn());
            Cube.GetComponent<Player>().notPositioned = false;
         } 
    }
    IEnumerator waitAndturnOn()
    {
        yield return new WaitForSeconds(.05f);
        ripple.gameObject.SetActive(false);
        Debug.Log("active");
    }

}
