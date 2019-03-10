using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Vuforiaをつかう
using Vuforia;

/**
 * Vuforiaの検出器をハンドリングする
 */
public class PlaneTracker : MonoBehaviour
{
    // ARで表示したいオブジェクトをインスペクタで指定する
    public GameObject contentPrefab;
    // 生成したインスタンスを一時保存する
    GameObject prevInstance;
    // Vuforiaの検出器
    PositionalDeviceTracker tracker;

    // このオブジェクトがアクティブになったときにUnity側から呼ばれる
    public void Awake()
    {
        // Vuforia が有効化されたときに呼ばれる処理を登録する
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
    }

    // このオブジェクトが破棄された（Unity終了時含む）ときに呼ばれる
    public void OnDestroy()
    {
        // Awakeで設定した処理を破棄させる
        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
    }

    // Vuforiaが有効化されたときに呼ばれる
    private void OnVuforiaStarted()
    {
        // Vuforiaの検出器を取得する
        tracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
    }

    /**
     * OnInteractiveHitTest のコールバックに設定する
     * HitTestResult この引数はタップした位置情報
     */
    public void SpawnContent(HitTestResult result)
    {
        // 手動で追加していた GroundPlaneStage と同等のAR上の床面「アンカー」を生成する
        GameObject anchor = tracker.CreatePlaneAnchor(Guid.NewGuid().ToString(), result);

        // 床面情報が取得できなかったら関数を終了させる
        if (result == null || anchor == null)
        {
            return;
        }

        // このオブジェクトのインスペクタにアタッチしたプレファブのインスタンスを生成する
        GameObject content = Instantiate(contentPrefab);
        // インスタンスの親をアンカーにする
        content.transform.parent = anchor.transform;
        // インスタンスの座標と回転をリセットする
        content.transform.localPosition = Vector3.zero;
        content.transform.localRotation = Quaternion.identity;
        // インスタンスをアクティブにする
        content.SetActive(true);

        // すでにインスタンスがあったら削除する
        if (prevInstance != null)
        {
            Destroy(prevInstance);
        }

        // インスタンスを格納する
        prevInstance = content;
    }
}
