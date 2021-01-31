using Godot;
using System;

public abstract class Weapon : Spatial
{
    public float Damage;
    public AnimationPlayerManager.AnimationState IdleAnimationState;
    public AnimationPlayerManager.AnimationState FireAnimationState;
    public bool WeaponEnabled;
    public Player PlayerNode = null;

    public override void _Ready()
    {

    }

    public abstract void FireWeapon();

    public abstract bool EquipWeapon();

    public abstract bool UnequipWeapon();
}
