using Microsoft.Kinect.Face;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;
using UnityEngine.UI;



public class AnyName : MonoBehaviour
{
    // Kinect members.

    GameObject tbh;

    private Vector3 pos;

    private bool shbool = true;

    private bool boolfire = true, boolfire2 = true;


    private bool boolkick = true;
    private bool boolkickl = true;

    public float turnvariable = 1.5f;
    public float firenblock = 0.5f;
    public float kickvariable = 0.15f;

    private KinectSensor sensor;
    private BodyFrameReader bodyReader;
    private Body[] bodies;
    private CameraSpacePoint handleft, handright, sholderright, sholderleft, head, footleft, footright;
    private int width;
    private int height;
    private bool ix = false, ix2 = false, ix3 = false, ix4 = false, ix5 = false;
    private FaceFrameSource[] faceFrameSources;
    private FaceFrameReader[] faceFrameReaders;
    private FaceFrameResult results = null;

    private int bodyCount;

    FaceFrameFeatures faceFrameFeatures =
            FaceFrameFeatures.BoundingBoxInColorSpace
                | FaceFrameFeatures.PointsInColorSpace
                | FaceFrameFeatures.BoundingBoxInInfraredSpace
                | FaceFrameFeatures.PointsInInfraredSpace
                | FaceFrameFeatures.RotationOrientation
                | FaceFrameFeatures.FaceEngagement
                | FaceFrameFeatures.Glasses
                | FaceFrameFeatures.Happy
                | FaceFrameFeatures.LeftEyeClosed
                | FaceFrameFeatures.RightEyeClosed
                | FaceFrameFeatures.LookingAway
                | FaceFrameFeatures.MouthMoved
                | FaceFrameFeatures.MouthOpen;

    float moveZ = 0.0f, moveY = 0.0f;
    float inputH = 0.0f;
    float inputV = 0.0f;
    
    Vector3 movement;
    private Animator anim;
    private Animator anim2;

    GameObject enemy;
    private CharacterController cc;
    
    private Vector3 startpos;
    private Quaternion startrot;
    public static int levelN = 0;

    Vector3 v3 = Vector3.zero;

    public float rotationspeed, rotationfriction, rotationsmoothness;
    private float resultingvalue;
    private Quaternion rotatefrom, rotateto;


    
    public GameObject bulletemitter;
    public GameObject bullet;
    public GameObject shieldemitter;
    public GameObject shield;

    //public GameObject textemitter;
    //public GameObject insttext;
    //public GameObject canv;



    GameObject shi;


    public float bff;

    private UnityEngine.AudioSource asour;
    private UnityEngine.AudioSource asour1;
    public AudioClip[] ac;

    public Button b2, b3;
    Button btn2, btn3;
    public Camera cam2;
    
    void TaskOnClick1()
    {
        //Output this to console when the Button is clicked
        turnvariable = 0.10f;

        firenblock = 0.25f;

        kickvariable = 0.12f;

        Debug.Log("Medium Button Clicked");

    }

    void TaskOnClick2()
    {
        //Output this to console when the Button is clicked
        turnvariable = 0.15f;
        firenblock = 0.5f;
        kickvariable = 0.25f;
        Debug.Log("Large Button Clicked");

    }
    


    void Start()
    {
        cam2.enabled = false;
        cam2.enabled = true;
        btn2 = b2.GetComponent<Button>(); 
        btn3 = b3.GetComponent<Button>(); 
        
        asour = GetComponent<UnityEngine.AudioSource>();
        asour1 = GetComponent<UnityEngine.AudioSource>();
        asour.clip = ac[0];
        asour.Play();

        startpos = new Vector3(0.0f, 0.0f, 0.0f);
        startrot = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
        startpos = transform.position;
        startrot = transform.rotation;

        sensor = KinectSensor.GetDefault();
                
        if (sensor != null)
        {
            sensor.Open();
            bodyCount = sensor.BodyFrameSource.BodyCount;
            bodyReader = sensor.BodyFrameSource.OpenReader();
            bodies = new Body[sensor.BodyFrameSource.BodyCount];
            width = sensor.ColorFrameSource.FrameDescription.Width;
            height = sensor.ColorFrameSource.FrameDescription.Height;
            faceFrameSources = new FaceFrameSource[bodyCount];
            faceFrameReaders = new FaceFrameReader[bodyCount];

            for (int i = 0; i < bodyCount; i++)
            {
                faceFrameSources[i] = FaceFrameSource.Create(sensor, 0, faceFrameFeatures);
                faceFrameReaders[i] = faceFrameSources[i].OpenReader();
            }

        }

    }

    void NextLevel()
    {

        levelN++;
        if (levelN > 1)
        {
            levelN = 0;
        }
        
        SceneManager.LoadScene(levelN);
        
    }


    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject == enemy)

        {
            Debug.Log  ("Sphere collided with enemy----------------");
        }
    }



    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "dead")
        {
            cc.transform.position = startpos;
            cc.transform.rotation = startrot;
            anim.Play("LOSE00", -1, 0f);
        }

        if (col.tag == "missile")
        {

          
        }

        else if (col.tag == "inst")
        {

            //GameObject gmo = Instantiate(insttext, textemitter.transform.position, shieldemitter.transform.rotation);
            //gmo.transform.parent = canv.transform;

            //Destroy(col.gameObject);

        }

        else if (col.tag == "inst")
        {

            
            col.gameObject.transform.parent = transform;

            
        }

        else if (col.tag == "goal")
        {

            Destroy(col.gameObject);
            GetComponent<Animator>().Play("WIN00", -1, 0f);
            Invoke("NextLevel", 2f);

        }

        else if (col.tag == "elevator")
        {

            transform.parent = col.transform;
        }

        else if (col.tag == "resiz")
        {

            transform.parent = col.transform;
            //transform.parent = null;
        }



    }
    void OnTriggerExit(Collider col)
    {

        //transform.parent = col.transform;
        transform.parent = null;

    }
        void Update()

    {
        btn2.onClick.AddListener(TaskOnClick1);
        btn3.onClick.AddListener(TaskOnClick2);
        enemy = FindClosestEnemy();
        //anim2 = this.enemy.GetComponent<Animator>();
        if (Vector3.Distance(enemy.transform.position,
         transform.position) > 10)
        {
            asour.Pause();
            asour.clip = ac[0];
            asour.Play();
        }

        if (Vector3.Distance(enemy.transform.position,
         transform.position) < 10)

        {
            asour.Pause();
            asour.clip = ac[1];
            asour.Play();
        }
        if (bodies == null)
        {
            return;
        }
        if (bodyReader != null)
        {
            using (var frame = bodyReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(bodies);
                    var body = bodies.Where(b => b.IsTracked).FirstOrDefault();
                    if (body != null)
                    {

                        handleft = body.Joints[JointType.HandLeft].Position;
                        handright = body.Joints[JointType.HandRight].Position;
                        sholderleft = body.Joints[JointType.ShoulderLeft].Position;
                        sholderright = body.Joints[JointType.ShoulderRight].Position;
                        head = body.Joints[JointType.Head].Position;
                        footleft = body.Joints[JointType.FootLeft].Position;
                        footright = body.Joints[JointType.FootRight].Position;
                        for (int i = 0; i < bodyCount; i++)
                        {
                            if (faceFrameSources[i].IsTrackingIdValid)
                            {
                                using (FaceFrame frame1 = faceFrameReaders[i].AcquireLatestFrame())
                                {
                                    if (frame1 != null)
                                    {
                                        var result = frame1.FaceFrameResult;
                                        if (result != null)
                                        {
                                            if (result.FaceProperties[FaceProperty.RightEyeClosed] == DetectionResult.Yes)
                                            {
                                                ix = true;
                                            }
                                            if (result.FaceProperties[FaceProperty.RightEyeClosed] == DetectionResult.No)
                                            {
                                                ix = false;
                                            }
                                            if (result.FaceProperties[FaceProperty.LeftEyeClosed] == DetectionResult.Yes)
                                            {
                                                ix2 = true;
                                            }
                                            if (result.FaceProperties[FaceProperty.LeftEyeClosed] == DetectionResult.No)
                                            {
                                                ix2 = false;
                                            }
                                            if (ix5 == true)
                                            {
                                                if (result.FaceProperties[FaceProperty.MouthOpen] == DetectionResult.Yes)
                                                {
                                                    ix3 = true;
                                                    ix5 = false;
                                                }
                                            }
                                            if (ix5 == false)
                                            {
                                                if (result.FaceProperties[FaceProperty.MouthOpen] == DetectionResult.No)
                                                {
                                                    ix3 = false;
                                                    ix5 = true;
                                                }
                                            }
                                            if (result.FaceProperties[FaceProperty.Happy] == DetectionResult.Yes)
                                            {
                                                ix4 = true;
                                            }
                                            if (result.FaceProperties[FaceProperty.Happy] == DetectionResult.No)
                                            {
                                                ix4 = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (bodies[i].IsTracked)
                                {
                                    faceFrameSources[i].TrackingId = bodies[i].TrackingId;
                                }
                            }
                        }
                    }
                }
            }
        }


        inputH = 0.0f;
        anim.SetFloat("inputH", inputH);
       // inputV = 0.0f;
       // anim.SetFloat("inputV", inputV);
        anim.SetFloat("atack", 0.0f);
        anim.SetFloat("spell", 0.0f);
        anim.SetBool("block", false);
       // anim.SetFloat("jumper", -1.0f);


        inputH = 0.0f;
        anim.SetFloat("inputH", inputH);


        if ((sholderright.Z - sholderleft.Z) > 0.10f || /*ix ==true  ||*/
            Input.GetAxis("Horizontal") > 0)

        {


            inputH = 1.0f;

            anim.SetFloat("inputH", inputH);
            resultingvalue += (1.0f) * rotationspeed * rotationfriction;
            rotatefrom = transform.rotation;
            rotateto = Quaternion.Euler(0, resultingvalue, 0);
            transform.rotation = Quaternion.Lerp(rotatefrom, rotateto, Time.deltaTime * rotationsmoothness);


        }


        if ((sholderleft.Z - sholderright.Z) > 0.10f || Input.GetAxis("Horizontal") < 0)
        {


            inputH = -1.0f;

            anim.SetFloat("inputH", inputH);


            //anim.Play("WALK00_R");
            //cc.transform.localRotation = Quaternion.Euler(new Vector3(0, rot, 0));
            resultingvalue += (-1.0f) * rotationspeed * rotationfriction;
            rotatefrom = transform.rotation;
            rotateto = Quaternion.Euler(0, resultingvalue, 0);
            transform.rotation = Quaternion.Lerp(rotatefrom, rotateto, Time.deltaTime * rotationsmoothness);


        }

        //////////////////////////////////////////////////


        if ((sholderleft.Z - handleft.Z) > firenblock || Input.GetKeyDown("b"))
        {
            anim.SetBool("block", true);

            if (shbool == true)
            {
                shi = Instantiate(shield, shieldemitter.transform.position, shieldemitter.transform.rotation);
                shbool = false;
                shi.transform.parent = gameObject.transform;
                StartCoroutine(waiter());
            }

            if (shbool == false)
            {
                Destroy(shi, 5);
            }
        }

        if (sholderright.Z - handright.Z > firenblock
            || Input.GetKeyDown("space"))

        {
            anim.SetFloat("spell", 1.0f);
            StartCoroutine(waiter());

        }


        if (sholderleft.Z - handleft.Z > firenblock ||
            Input.GetKeyDown("q") || Input.GetKey("q")
            && Input.GetAxis("Vertical") > 0)

        {


            anim.SetFloat("atack", 1.0f);


        }



        if (sholderleft.Z - handleft.Z > firenblock || Input.GetKeyDown("m")
        || Input.GetKey("m")
        && Input.GetAxis("Vertical") > 0)

        {
            anim.SetFloat("atack", 0.5f);

        }

        if (handleft.Y > handright.Y && handleft.Y > head.Y
                || Input.GetKey("c"))

        {


            anim.Play("jump");
         
        }




        /////////////////////////////////////////
        if (ix3 == true)
        {

            GameObject tbh;
            //GameObject tbh2;

            anim.Play("DAMAGED00");


            tbh = Instantiate(bullet, bulletemitter.transform.position, bulletemitter.transform.rotation);



            tbh.transform.Rotate(Vector3.left * 90);


            Rigidbody trb;
            trb = tbh.GetComponent<Rigidbody>();
            trb.AddForce(transform.forward * bff);
            Destroy(tbh, 2.0f);
            ix3 = false;

        }


        if (cc.isGrounded)

        {

            moveY = -50.0f* Time.deltaTime;




            if (handleft.Y > handright.Y && handleft.Y > head.Y 
                || Input.GetKey("z"))

            {


                inputV = 0;
                anim.SetFloat("inputV", inputV);
                anim.Play("jump");
                moveY = 15.0f;
                moveZ = 05.0f;

            }
            else
            {

                moveY = 0.0f;
                moveZ = 0.0f;

            }


            //if ((handleft.Y - handright.Y)>0.25f && handleft.Y < head.Y)
            if (ix4 == true ||
                (handleft.Y - handright.Y) > 0.25f && handleft.Y < head.Y
                || Input.GetAxis("Vertical") > 0)

            {

                inputV = 1;
                anim.SetFloat("inputV", inputV);
                moveY = 0.0f;
                moveZ = 4.0f;
                //ps.Play();
            }
            else
            {

                //ps.Stop();
                inputV = 0.0f;
                anim.SetFloat("inputV", inputV);
                ix4 = false;
            }


            if ((handright.Y - handleft.Y) > 0.25 || Input.GetAxis("Vertical") < 0)
            {


                inputV = -1;
                anim.SetFloat("inputV", inputV);
                moveZ = -6.0f;
                moveY = 0.0f;

            }


        }

        else
        {
            moveY -= 50.0f * Time.deltaTime;

        }


        Vector3 movement1 = Vector3.zero;

        movement1.x = 0.0f;
        movement1.y = moveY;
        movement1.z = moveZ;


        movement1 = transform.TransformDirection(movement1);

        cc.Move(movement1 * Time.deltaTime);


    }

    IEnumerator waiter() {

        yield return new WaitForSecondsRealtime(1);

        tbh = Instantiate(bullet, bulletemitter.transform.position, bulletemitter.transform.rotation);
        tbh.transform.parent = gameObject.transform;
        tbh.transform.Rotate(Vector3.left * 90);
        Rigidbody trb;
        trb = tbh.GetComponent<Rigidbody>();
        trb.AddForce(transform.forward * bff);
        Destroy(tbh, 2.0f);
        ix3 = false;

        shbool = true;
        
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("missile");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }


    void OnApplicationQuit()
    {
        if (bodyReader != null)
        {
            bodyReader.Dispose();
        }
        
        if (sensor != null && sensor.IsOpen)
        {
            sensor.Close();
        }
    }
    
    private float slowMoTimeScale = 0.1f;
    
    void SlowMo()
    {
        //assign new time scale value
        Time.timeScale = slowMoTimeScale;
        //reduce this to the same proportion as timescale to ensure smooth simulation
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
    }
    
    private IEnumerator coroutine;

    private IEnumerator WaitAndPrint(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndPrint " + Time.time);
        }
     }

}
