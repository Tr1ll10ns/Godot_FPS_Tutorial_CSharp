using Godot;
using System;

public class Rifle : Weapon
{

    public override void _Ready()
    {
        Damage = 4;

        IdleAnimationState = AnimationPlayerManager.AnimationState.Rifle_idle;
        FireAnimationState = AnimationPlayerManager.AnimationState.Rifle_fire;

        WeaponEnabled = false;

        PlayerNode = null;
    }

    public override void FireWeapon()
    {
        RayCast ray = GetNode<RayCast>("Ray_Cast"); ;
        ray.ForceRaycastUpdate();

        if (ray.IsColliding())
        {
            var body = ray.GetCollider();

            if (body == PlayerNode)
            {

            }
            else if (body is HittableByBullets hittableByBullets)
            {
                hittableByBullets.BulletHit(Damage, ray.GlobalTransform);
            }
        }
    }

    public override bool EquipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationState.ToString())
        {
            WeaponEnabled = true;
            return true;
        }
        if (PlayerNode.AnimationPlayer.currentState.ToString() == "Idle_unarmed")
        {
            PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Rifle_equip);
        }
        return false;
    }

    public override bool UnequipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationState.ToString())
        {
            if (PlayerNode.AnimationPlayer.currentState.ToString() != "Rifle_unequip")
            {
                PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Rifle_unequip);
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