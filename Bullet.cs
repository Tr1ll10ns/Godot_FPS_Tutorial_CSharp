using Godot;
using System;

public class Bullet : Spatial
{
    public float Speed = 70;
    public float Damage = 15;

    public float KillTimer = 4;
    public float Timer = 0;

    public bool HitSomething = false;

    public override void _Ready()
    {
        GetNode("Area").Connect("body_entered", this, "collided");
    }


    public override void _PhysicsProcess(float delta)
    {
        var forward_dir = GlobalTransform.basis.z.Normalized();

        GlobalTranslate(forward_dir * Speed * delta);


        Timer += delta;

        if (Timer >= KillTimer)
        {
            QueueFree();
        }
    }

    public void collided(CollisionObject2D body)
    {
        if (HitSomething == false)
        {
            if (body is HittableByBullets hittableByBullets)
            {
                hittableByBullets.BulletHit(Damage, GlobalTransform);
            }
        }
        HitSomething = true;
        QueueFree();
    }
}
