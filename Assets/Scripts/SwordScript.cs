using UnityEngine;


public class SwordScript : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private GameObject Player;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position;
        float arctg = Mathf.Atan2(diference.y, diference.x);
        float gradient = diference.y / diference.x;
        float arcctg = 1 / arctg;
        float x = arctg * 180 / Mathf.PI;
        transform.rotation = Quaternion.Euler(0,0,x - 90);
        transform.position = Player.transform.position + new Vector3(Mathf.Cos(arctg), Mathf.Sin(arctg));
    }
}
