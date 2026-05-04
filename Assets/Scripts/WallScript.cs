using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class WallScript : MonoBehaviour
{
    public float GridChange;
    private string dir;
    public LayerMask BlockingLayer;
    public SpriteRenderer spriteRenderer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dir = "";
        if (transform.position.y < GridChange)
        {
            transform.localScale = Vector3.one;
            RaycastHit2D hitu = (Physics2D.Raycast(transform.position, Vector3.up, 1, BlockingLayer));
            RaycastHit2D hitl = (Physics2D.Raycast(transform.position, Vector3.left, 1, BlockingLayer));
            RaycastHit2D hitr = (Physics2D.Raycast(transform.position, Vector3.right, 1, BlockingLayer));
            RaycastHit2D hitd = (Physics2D.Raycast(transform.position, Vector3.down, 1, BlockingLayer));
            var overlapul = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y + 1), BlockingLayer);
            var overlapld = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y - 1), BlockingLayer);
            var overlapdr = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y - 1), BlockingLayer);
            var overlapru = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y + 1), BlockingLayer);
            if (hitu && (hitu.collider.CompareTag("Wall")))
            {
                dir += "u";
            }
            if ((hitu && (hitu.collider.CompareTag("Wall"))) && (hitl && (hitl.collider.CompareTag("Wall"))) && (overlapul == null || !overlapul.CompareTag("Wall")))
            {
                dir += "1";
            }
            if (hitl && (hitl.collider.CompareTag("Wall")))
            {
                dir += "l";
            }
            if ((hitl && (hitl.collider.CompareTag("Wall"))) && (hitd && (hitd.collider.CompareTag("Wall"))) && (overlapld == null || !overlapld.CompareTag("Wall")))
            {
                dir += "1";
            }
            if (hitd && (hitd.collider.CompareTag("Wall")))
            {
                dir += "d";
            }
            if ((hitd && (hitd.collider.CompareTag("Wall"))) && (hitr && (hitr.collider.CompareTag("Wall"))) && (overlapdr == null || !overlapdr.CompareTag("Wall")))
            {
                dir += "1";
            }
            if (hitr && (hitr.collider.CompareTag("Wall")))
            {
                dir += "r";
            }
            if ((hitr && (hitr.collider.CompareTag("Wall"))) && (hitu && (hitu.collider.CompareTag("Wall"))) && (overlapru == null || !overlapru.CompareTag("Wall")))
            {
                dir += "1";
            }
            spriteRenderer.sprite = Resources.Load<Sprite>("Wall(square " + dir + ")");
        }
        else
        {
            
            if ((Mathf.Abs(transform.position.x) % 1) == ((transform.position.y - GridChange) % 2) / 2)
            {
                transform.localScale = Vector3.one;
                var overlapll = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y), BlockingLayer);
                var overlaplld = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y - 1), BlockingLayer);
                var overlapld = Physics2D.OverlapPoint(new Vector3(transform.position.x - 0.5f, transform.position.y - 1), BlockingLayer);
                var overlaprd = Physics2D.OverlapPoint(new Vector3(transform.position.x + 0.5f, transform.position.y - 1), BlockingLayer);
                var overlaprrd = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y - 1), BlockingLayer);
                var overlaprr = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y), BlockingLayer);
                var overlapur = Physics2D.OverlapPoint(new Vector3(transform.position.x + 0.5f, transform.position.y + 1), BlockingLayer);
                var overlapu = Physics2D.OverlapPoint(new Vector3(transform.position.x, transform.position.y + 1), BlockingLayer);
                var overlapul = Physics2D.OverlapPoint(new Vector3(transform.position.x - 0.5f, transform.position.y + 1), BlockingLayer);
                RaycastHit2D hitl = (Physics2D.Raycast(transform.position, Vector3.left, 0.5f, BlockingLayer));
                RaycastHit2D hitr = (Physics2D.Raycast(transform.position, Vector3.right, 0.5f, BlockingLayer));
                RaycastHit2D hitd = (Physics2D.Raycast(transform.position, Vector3.down, 1, BlockingLayer));

                if (hitl && (hitl.collider.CompareTag("Wall")))
                {
                    dir += "l";
                }
                if ((hitl && (hitl.collider.CompareTag("Wall"))) && (hitd && (hitd.collider.CompareTag("Wall"))) && ((overlapll == null || !overlapll.CompareTag("Wall")) || (overlaplld == null || !overlaplld.CompareTag("Wall")) || (overlapld == null || !overlapld.CompareTag("Wall"))))
                {
                    dir += "1";
                }
                if (hitd && (hitd.collider.CompareTag("Wall")))
                {
                    dir += "d";
                }
                if ((hitd && (hitd.collider.CompareTag("Wall"))) && (hitr && (hitr.collider.CompareTag("Wall"))) && ((overlaprd == null || !overlaprd.CompareTag("Wall")) || (overlaprrd == null || !overlaprrd.CompareTag("Wall")) || (overlaprr == null || !overlaprr.CompareTag("Wall"))))
                {
                    dir += "1";
                }
                if (hitr && (hitr.collider.CompareTag("Wall")))
                {
                    dir += "r";
                }
                if ((hitr && (hitr.collider.CompareTag("Wall"))) && (hitl && (hitl.collider.CompareTag("Wall"))) && ((overlapur == null || !overlapur.CompareTag("Wall")) || (overlapu == null || !overlapu.CompareTag("Wall")) || (overlapul == null || !overlapul.CompareTag("Wall"))))
                {
                    dir += "1";
                }
            }
            else
            {

                transform.localScale = new Vector3(1, -1, 1);
                var overlaplu = Physics2D.OverlapPoint(new Vector3(transform.position.x - 0.5f, transform.position.y + 1), BlockingLayer);
                var overlapllu = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y + 1), BlockingLayer);
                var overlapll = Physics2D.OverlapPoint(new Vector3(transform.position.x - 1, transform.position.y), BlockingLayer);
                var overlapld = Physics2D.OverlapPoint(new Vector3(transform.position.x - 0.5f, transform.position.y - 1), BlockingLayer);
                var overlapd = Physics2D.OverlapPoint(new Vector3(transform.position.x, transform.position.y - 1), BlockingLayer);
                var overlaprd = Physics2D.OverlapPoint(new Vector3(transform.position.x + 0.5f, transform.position.y - 1), BlockingLayer);
                var overlaprr = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y), BlockingLayer);
                var overlaprru = Physics2D.OverlapPoint(new Vector3(transform.position.x + 1, transform.position.y + 1), BlockingLayer);
                var overlapru = Physics2D.OverlapPoint(new Vector3(transform.position.x + 0.5f, transform.position.y + 1), BlockingLayer);
                RaycastHit2D hitu = (Physics2D.Raycast(transform.position, Vector3.up, 1, BlockingLayer));
                RaycastHit2D hitl = (Physics2D.Raycast(transform.position, Vector3.left, 0.5f, BlockingLayer));
                RaycastHit2D hitr = (Physics2D.Raycast(transform.position, Vector3.right, 0.5f, BlockingLayer));

                if (hitl && (hitl.collider.CompareTag("Wall")))
                {
                    dir += "l";
                }
                if ((hitl && (hitl.collider.CompareTag("Wall"))) && (hitu && (hitu.collider.CompareTag("Wall"))) && ((overlaplu == null || !overlaplu.CompareTag("Wall")) || (overlapllu == null || !overlapllu.CompareTag("Wall")) || (overlapll == null || !overlapll.CompareTag("Wall"))))
                {
                    dir += "1";
                }
                if (hitu && (hitu.collider.CompareTag("Wall")))
                {
                    dir += "d";
                }
                if ((hitu && (hitu.collider.CompareTag("Wall"))) && (hitr && (hitr.collider.CompareTag("Wall"))) && ((overlaprr == null || !overlaprr.CompareTag("Wall")) || (overlaprru == null || !overlaprru.CompareTag("Wall")) || (overlapru == null || !overlapru.CompareTag("Wall"))))
                {
                    dir += "1";
                }
                if (hitr && (hitr.collider.CompareTag("Wall")))
                {
                    dir += "r";
                }
                if ((hitr && (hitr.collider.CompareTag("Wall"))) && (hitl && (hitl.collider.CompareTag("Wall"))) && ((overlapld == null || !overlapld.CompareTag("Wall")) || (overlapd == null || !overlapd.CompareTag("Wall")) || (overlaprd == null || !overlaprd.CompareTag("Wall"))))
                {
                    dir += "1";
                }
            }
            spriteRenderer.sprite =  Resources.Load<Sprite>("Wall(triangle " + dir + ")");
        }
    }
}
