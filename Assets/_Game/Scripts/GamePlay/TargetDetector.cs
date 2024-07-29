using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class TargetDetector
{
    private static bool IsInvalidTarget(Character character, Character enemy) => IsInValidStatus(enemy);
    private static bool IsOutOfRange(Character character, Character enemy) => Vector3.Distance(enemy.TF.position, character.TF.position) > character.CurAttackRange;

    private static bool IsInValidStatus(Character character) => character.IsStatus(StatusType.Dead) || character.IsStatus(StatusType.Untouchable);
    public static void DetectNearestTarget(Character character, List<Character> targetsInRange)
    {
        if(!GameManager.IsState(GameState.GamePlay))
        {
            return;
        }

        if (!character.IsStatus(StatusType.Normal))
        {
            return;
        }

        character.CurTargetChar = null;
        float nearestDist = 0f;
        float checkingDist;
        foreach (Character enemy in targetsInRange)
        {
            if (IsInvalidTarget(character, enemy) || IsOutOfRange(character, enemy))
            {
                continue;
            }

            checkingDist = Vector3.Distance(enemy.TF.position, character.TF.position);
            if (nearestDist == 0f)
            {
                nearestDist = checkingDist;
                character.CurTargetChar = enemy;
            }

            if (checkingDist < nearestDist)
            {
                nearestDist = checkingDist;
                character.CurTargetChar = enemy;
            }
        }

        if(character is Player)
        {
            foreach (Character enemy in targetsInRange)
            {
                if (enemy)
                {
                    enemy.ToggleTargetIndicator(false);
                }
            }

            if (character.CurTargetChar != null)
            {
                character.CurTargetChar.ToggleTargetIndicator(true);
            }
        }
    }

    public static void RemoveTargetInRange(Character character, Character target)
    {
        character.ToggleTargetIndicator(false);
        character.TargetsInRange.Remove(target);
    }

    public static void AddTargetInRange(Character character, Character target)
    {
        if (!character.IsStatus(StatusType.Dead))
        {
            character.TargetsInRange.Add(target);
        }
    }
}
