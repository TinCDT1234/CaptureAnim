using UnityEngine;

public class DiceCOntroller : MonoBehaviour
{
    public GameObject dice;
    public float duration = 0.5f;
    public float angle = 90f;
    public float delayCallFactor = 4f;
    public float minangle = 120f;
    public float maxangle = 500f;
    private void Awake()
    {
        dice = transform.GetChild(0).gameObject;
    }
    public void Roll()
    {
        LeanTween.rotateAroundLocal(gameObject, Random.onUnitSphere, Random.Range(80f,270f), duration).setOnComplete(() =>
           LeanTween.rotateAroundLocal(gameObject, Random.onUnitSphere, Random.Range(80f, 270f), duration).setOnComplete(() =>
           LeanTween.rotateAroundLocal(gameObject, Random.onUnitSphere, Random.Range(80f, 270f), duration).setOnComplete(() =>
           LeanTween.rotateAroundLocal(gameObject, Random.onUnitSphere, Random.Range(80f, 270f), duration))))
            ;
            
       
    }

    public void RollRecursion(int count)
    {
        if (count <= 0)
        {
            // Stop the recursion when count is less than or equal to 0
            LeanTween.scale(gameObject, Vector3.one * 1, duration);
            LeanTween.rotateLocal(gameObject,Vector3.zero, duration);
            return;
        }
        if(count == 10)
        {
            LeanTween.scale(gameObject, Vector3.one * 1.4f, duration);
        }
        LeanTween.delayedCall(duration/ delayCallFactor, ()=> LeanTween.rotateAroundLocal(gameObject, Random.onUnitSphere, Random.Range(minangle, maxangle), duration).setOnComplete(()=> {
            RollRecursion(count - 1);
            Debug.Log(count);
        }));
    }
}
