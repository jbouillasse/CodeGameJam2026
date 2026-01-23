using UnityEngine;

public class CarabineController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddShot();

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point = new Vector2(mousePos.x, mousePos.y);

            Collider2D hit = Physics2D.OverlapPoint(point);

            if (hit != null)
            {
                BallonScript b = hit.GetComponent<BallonScript>();
                if (b != null)
                {
                    b.Pop();
                }
            }
        }
    }
}
