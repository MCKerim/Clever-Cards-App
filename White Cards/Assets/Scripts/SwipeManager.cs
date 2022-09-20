using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwipeManager : MonoBehaviour
{
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    [SerializeField] private float swipeTreshold = 0.8f;
    [SerializeField] private float maxSwipeTime = 1f;
    private float touchStartTime;
    private float touchEndTime;

    [SerializeField] private float minSwipeDistance = 2f;
    private CardManager cardManager;

    private Vector2 bottomLeftCorner;
    private Vector2 topRightCorner;

    [SerializeField] private RectTransform cardRectTransform;
    [SerializeField] private RectTransform settingsRectTransform;

    [SerializeField] private GameObject trail;

    // Start is called before the first frame update
    void Start()
    {
        cardManager = GameObject.FindObjectOfType<CardManager>();

        UpdateTouchArea();
    }

    // Update is called once per frame
    void Update()
    {
        Touch[] touches = Input.touches;
        if(touches.Length > 0)
        {
            Touch t = touches[0];
            if(t.phase == TouchPhase.Began)
            {
                touchStartPos = t.position;
                touchStartTime = Time.realtimeSinceStartup;

                Vector3 pos = Camera.main.ScreenToWorldPoint(t.position);
                pos.z = 20;
                trail.transform.position = pos;    
                StartCoroutine(UpdateTrail());

            }
            else if(t.phase == TouchPhase.Ended)
            {
                touchEndPos = t.position;
                touchEndTime = Time.realtimeSinceStartup;

                if(CheckIfPointIsOnCard(Camera.main.ScreenToWorldPoint(touchStartPos))){

                    float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(touchStartPos), Camera.main.ScreenToWorldPoint(touchEndPos));
                    if(touchEndTime - touchStartTime <= maxSwipeTime && distance >= minSwipeDistance){
                        CalculateSwipeDirection(touchStartPos, touchEndPos);
                    }
                    else
                    {
                        StopCoroutine(UpdateTrail());
                        trail.SetActive(false);
                        
                        GameObject.FindObjectOfType<CardUIManager>().Turn();
                    }
                }
            }
        }
    }

    IEnumerator UpdateTrail(){
        Touch[] touches;
        Vector3 pos;

        trail.SetActive(true);
        float time = 0f;

        while(time <= maxSwipeTime){
            touches = Input.touches;
            if(touches.Length > 0){
                pos = Camera.main.ScreenToWorldPoint(touches[0].position);
                pos.z = 20;
                trail.transform.position = pos;
            }
            time += Time.deltaTime;
            yield return null;
        }
        
        trail.SetActive(false);
    }

    private void CalculateSwipeDirection(Vector2 start, Vector2 end)
    {
        Vector2 dir = (end - start).normalized;
        Debug.DrawLine(start, end);

        //Hard
        if(Vector2.Dot(Vector2.right, dir) > swipeTreshold)
        {
            cardManager.RateCardHard();
        }
        //Easy
        else if(Vector2.Dot(Vector2.left, dir) > swipeTreshold)
        {
            cardManager.RateCardEasy();
        }
        //Medium
        else if(Vector2.Dot(Vector2.down, dir) > swipeTreshold)
        {
            cardManager.RateCardMedium();
        }
    }

    private void UpdateTouchArea()
    {
        Vector3[] rectCorners = new Vector3[4];
        cardRectTransform.GetWorldCorners(rectCorners);

        bottomLeftCorner = rectCorners[0];
        topRightCorner = rectCorners[2];

        Vector3[] settingsRectCorners = new Vector3[4];
        settingsRectTransform.GetWorldCorners(settingsRectCorners);
        float settingsHeight = settingsRectCorners[1].y - settingsRectCorners[0].y;

        bottomLeftCorner.y += settingsHeight * 1.5f;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UpdateTouchArea();
        Gizmos.color = Color.red;
        Vector2 topLeft = new Vector3(bottomLeftCorner.x, topRightCorner.y);
        Vector2 topRight = new Vector3(topRightCorner.x, topRightCorner.y);
        Vector2 bottomLeft = new Vector3(bottomLeftCorner.x, bottomLeftCorner.y);
        Vector2 bottomRight = new Vector3(topRightCorner.x, bottomLeft.y);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
    }
#endif

    public bool CheckIfPointIsOnCard(Vector2 point)
    {
        return point.x >= bottomLeftCorner.x && point.x <= topRightCorner.x && point.y >= bottomLeftCorner.y && point.y <= topRightCorner.y;
    }
}
