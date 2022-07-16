using UnityEngine;

public class GeneralDieComponent : BaseDieComponent
{
    private Passive passive1;
    private Passive passive2;
    private Passive passive3;

    public Passive currentPassive;

    private void Update()
    {
        UpdateBase();
    }

    protected override void OnRollBegin()
    {
        currentPassive = GeneratePassive();
    }

    public void SetPassives(Passive passive1, Passive passive2, Passive passive3)
    {
        this.passive1 = passive1;
        this.passive2 = passive2;
        this.passive3 = passive3;
    }

    private Passive GeneratePassive()
    {
        int value = Random.Range(1, 4);
        if (value == 1) return passive1;
        if (value == 2) return passive2;
        if (value == 3) return passive3;
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(transform.position, "");
    }
#endif // UNITY_EDITOR
}
