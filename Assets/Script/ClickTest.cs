using UnityEngine;

public class ClickTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("“–‚½‚Á‚½ : " + hit.collider.name);
            }
            else
            {
                Debug.Log("‰½‚É‚à“–‚½‚ç‚È‚¢");
            }
        }
    }
}
