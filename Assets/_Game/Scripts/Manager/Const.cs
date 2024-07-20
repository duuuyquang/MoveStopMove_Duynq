using System.Collections.Generic;

public static class Const
{
    public static List<string> ENEMY_NAME_LIST = new() { "Dazzle", "Crystal", "Lina", "Clink", "Axe", "Phantom", "Sniper", "TrollWarlord", "BrewMaster", "Hoodwink", "Winranger", "Traxex", "Enchantress", "Luna" };

    public const string TAG_NAME_ENEMY          = "Enemy";
    public const string TAG_NAME_PLAYER         = "Player";
    public const string TAG_NAME_BULLET         = "Bullet";
    public const string TAG_NAME_ATK_RANGE      = "AttackRange";

    public const string ANIM_NAME_IDLE          = "idle";
    public const string ANIM_NAME_RUN           = "run";
    public const string ANIM_NAME_DANCE         = "dance";
    public const string ANIM_NAME_ATTACK        = "attack";
    public const string ANIM_NAME_DIE           = "die";
    public const string ANIM_NAME_SHOP          = "shop";

    public const float CHARACTER_UPSCALE_UNIT   = 0.1f;

    public const int WEAPON_BASE_BULLET_AMOUNT  = 1;

    public const int COMBAT_POINT_DEFAULT_SIZE  = 80;
}
