using UnityEngine;

public class Test : MonoBehaviour
{
    Transform mTran;
    Rigidbody2D Rigid;

    int Speed = 100;
    bool Fly;

    void Start()
    {
        mTran = transform;
        Rigid = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Fly)
        {
            mTran.Translate(Vector2.up * Time.deltaTime * Speed);
            if (mTran.position.y > 40) mTran.position = new Vector3(0, -30, 0);
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Fo", GUILayout.Width(120), GUILayout.Height(80)))
        {
            Fly = false;
            mTran.position = new Vector3(0, -30, 0);
            Rigid.velocity = Vector2.zero;
            Rigid.AddForce(new Vector2(Random.Range(-Speed / 2, Speed / 2), Speed), ForceMode2D.Impulse);
        }
        if (GUILayout.Button("Fly", GUILayout.Width(120), GUILayout.Height(80)))
        {
            Fly = true;
            mTran.position = new Vector3(0, -30, 0);
            Rigid.velocity = Vector2.zero;
        }
        Speed = (int)GUILayout.HorizontalSlider(Speed, 0, 200, GUILayout.Width(300), GUILayout.Height(60));
        GUILayout.Label("速度：" + Speed);
        if (GUILayout.Button("Game", GUILayout.Width(120), GUILayout.Height(80))) Application.LoadLevel(0);
    }
}