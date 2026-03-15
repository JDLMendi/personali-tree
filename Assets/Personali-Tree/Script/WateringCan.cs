using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;

public class WateringCan : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TreeManager treeManager;

    [Header("Animation Settings")]
    public float hoverScale = 1.3f;
    public float animationDuration = 0.2f;
    public Vector3 clickPunchAmount = new Vector3(0.1f, 0.1f, 0.1f);

    [Header("Idle Settings")]
    public float floatStrength = 10f;
    public float floatDuration = 2f;
    public Vector3 idleRotation = new Vector3(0, 0, 5f);

    private Vector3 _initialScale;
    private Tween _idleTween;

    public void Start()
    {
        _initialScale = transform.localScale;
        if (treeManager == null)
            treeManager = FindFirstObjectByType<TreeManager>();

        StartIdleAnimation();
    }

    private void StartIdleAnimation()
    {
        // Create a sequence so we can move and rotate at the same time
        Sequence idleSeq = DOTween.Sequence();

        // Gentle floating up and down
        idleSeq.Append(transform.DOLocalMoveY(transform.localPosition.y + floatStrength, floatDuration).SetEase(Ease.InOutSine));
        idleSeq.Append(transform.DOLocalMoveY(transform.localPosition.y, floatDuration).SetEase(Ease.InOutSine));

        // Gentle tilt back and forth
        transform.DOLocalRotate(idleRotation, floatDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        idleSeq.SetLoops(-1); // Loop infinitely
        _idleTween = idleSeq;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // We use Complete() so the punch doesn't stack weirdly
        transform.DOPunchScale(clickPunchAmount, animationDuration, 10, 1);
        
        // Add a "pour" tilt on click
        transform.DOPunchRotation(new Vector3(0, 0, -20f), 0.3f);

        treeManager.WaterPlant();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Pause idle so it doesn't fight the hover scale
        _idleTween.Pause();
        
        transform.DOScale(_initialScale * hoverScale, animationDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_initialScale, animationDuration).SetEase(Ease.OutQuad).OnComplete(() => 
        {
            // Resume idle once it's back to normal size
            _idleTween.Play();
        });
    }
}