using System;
using System.Collections;
using PLAYERTWO.PlatformerProject;
using UnityEngine;

public class Glider : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float scaleDuration = 0.7f;
    [SerializeField] private TrailRenderer[] trails;

    void Start()
    {
        if (player == null)
        {
            player = GetComponentInParent<Player>();
        }

        player.playerEvents.OnGlidingStart.AddListener(ShowGlider);
		player.playerEvents.OnGlidingStop.AddListener(HideGlider);

        transform.localScale = Vector3.zero;
        SetTrailsEmitting(false);
    }

    private void HideGlider()
    {
        StopAllCoroutines();
		StartCoroutine(ScaleGliderRoutine(Vector3.one, Vector3.zero)); // 缩放到0
        SetTrailsEmitting(false);
    }

    private void ShowGlider()
    {
        StopAllCoroutines(); // 停止正在运行的缩放动画，避免冲突
		StartCoroutine(ScaleGliderRoutine(Vector3.zero, Vector3.one)); // 缩放到正常大小
        SetTrailsEmitting(true);
    }

    /// <summary>
    /// 在 scaleDuration 时间内逐渐插值到目标缩放
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private IEnumerator ScaleGliderRoutine(Vector3 from, Vector3 to)
    {
        var time = 0f;
		transform.localScale = from;

		while (time < scaleDuration)
		{
			var scale = Vector3.Lerp(from, to, time / scaleDuration);
			transform.localScale = scale;
			time += Time.deltaTime;
			yield return null;
		}
		// 最后确保完全缩放到目标值
		transform.localScale = to;
    }

    /// <summary>
	/// 设置拖尾特效是否开启
	/// </summary>
	protected virtual void SetTrailsEmitting(bool value)
	{
		if (trails == null) return;
		foreach (var trail in trails)
		{
			trail.emitting = value;
		}
	}
}
