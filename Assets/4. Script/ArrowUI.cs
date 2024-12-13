using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum Arrow_Type
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
}

public class ArrowUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Arrow_Type type;
    private UIManager umInstance;
    private Coroutine blinkCoroutine; 
    private Color onColor = new Color(1, 1, 1, 1);
    private Color offColor = new Color(1, 1, 1, 0);
    private void OnEnable()
    {
        blinkCoroutine = StartCoroutine(BlinkCo());
    }
    private void Start()
    {
        umInstance = UIManager.Instance;
    }
    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine); // 코루틴 중지
            blinkCoroutine = null;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        umInstance.NeckRotate(type.ToString());
    }
    IEnumerator BlinkCo()
    {
        while(true)
        {
            GetComponent<Image>().color = onColor;
            yield return new WaitForSeconds(0.5f);
            GetComponent<Image>().color = offColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
