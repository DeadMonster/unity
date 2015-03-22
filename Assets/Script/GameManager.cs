using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;
    //游戏得分
    int m_score = 0;

    //游戏最高得分
    static int m_hiscore = 0;

    //弹药数量
    int m_ammo = 100;

    //游戏主角
    Player m_player;

    //UI文字
    GUIText txt_ammo;
    GUIText txt_hiscore;
    GUIText txt_life;
    GUIText txt_score;
	// Use this for initialization
	void Start () {
        Instance = this;

        //获得主角
        m_player=GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //获得设置的UI文字
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("txt_ammo") == 0)
            {
                txt_ammo = t.GetComponent<GUIText>();
            }
            else if (t.name.CompareTo("txt_hiscore") == 0)
            {
                txt_hiscore= t.GetComponent<GUIText>();
            }
            else if (t.name.CompareTo("txt_life") == 0)
            {
                txt_life = t.GetComponent<GUIText>();
            }
            else if (t.name.CompareTo("txt_score") == 0)
            {
                txt_score = t.GetComponent<GUIText>();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetScore(int score)
    {
        m_score += score;
        if (m_score > m_hiscore)
        {
            m_hiscore = m_score;
        }
        txt_score.text = "Score<color=yellow>" + m_score + "</color>";
        txt_hiscore.text = "Hiscore" + m_hiscore;
    }

    //更新弹药
    public void SetAmmo(int ammo)
    {
        m_ammo -= ammo;
        //如果弹药为负数，重新装弹
        if (m_ammo < 0)
            m_ammo = 100;
        txt_ammo.text = m_ammo.ToString() + "/100";
    }

    //更新生命
    public void SetLife(int life)
    {
        txt_life.text = life.ToString();
    }

    void OnGUI()
    {
        if(m_player.m_life<=0)
        {
            //居中显示文字
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            //改变文字大小
            GUI.skin.label.fontSize = 40;

            //显示Game Over文字
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Game Over");

            //设置按钮文字大小
            GUI.skin.label.fontSize = 30;
            //显示重新游戏按钮
            if(GUI.Button(new Rect(Screen.width*0.5f-150,Screen.height*0.75f,300,40),"Try again"))
            {
                //重新读入当前关卡
                Application.LoadLevel(Application.loadedLevelName);
            }
        }
    }
}
