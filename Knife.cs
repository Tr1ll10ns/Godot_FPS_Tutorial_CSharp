using Godot;
using System;

public class Knife : Spatial
{
    float Damage = 40;

    public string IdleAnimationName = "Knife_idle";
    public string FireAnimationName = "Knife_fire";

    public bool WeaponEnabled = false;

    public Player PlayerNode = null;

    public override void _Ready()
    {

    }

    public void FireWeapon()
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

    public bool EquipWeapon()
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

    public bool UnequipWeapon()
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
