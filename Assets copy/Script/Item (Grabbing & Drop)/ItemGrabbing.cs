using UnityEngine;

public class ItemGrabbing : MonoBehaviour
{
    [Header("Grabbing Settings")]
    [SerializeField] private Transform handPosition; // Titik di mana item dipegang
    [SerializeField] private float grabRange = 2f;
    [SerializeField] private LayerMask itemLayer;

    private GameObject grabbedItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Tekan 'E' untuk mengambil atau membuang item
        {
            if (grabbedItem == null)
            {
                TryGrabItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    private void TryGrabItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange, itemLayer))
        {
            if (hit.collider != null)
            {
                grabbedItem = hit.collider.gameObject;
                grabbedItem.GetComponent<Rigidbody>().isKinematic = true;
                
                // Menjadikan item anak dari tangan karakter (HandPosition)
                grabbedItem.transform.SetParent(handPosition);
                
                // Memindahkan posisi item ke tangan
                grabbedItem.transform.localPosition = Vector3.zero;
                
                // Mengatur rotasi item agar sesuai dengan tangan
                grabbedItem.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void DropItem()
    {
        if (grabbedItem != null)
        {
            grabbedItem.transform.SetParent(null); // Melepaskan dari tangan
            Rigidbody rb = grabbedItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            
            // Tambahkan sedikit dorongan saat drop agar item tidak jatuh diam
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            
            grabbedItem = null;
        }
    }
}
