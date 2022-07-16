using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupFinder : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        GameManager.Instance.RegisterTargetGroupFinder(this);
    }

    public void SetGroup(List<Transform> dice)
    {
        CinemachineTargetGroup.Target[] targets = new CinemachineTargetGroup.Target[dice.Count];
        for (int i = 0; i < dice.Count; ++i)
        {
            targets[i].target = dice[i];
            targets[i].radius = 1.0f;
            targets[i].weight = 1.0f;
        }
        targetGroup.m_Targets = targets;
    }
}
