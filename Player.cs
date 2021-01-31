using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody, HittableByBullets
{
    [Export]
    public float Gravity = -24.8f;
    [Export]
    public float MaxSpeed = 20.0f;
    [Export]
    public float JumpSpeed = 18.0f;
    [Export]
    public float Accel = 4.5f;
    [Export]
    public float Deaccel = 16.0f;
    [Export]
    public float MaxSlopeAngle = 40.0f;
    [Export]
    public float MouseSensitivity = 0.05f;
    [Export]
    public float MaxSprintSpeed = 30.0f;
    [Export]
    public float SprintAccel = 18.0f;

    private bool _isSprinting = false;

    private Vector3 _vel = new Vector3();
    private Vector3 _dir = new Vector3();

    private Camera _camera;
    private Spatial _rotationHelper;

    public AnimationPlayerManager AnimationPlayer;

    private SpotLight _flashlight;

    string currentWeaponName = "UNARMED";
    Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon>() { { "UNARMED", null }, { "KNIFE", null }, { "PISTOL", null }, { "RIFLE", null }, { "", null } };
    Dictionary<int, string> weaponNumberToName = new Dictionary<int, string>() { { 0, "UNARMED" }, { 1, "KNIFE" }, { 2, "PISTOL" }, { 3, "RIFLE" }, { -1, "" } };
    Dictionary<string, int> weaponNameToNumber = new Dictionary<string, int>() { { "UNARMED", 0 }, { "KNIFE", 1 }, { "PISTOL", 2 }, { "RIFLE", 3 }, { "", 0 } };
    bool changingWeapon = false;
    string changingWeaponName = "UNARMED";

    float health = 100;

    Control UIStatusLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");
        _camera = GetNode<Camera>("Rotation_Helper/Camera");
        AnimationPlayer = GetNode<AnimationPlayerManager>("Rotation_Helper/Model/Animation_Player");
        AnimationPlayer.CallbackFunction = FireBullet;
        _rotationHelper = GetNode<Spatial>("Rotation_Helper");

        weapons["KNIFE"] = GetNode<Weapon>("Rotation_Helper/Gun_Fire_Points/Knife_Point");
        weapons["PISTOL"] = GetNode<Weapon>("Rotation_Helper/Gun_Fire_Points/Pistol_Point");
        weapons["RIFLE"] = GetNode<Weapon>("Rotation_Helper/Gun_Fire_Points/Rifle_Point");

        Vector3 gunAimPointPos = GetNode<Spatial>("Rotation_Helper/Gun_Aim_Point").GlobalTransform.origin;

        foreach (KeyValuePair<string, Weapon> weapon in weapons)
        {
            Weapon weaponNode = weapon.Value;
            if (weaponNode != null)
            {
                weaponNode.PlayerNode = this;
                weaponNode.LookAt(gunAimPointPos, new Vector3(0, 1, 0));
                weaponNode.RotateObjectLocal(new Vector3(0, 1, 0), Mathf.Deg2Rad(180));
            }
        }

        currentWeaponName = "UNARMED";
        changingWeaponName = "UNARMED";

        UIStatusLabel = GetNode<Control>("HUD/Panel/Gun_label");

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _PhysicsProcess(float delta)
    {
        ProcessInput(delta);
        ProcessMovement(delta);
        ProcessChangingWeapons(delta);

    }

    private void ProcessInput(float delta)
    {
        //  -------------------------------------------------------------------
        //  Walking
        _dir = new Vector3();
        Transform camXform = _camera.GlobalTransform;

        Vector2 inputMovementVector = new Vector2();

        if (Input.IsActionPressed("movement_forward"))
            inputMovementVector.y += 1;
        if (Input.IsActionPressed("movement_backward"))
            inputMovementVector.y -= 1;
        if (Input.IsActionPressed("movement_left"))
            inputMovementVector.x -= 1;
        if (Input.IsActionPressed("movement_right"))
            inputMovementVector.x += 1;

        inputMovementVector = inputMovementVector.Normalized();

        // Basis vectors are already normalized.
        _dir += -camXform.basis.z * inputMovementVector.y;
        _dir += camXform.basis.x * inputMovementVector.x;
        //  -------------------------------------------------------------------

        //  -------------------------------------------------------------------
        //  Jumping
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("movement_jump"))
                _vel.y = JumpSpeed;
        }
        //  -------------------------------------------------------------------

        //  -------------------------------------------------------------------
        //  Capturing/Freeing the cursor
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
                Input.SetMouseMode(Input.MouseMode.Captured);
            else
                Input.SetMouseMode(Input.MouseMode.Visible);
        }
        //  -------------------------------------------------------------------
        //  Sprinting
        if (Input.IsActionPressed("movement_sprint"))
            _isSprinting = true;
        else
            _isSprinting = false;
        //  -------------------------------------------------------------------

        //  -------------------------------------------------------------------
        //  Turning the flashlight on/off
        if (Input.IsActionJustPressed("flashlight"))
        {
            if (_flashlight.IsVisibleInTree())
                _flashlight.Hide();
            else
                _flashlight.Show();
        }
        //  ----------------------------------
        //  Changing weapons.
        int weaponChangeNumber = weaponNameToNumber[currentWeaponName];

        if (Input.IsActionPressed("weapon_1"))
        { weaponChangeNumber = 0; }
        if (Input.IsActionPressed("weapon_2"))
        { weaponChangeNumber = 1; }
        if (Input.IsActionPressed("weapon_3"))
        { weaponChangeNumber = 2; }
        if (Input.IsActionPressed("weapon_4"))
        { weaponChangeNumber = 3; }

        if (Input.IsActionJustPressed("shift_weapon_positive"))
        { weaponChangeNumber += 1; }
        if (Input.IsActionJustPressed("shift_weapon_negative"))
        { weaponChangeNumber -= 1; }

        weaponChangeNumber = Mathf.Clamp(weaponChangeNumber, 0, weaponNumberToName.Count - 1);

        if (changingWeapon == false)
        {
            if (weaponNumberToName[weaponChangeNumber] != currentWeaponName)
            {
                changingWeaponName = weaponNumberToName[weaponChangeNumber];
                changingWeapon = true;
            }
        }
        //  ----------------------------------

        //  ----------------------------------
        //  Firing the weapons
        if (Input.IsActionPressed("fire"))
        {
            if (changingWeapon == false)
            {
                Weapon currentWeapon = weapons[currentWeaponName];
                if (currentWeapon != null)
                {
                    if (AnimationPlayer.currentState == currentWeapon.IdleAnimationState)
                    {
                        AnimationPlayer.SetAnimation(currentWeapon.FireAnimationState);
                    }
                }
            }
        }
        //  ----------------------------------

    }

    private void ProcessMovement(float delta)
    {
        _dir.y = 0;
        _dir = _dir.Normalized();

        _vel.y += delta * Gravity;

        Vector3 hvel = _vel;
        hvel.y = 0;

        Vector3 target = _dir;

        if (_isSprinting)
            target *= MaxSprintSpeed;
        else
            target *= MaxSpeed;

        float accel;
        if (_dir.Dot(hvel) > 0)
            if (_isSprinting)
                accel = SprintAccel;
            else
                accel = Accel;
        else
            accel = Deaccel;

        hvel = hvel.LinearInterpolate(target, accel * delta);
        _vel.x = hvel.x;
        _vel.z = hvel.z;
        _vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
    }

    public void ProcessChangingWeapons(float delta)
    {
        if (changingWeapon == true)

        {
            bool weaponUnequipped = false;
            Weapon currentWeapon = weapons[currentWeaponName];

            if (currentWeapon == null)
            {
                weaponUnequipped = true;
            }
            else
            {
                if (currentWeapon.WeaponEnabled == true)
                {
                    weaponUnequipped = currentWeapon.UnequipWeapon();
                }
                else
                {
                    weaponUnequipped = true;
                }
            }

            if (weaponUnequipped == true)
            {
                bool weaponEquipped = false;
                Weapon weaponToEquip = weapons[changingWeaponName];

                if (weaponToEquip == null)
                {
                    weaponEquipped = true;
                }
                else
                {
                    if (weaponToEquip.WeaponEnabled == false)
                    { weaponEquipped = weaponToEquip.EquipWeapon(); }
                    else
                    { weaponEquipped = true; }
                }

                if (weaponEquipped == true)
                {
                    changingWeapon = false;
                    currentWeaponName = changingWeaponName;
                    changingWeaponName = "";
                }
            }
        }
    }

    public void FireBullet()
    {
        if (changingWeapon)
        {

        }
        else
        {
            weapons[currentWeaponName].FireWeapon();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
            _rotationHelper.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
            RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

            Vector3 cameraRot = _rotationHelper.RotationDegrees;
            cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
            _rotationHelper.RotationDegrees = cameraRot;
        }
    }

    public void BulletHit(float damage, Transform globalTransform)
    {
        throw new NotImplementedException();
    }
}