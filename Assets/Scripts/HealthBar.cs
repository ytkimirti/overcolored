using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    public float currHealth = 1f;
    public float scaleY = 0.5f;
    public float scaleX = 1;
    public float height = 0.5f;

    [Space]
    public float lerpSpeed;

    [Header("References, ignore .d")]
    public Transform barTransform;
    public SpriteFlasher spriteFlasher;
    public SpriteFlasher spriteFlasherBG;
    public SpriteRenderer sprite;
    public SpriteRenderer bgSprite;

    Transform cameraTrans;

    void Start()
    {
        cameraTrans = CameraController.main.cam.transform;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;

        barTransform.LookAt(cameraTrans, Vector3.up);

        float newScaleX = Mathf.Lerp(barTransform.localScale.x, scaleX * currHealth, lerpSpeed * Time.deltaTime);

        barTransform.localScale = new Vector2(newScaleX, scaleY);

        bgSprite.transform.localScale = new Vector2(scaleX / newScaleX, 1);

        sprite.enabled = barTransform.localPosition.y > height / 2;
        bgSprite.enabled = sprite.enabled;

    }

    public void OnDamage()
    {
        ShowBar();

        spriteFlasher.Flash();
        //spriteFlasherBG.Flash();
    }

    public void ShowBar()
    {
        barTransform.DOLocalMoveY(height, 0.2f).OnComplete(HideBar);
    }

    public void HideBar()
    {
        barTransform.DOLocalMoveY(0, 0.2f).SetDelay(0.8f);
    }
}
