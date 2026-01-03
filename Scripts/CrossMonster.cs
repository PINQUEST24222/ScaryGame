using UnityEngine;

public class CrossMonster : MonoBehaviour
{
    public GameObject cross;
    public float rotateSpeed;
    public float sanityDrainSpeed;
    [HideInInspector] public float rotateAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotateAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, rotateAmount);
        if (!cross.GetComponent<MeshRenderer>().isVisible)
        {
            rotateAmount += rotateSpeed * Time.deltaTime;
            rotateAmount = Mathf.Clamp(rotateAmount, 0, 180);
            if (rotateAmount >= 180)
            {
                SanityManager.sanity -= sanityDrainSpeed * Time.deltaTime;
            }
        }
    }
}
