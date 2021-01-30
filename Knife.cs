using Godot;
using System;

public class Knife : Weapon
{

    public override void _Ready()
    {
        Damage = 40;

        IdleAnimationName = "Knife_idle";
        FireAnimationName = "Knife_fire";

        WeaponEnabled = false;

        PlayerNode = null;
    }

    public override void FireWeapon()
    {
        Area area = GetNode<Area>("Area");
        var bodies = area.GetOverlappingBodies();

        foreach (PhysicsBody body in bodies)
        {
            if (body == PlayerNode)
            {

            }
            else if (body is HittableByBullets hittableByBullets)
            {
                hittableByBullets.BulletHit(Damage, area.GlobalTransform);
            }
        }

    }

    public override bool EquipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationName)
        {
            WeaponEnabled = true;
            return true;
        }
        if (PlayerNode.AnimationPlayer.currentState.ToString() == "Idle_unarmed")
        {
            PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Knife_equip);
        }
        return false;
    }

    public override bool UnequipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationName)
        {
            if (PlayerNode.AnimationPlayer.currentState.ToString() != "Knife_unequip")
            {
                PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Knife_unequip);
            }
        }
        if (PlayerNode.AnimationPlayer.currentState.ToString() == "Idle_unarmed")
        {
            WeaponEnabled = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
