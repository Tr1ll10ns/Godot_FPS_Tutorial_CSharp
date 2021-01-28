using Godot;
using System;

public class Rifle : Spatial
{
    float Damage = 4;

    public string IdleAnimationName = "Rifle_idle";
    public string FireAnimationName = "Rifle_fire";

    public bool WeaponEnabled = false;

    public Player PlayerNode = null;

    public override void _Ready()
    {

    }

    public void FireWeapon()
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

    public bool EquipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationName)
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

    public bool UnequipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationName)
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