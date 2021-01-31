using Godot;
using System;

public class RigidBodyHitTest : RigidBody, HittableByBullets
{
    float baseBulletBoost = 9;

    public override void _Ready()
    {

    }

    public void BulletHit(float damage, Transform BulletGlobalTransform)
    {
        Vector3 direction_vect = BulletGlobalTransform.basis.z.Normalized() * baseBulletBoost;

        ApplyImpulse((BulletGlobalTransform.origin - GlobalTransform.origin).Normalized(), direction_vect * damage);
    }

}
