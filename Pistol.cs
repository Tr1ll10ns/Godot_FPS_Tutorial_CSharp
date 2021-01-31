using Godot;
using System;

public class Pistol : Weapon
{

    [Export]
    public PackedScene BulletScene;

    public override void _Ready()
    {
        Damage = 15;

        IdleAnimationState = AnimationPlayerManager.AnimationState.Pistol_idle;
        FireAnimationState = AnimationPlayerManager.AnimationState.Pistol_fire;

        WeaponEnabled = false;
        PlayerNode = null;
    }

    public override void FireWeapon()
    {
        Bullet bullet = BulletScene.Instance() as Bullet;
        Node sceneRoot = GetTree().Root.GetChildren()[0] as Node;
        sceneRoot.AddChild(bullet);

        bullet.GlobalTransform = this.GlobalTransform;
        bullet.Scale = new Vector3(4, 4, 4);
        bullet.Damage = Damage;
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
            PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Pistol_equip);
        }
        return false;
    }

    public override bool UnequipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationState.ToString())
        {
            if (PlayerNode.AnimationPlayer.currentState.ToString() != "Pistol_unequip")
            {
                PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Pistol_unequip);
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
