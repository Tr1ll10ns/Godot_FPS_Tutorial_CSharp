using Godot;
using System;

public class Pistol : Spatial
{
    public float Damage = 15;

    public string IdleAnimationName = "Pistol_idle";
    public string FireAnimationName = "Pistol_fire";

    public bool WeaponEnabled = false;

    [Export]
    public PackedScene BulletScene;

    public Player PlayerNode = null;

    public override void _Ready()
    {

    }

    public void FireWeapon()
    {
        Bullet bullet = BulletScene.Instance() as Bullet;
        Node sceneRoot = GetTree().Root.GetChildren()[0] as Node;
        sceneRoot.AddChild(bullet);

        bullet.GlobalTransform = this.GlobalTransform;
        bullet.Scale = new Vector3(4, 4, 4);
        bullet.Damage = Damage;
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
            PlayerNode.AnimationPlayer.SetAnimation(AnimationPlayerManager.AnimationState.Pistol_equip);
        }
        return false;
    }

    public bool UnequipWeapon()
    {
        if (PlayerNode.AnimationPlayer.currentState.ToString() == IdleAnimationName)
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
