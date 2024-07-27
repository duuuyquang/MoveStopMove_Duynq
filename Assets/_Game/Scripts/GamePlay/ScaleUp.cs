using UnityEngine;

public static class ScaleUp
{
    public static int[] SCALEUP_THRESHOLD = { 1, 5, 13, 25, 41, 61 }; // kill 4 enemies same scale to upscale: 4, 8, 12, 16, 20 (CombatPoint relatively)

    public static void CheckToProcess(Character character, int opponentCombatPoint)
    {
        int oldCombatPoint = character.CombatPoint;
        int gainnedPoint = GetPointInReturn(opponentCombatPoint);
        character.ShowCombatPointGainned(gainnedPoint);
        character.CombatPoint += gainnedPoint;

        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++)
        {
            if (oldCombatPoint < SCALEUP_THRESHOLD[i] && character.CombatPoint >= SCALEUP_THRESHOLD[i])
            {
                Process(character);
                return;
            }
        }
    }

    public static void Process(Character character, bool vfxOn = true)
    {
        character.CurSize += Const.CHARACTER_UPSCALE_UNIT;
        character.TF.localScale = Vector3.one * character.CurSize;
        character.TF.position += Vector3.up * Const.CHARACTER_UPSCALE_UNIT;
        if (vfxOn)
        {
            character.TriggerScaleUpVFX(Const.CHARACTER_UPSCALE_VFX_DELAY);
        }
        if(character is Enemy)
        {
            SoundManager.Instance.PlaySizeUp2(character.audioSource);
            character.InitSpeed();
        }
        else
        {
            SoundManager.Instance.PlaySizeUp1(character.audioSource);
        }
    }

    public static int GetPointInReturn(int point)
    {
        int returnPoint = SCALEUP_THRESHOLD.Length + 1;
        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++)
        {
            if (point <= SCALEUP_THRESHOLD[i])
            {
                returnPoint = i + 1;
                break;
            }
        }
        return returnPoint;
    }

    public static void ProcessByInitCombatPoint(Character character,int initPoint)
    {
        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++)
        {
            if (initPoint >= SCALEUP_THRESHOLD[i])
            {
                Process(character, false);
            }
        }
    }
}
