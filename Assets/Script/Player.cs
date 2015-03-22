using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public Transform m_transform;

    // 角色控制器组件
    CharacterController m_ch;

    // 角色移动速度
    float m_movSpeed = 3.0f;

    // 重力
    float m_gravity = 2.0f;

    //枪口
    Transform m_muzzlepoint;

    //射击时，射线能射到的碰撞层；
    public LayerMask m_layer;

    //射中目标后的粒子效果
    public Transform m_fx;

    //射击音效
    public AudioClip m_audio;

    //射击间隔时间计时器
    float m_shootTimer = 0;
    // 摄像机
    Transform m_camTransform;

    // 摄像机旋转角度
    Vector3 m_camRot;

    // 摄像机高度
    float m_camHeight = 1.4f;

    // 生命值
    public int m_life = 5;

	// Use this for initialization
	void Start () {
        m_transform = this.transform;
        // 获取角色控制器组件
        m_ch = this.GetComponent<CharacterController>();

        // 获取摄像机
        m_camTransform = Camera.main.transform;
        
        // 设置摄像机初始位置
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;

        // 设置摄像机的旋转方向与主角一致
        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;

        m_muzzlepoint = m_camTransform.FindChild("M16/weapon/muzzlepoint").transform;
        Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
	//如果生命值为0，什么也不做
        if (m_life <= 0)
            return;
        Control();

	}
    void Control()
    {


        //获取鼠标移动距离
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");

        // 旋转摄像机
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camTransform.eulerAngles = m_camRot;

        // 使主角的面向方向与摄像机一致
        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0; camrot.z = 0;
        m_transform.eulerAngles = camrot;

        // 定义3个值控制移动
        float xm = 0, ym = 0, zm = 0;

        // 重力运动
        ym -= m_gravity * Time.deltaTime;

        // 上下左右运动
        if (Input.GetKey(KeyCode.W))
        {
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            zm -= m_movSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            xm += m_movSpeed * Time.deltaTime;
        }
        // 使用角色控制器提供的Move函数进行移动 它会自动检测碰撞
        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));

        // 使摄像机的位置与主角一致
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;

        //更新射击时间
        m_shootTimer -= Time.deltaTime;

        //鼠标左键射击
        if(Input.GetMouseButton(0)&&m_shootTimer<=0)
        {
            m_shootTimer = 0.1f;
            //射击音效
            this.audio.PlayOneShot(m_audio);
            //减少弹药 更新弹药UI 
            GameManager.Instance.SetAmmo(1);
            //RaycastHit用来保存射线的探测结果
            RaycastHit info;

            //从muzzleponit的位置，向摄像机面向的正方向射出一条射线
            //射线只能与m_layer所指定的层碰撞
            bool hit = Physics.Raycast(m_muzzlepoint.position, m_camTransform.TransformDirection(Vector3.forward), out info, 100, m_layer);
            if (hit)
            {
                //如果射中了Tag为enemy的游戏体 
                if(info.transform.tag.CompareTo("enemy")==0)
                {
                    Enemy enemy = info.transform.GetComponent<Enemy>();
                    //减少敌人生命
                    enemy.OnDamage(1);

                }
            }
            //在射中的地方添加例子效果
            Instantiate(m_fx,info.point,info.transform.rotation);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position,"Spawn.tif");
    }

    public void OnDamage(int damage)
    {
        m_life -= damage;

        //更新UI
        GameManager.Instance.SetLife(m_life);

        //如果生命值为0，取消鼠标锁定
        if(m_life<=0)
        {
            Screen.lockCursor = false;
        }
    }

}
