using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private const float FormationDistance = 3;
    private const int BinarySearchIterations = 30;
    private const float DistanceUpperBound = 1e9f;

    private List<GameObject> _currentSelection;
    public List<GameObject> CurrentSelection
    {
        get { return _currentSelection; }
    }

    void Start()
    {
        _currentSelection = new List<GameObject>();
    }

    void Awake()
    {
        Messenger<int>.AddListener("FormationChanged", ChangeFormation);
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener("FormationChanged", ChangeFormation);
    }

    private void ChangeFormation(int formationId)
    {
        float minMaxDist = float.PositiveInfinity;
        for (var leaderIndex = 0; leaderIndex < CurrentSelection.Count; leaderIndex++)
        {
            List<float> targetForwardCoeffs = new List<float>();
            List<float> targetRightCoeffs = new List<float>();
            for (var i = 1; i < _currentSelection.Count; i++)
            {
                float forwardCoeff = 0, rightCoeff = 0;
                switch (formationId)
                {
                    case 1:
                        forwardCoeff = -FormationDistance * i;
                        break;

                    case 2:
                        rightCoeff = FormationDistance * (i + 1) / 2;
                        if (i % 2 == 0)
                        {
                            rightCoeff = -rightCoeff;
                        }
                        break;

                    case 3:
                        rightCoeff = FormationDistance * (i + 1) / 2;
                        if (i % 2 == 0)
                        {
                            rightCoeff = -rightCoeff;
                        }
                        forwardCoeff = -FormationDistance * (i + 1) / 2;
                        break;
                }
                targetForwardCoeffs.Add(forwardCoeff);
                targetRightCoeffs.Add(rightCoeff);
            }
            FindMinMaxMatching(leaderIndex, targetForwardCoeffs, targetRightCoeffs, ref minMaxDist);

            // Alternative approach: too slow for large number of units (due to exponential complexity) 
            // but fine for just five of them

            // TryAllPermutations(leaderIndex, targetForwardCoeffs, targetRightCoeffs, ref minMaxDist);

        }
    }

    private void FindMinMaxMatching(int leaderIndex, List<float> targetForwardCoeffs, List<float> targetRightCoeffs, ref float minMaxDist)
    {
        List<GameObject> units = new List<GameObject>();
        for (var unitIndex = 0; unitIndex < CurrentSelection.Count; unitIndex++)
        {
            if (unitIndex != leaderIndex)
            {
                units.Add(CurrentSelection[unitIndex]);
            }
        }

        GameObject leader = CurrentSelection[leaderIndex];
        Vector3 mainDirection = leader.transform.forward;
        Vector3 orthogonalDirection = Vector3.Cross(mainDirection, Vector3.up);
        List<Vector3> targetPositions = new List<Vector3>();
        for (var unitIndex = 0; unitIndex < units.Count; unitIndex++)
        {
            targetPositions.Add(leader.transform.position + mainDirection * targetForwardCoeffs[unitIndex] +
                                orthogonalDirection * targetRightCoeffs[unitIndex]);
        }

        float l = 0, r = DistanceUpperBound;
        for (int i = 0; i < BinarySearchIterations; i++)
        {
            float mid = (l + r) / 2;
            List<int> leftPair = KuhnsAlgorithm(units, targetPositions, mid);
            if (leftPair.TrueForAll(a => a != -1))
            {
                r = mid;
            }
            else
            {
                l = mid;
            }
        }
        float threshold = r;

        if (threshold < minMaxDist)
        {
            minMaxDist = threshold;
            List<int> unitForTarget = KuhnsAlgorithm(units, targetPositions, threshold);
            for (int i = 0; i < unitForTarget.Count; i++)
            {
                GoblinMovement unit = units[unitForTarget[i]].GetComponent<GoblinMovement>();
                unit.Leader = leader;
                unit.ForwardCoeff = targetForwardCoeffs[i];
                unit.RightCoeff = targetRightCoeffs[i];
            }
            CurrentSelection[leaderIndex].GetComponent<GoblinMovement>().Leader = null;
        }
    }

    private List<int> KuhnsAlgorithm(List<GameObject> units, List<Vector3> targetPositions, float mid)
    {
        List<int> leftPair = Enumerable.Repeat(-1, units.Count).ToList();
        for (int j = 0; j < units.Count; j++)
        {
            List<bool> used = new List<bool>(new bool[units.Count]);
            if (!dfs(j, leftPair, used, units, targetPositions, Utils.Sqr(mid)))
            {
                break;
            }
        }
        return leftPair;
    }

    private bool dfs(int node, List<int> leftPair, List<bool> used, List<GameObject> units, List<Vector3> targetPositions, float sqrAllowedDistance)
    {
        if (used[node])
        {
            return false;
        }
        used[node] = true;
        Vector3 unitPosition = units[node].transform.position;
        for (int to = 0; to < targetPositions.Count; to++)
        {
            if ((unitPosition - targetPositions[to]).sqrMagnitude < sqrAllowedDistance)
            {
                if (leftPair[to] == -1 || dfs(leftPair[to], leftPair, used, units, targetPositions, sqrAllowedDistance))
                {
                    leftPair[to] = node;
                    return true;
                }
            }
        }
        return false;
    }

    private void TryAllPermutations(int leaderIndex, List<float> targetForwardCoeffs, List<float> targetRightCoeffs, ref float minMaxDist)
    {
        List<GameObject> units = new List<GameObject>();
        List<int> indices = new List<int>();
        GameObject leader = CurrentSelection[leaderIndex];
        Vector3 mainDirection = leader.transform.forward;
        Vector3 orthogonalDirection = Vector3.Cross(mainDirection, Vector3.up);
        for (var unitIndex = 0; unitIndex < CurrentSelection.Count; unitIndex++)
        {
            if (unitIndex != leaderIndex)
            {
                units.Add(CurrentSelection[unitIndex]);
                indices.Add(indices.Count);
            }
        }

        bool exists;
        do
        {
            float maxDist = 0;
            for (var unitIndex = 0; unitIndex < units.Count; unitIndex++)
            {
                Vector3 targetPosition = leader.transform.position + mainDirection * targetForwardCoeffs[unitIndex] +
                                         orthogonalDirection * targetRightCoeffs[unitIndex];
                Vector3 distance = targetPosition - units[indices[unitIndex]].transform.position;
                maxDist = Math.Max(maxDist, distance.magnitude);
            }
            if (maxDist < minMaxDist)
            {
                minMaxDist = maxDist;
                for (var unitIndex = 0; unitIndex < units.Count; unitIndex++)
                {
                    int pIndex = indices[unitIndex];
                    units[pIndex].GetComponent<GoblinMovement>().Leader = leader;
                    units[pIndex].GetComponent<GoblinMovement>().ForwardCoeff = targetForwardCoeffs[unitIndex];
                    units[pIndex].GetComponent<GoblinMovement>().RightCoeff = targetRightCoeffs[unitIndex];
                }
                CurrentSelection[leaderIndex].GetComponent<GoblinMovement>().Leader = null;
            }

            exists = false;
            for (var i = indices.Count - 2; i >= 0; i--)
            {
                if (indices[i] < indices[i + 1])
                {
                    exists = true;
                    int indexToSwap = i + 1;
                    for (var j = i + 2; j < units.Count; j++)
                    {
                        if (indices[j] > indices[i] && indices[j] < indices[indexToSwap])
                        {
                            indexToSwap = j;
                        }
                    }
                    int tmp = indices[i];
                    indices[i] = indices[indexToSwap];
                    indices[indexToSwap] = tmp;
                    indices.Reverse(i + 1, indices.Count - i - 1);
                }
            }
        } while (exists);
    }

    public void Clear()
    {
        foreach (GameObject gameObj in _currentSelection)
        {
            SelectableUnit target = gameObj.GetComponent<SelectableUnit>();
            if (target)
            {
                target.RemoveSelection();
            }
        }
        _currentSelection.Clear();
        Messenger<int>.Broadcast("CntChanged", _currentSelection.Count);
    }

    public void Add(GameObject gameObj)
    {
        SelectableUnit target = gameObj.GetComponent<SelectableUnit>();
        target.Select();
        _currentSelection.Add(gameObj);
        Messenger<int>.Broadcast("CntChanged", _currentSelection.Count);
    }

    public void Remove(GameObject gameObj)
    {
        SelectableUnit target = gameObj.GetComponent<SelectableUnit>();
        if (!target)
        {
            return;
        }

        gameObj.GetComponent<GoblinMovement>().Leader = null;
        if (_currentSelection.Contains(gameObj))
        {
            target.RemoveSelection();
            _currentSelection.Remove(gameObj);
        }
        Messenger<int>.Broadcast("CntChanged", _currentSelection.Count);
    }

    public void Inverse(GameObject gameObj)
    {
        if (_currentSelection.Contains(gameObj))
        {
            Remove(gameObj);
        }
        else
        {
            Add(gameObj);
        }
    }
}
