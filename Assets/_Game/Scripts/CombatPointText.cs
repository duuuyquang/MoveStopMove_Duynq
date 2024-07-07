using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CombatPointText : MonoBehaviour
{
    [SerializeField] Transform TF;
    [SerializeField] TextMeshProUGUI point;

    [SerializeField] float speed;

    private float count = 0f;

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;

        if(count >= 3f)
        {
            Destroy(TF.gameObject);
        }

        TF.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void SetPoint(int gainPoint, float size)
    {
        point.fontSize = size;
        point.text = "+ " + gainPoint.ToString();
    }
}
