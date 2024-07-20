using TMPro;
using UnityEngine;

public class CombatPointText : GameUnit
{
    [SerializeField] TextMeshProUGUI point;
    [SerializeField] float speed;

    private float count = 0f;
    private const float DELAY_TIME = 3f;

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;

        if(count >= DELAY_TIME)
        {
            OnDespawn();
        }

        TF.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void SetPoint(int gainPoint, float size)
    {
        point.fontSize = size;
        point.text = "+ " + gainPoint.ToString();
    }

    private void OnDespawn()
    {
        //Destroy(TF.gameObject);
        count = 0f;
        SimplePool.Despawn(this);
    }
}
