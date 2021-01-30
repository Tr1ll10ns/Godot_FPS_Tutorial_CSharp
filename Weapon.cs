using Godot;
using System;

public abstract class Weapon : Spatial
{
    public float Damage;
    public string IdleAnimationName;
    public string FireAnimationName;
    public bool WeaponEnabled;
    public Player PlayerNode = null;
    public override void _Ready()
    {

    }

    public abstract void FireWeapon();

    public abstract bool EquipWeapon();

    public abstract bool UnequipWeapon();
}
