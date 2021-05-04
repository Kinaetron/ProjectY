using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Input;
using PolyOne.Components;

namespace ProjectY
{
    public class Player : Entity
    {
        private Texture2D playerTexture;

        private Vector2 remainder;
        private Vector2 velocity;

        private const float fallspeed = 6.0f;
        private const float airFriction = 0.66f;
        private const float gravityUp = 0.31f;
        private const float gravityDown = 0.21f;
        private const float initialJumpHeight = -4.8f;

        private const float turnMul = 0.9f;
        private const float runAccel = 1.0f;
        private const float horizontalSpeedLimit = 3.5f;

        private int lastDirection = 0;
        private const int dashLimit = 3;
        private const float dashTime = 200.0f;
        private const float dashAccel = 1.5f;
        private const float dashSpeedLimit = 8.0f;
        private const float dashLimitTime = 1000.0f;
        private const float dashCoolDownTime = 500.0f;

        private int shootCount = 0;
        private const int shootLimt = 3;
        private const float shootTimeLimit = 500.0f;
        private const float shootCoolDownTime = 250.0f;

        private int dashCount = 0;

        private InputType inputType = InputType.Keyboard;
        private Keys[] keyList = new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D, Keys.Up,
                                              Keys.Down, Keys.Left, Keys.Right ,Keys.Space };

        private const float ledgeBufferTime = 33.34f;
        private const float jumpButtonBufferTime = 33.34f;

        private CounterSet<string> counters = new CounterSet<string>();

        Direction direction = Direction.Right;

        private StateMachine state = new StateMachine(2);

        public Player(Vector2 position)
        : base(position)
        {
            Tag((int)GameTags.Player);
            Collider = new Hitbox((float)16.0f, (float)32.0f, 0.0f, 0.0f);
            Visible = true;

            playerTexture = Engine.Instance.Content.Load<Texture2D>("Player");

            state.SetCallbacks(0, new Func<int>(NormalUpdate), null, new Action(EnterNormal), new Action(LeaveNormal));
            state.SetCallbacks(1, new Func<int>(DashUpdate), null, new Action(EnterDash), new Action(LeaveDash));

            this.Add(state);
            this.Add(counters);
        }

        public override void Added(Scene Scene)
        {
            base.Added(Scene);
        }

        private void EnterNormal()
        {
        }

        private void LeaveNormal()
        {
        }

        private int NormalUpdate()
        {
            InputMode();
            Move();
            Jump();
            Shoot();
            ClampVelocity();

            return EnterDashMode();
        }

        private void EnterDash()
        {
            counters["dashTimer"] = dashTime;
            velocity = Vector2.Zero;
        }

        private void LeaveDash()
        {
            velocity = Vector2.Zero;
        }

        private int DashUpdate()
        {
            velocity.X += dashAccel * lastDirection;
            DashClampVelocity();


            return EnterNormalMode();
        }

        private int EnterDashMode()
        {
            if (PolyInput.GamePads[0].RightTriggerPressed(0.3f) == true)
            {
                if (counters["dashCoolDownTimer"] > 0) {
                    return 0;
                }

                if (counters["dashCoolTimer"] <= 0) {
                    dashCount++;
                }

                if (dashCount >= dashLimit && counters["dashLimitTimer"] > 0)
                {
                    counters["dashCoolDownTimer"] = dashCoolDownTime;
                    counters["dashLimitTimer"] = 0;
                    dashCount = 0;
                }

                if (dashCount == 1) {
                    counters["dashLimitTimer"] = dashLimitTime;
                }

                return 1;
            }
            else {
                return 0;
            }
        }

        private int EnterNormalMode()
        {
            if (PolyInput.GamePads[0].RightTriggerReleased(0.3f) == true ||
                counters["dashTimer"] <= 0) {
                return 0;
            }
            else {
                return 1;
            }
        }

        private bool IsOnGround() {
            return CollideCheck((int)GameTags.Tile, this.Position + Vector2.UnitY);
        }

        private int MovementInput()
        {
            if (inputType == InputType.Controller)
            {
                if (PolyInput.GamePads[0].LeftStickHorizontal(0.2f) > 0.4f ||
                    PolyInput.GamePads[0].DPadRightCheck == true)
                {
                    direction = Direction.Right;
                    lastDirection = 1;
                    return 1;
                }
                else if (PolyInput.GamePads[0].LeftStickHorizontal(0.2f) < -0.4f ||
                         PolyInput.GamePads[0].DPadLeftCheck == true)
                {
                    direction = Direction.Left;
                    lastDirection = -1;
                    return -1;
                }
            }
            else if (inputType == InputType.Keyboard)
            {
                if (PolyInput.Keyboard.Check(Keys.Right) ||
                    PolyInput.Keyboard.Check(Keys.D))
                {
                    direction = Direction.Right;
                    lastDirection = 1;
                    return 1;
                }
                else if (PolyInput.Keyboard.Check(Keys.Left) ||
                         PolyInput.Keyboard.Check(Keys.A))
                {
                    direction = Direction.Left;
                    lastDirection = -1;
                    return -1;
                }
            }
            return 0;
        }

        private void InputMode()
        {
            foreach (Keys key in keyList)
            {
                if (PolyInput.Keyboard.Check(key) == true) {
                    inputType = InputType.Keyboard;
                }
            }
            if (PolyInput.GamePads[0].ButtonCheck() == true) {
                inputType = InputType.Controller;
            }
        }

        private void Move()
        {
            if (IsOnGround() == false) {
                velocity.X *= airFriction;
            }

            if (MovementInput() != 0 && MovementInput() != Math.Sign(velocity.X)) {
                velocity.X *= turnMul;
            }

            velocity.Y += gravityDown;
            velocity.X += runAccel * MovementInput();

            if(MovementInput() == 0) {
                velocity.X = 0.0f;
            }
        }

        private void Jump()
        {
            if (IsOnGround() == true) {
                counters["ledgeBuffer"] = ledgeBufferTime;
            }

            if (inputType == InputType.Controller)
            {
                if (PolyInput.GamePads[0].Pressed(Buttons.A) == true) {
                    counters["jumpButtonBuffer"] = jumpButtonBufferTime;
                }

                if (counters["jumpButtonBuffer"] > 0)
                {
                    if (IsOnGround() == true || counters["ledgeBuffer"] > 0)
                    {
                        counters["jumpButtonBuffer"] = 0.0f;
                        velocity.Y = initialJumpHeight;
                    }
                }
                else if (PolyInput.GamePads[0].Released(Buttons.A) == true && velocity.Y < 0.0f)
                {
                    counters["jumpButtonBuffer"] = 0.0f;
                    velocity.Y = 0.0f;
                }
            }
            else if (inputType == InputType.Keyboard)
            {
                if (PolyInput.Keyboard.Pressed(Keys.Space) == true) {
                    counters["jumpButtonBuffer"] = jumpButtonBufferTime;
                }

                if (counters["jumpButtonBuffer"] > 0)
                {
                    if (IsOnGround() == true || counters["ledgeBuffer"] > 0)
                    {
                        counters["jumpButtonBuffer"] = 0.0f;
                        velocity.Y = initialJumpHeight;
                    }
                }
                else if (PolyInput.Keyboard.Released(Keys.Space) == true && velocity.Y < 0.0f)
                {
                    counters["jumpButtonBuffer"] = 0.0f;
                    velocity.Y = 0.0f;
                }
            }
        }

        private void Shoot()
        {
            if(counters["shootCoolDownTimer"] > 0) {
                return;
            }

            if (counters["shootTimer"] <= 0)
            {
                shootCount = 0;
                counters["shootTimer"] = shootTimeLimit;
            }

            if (shootCount >= shootLimt && counters["shootTimer"] > 0) {
                counters["shootCoolDownTimer"] = shootCoolDownTime;
                counters["shootTimer"] = 0;
                shootCount = 0;
            }

            if(inputType == InputType.Controller)
            {
                if(PolyInput.GamePads[0].Pressed(Buttons.X)) {
                    Bullet bullet = new Bullet(new Vector2(Position.X + 10, Position.Y + 8), direction);
                    Scene.Add(bullet);
                    bullet.Added(this.Scene);
                    shootCount++;
                }
            }

            if (inputType == InputType.Keyboard)
            {
                if (PolyInput.Keyboard.Pressed(Keys.X))
                {
                    Bullet bullet = new Bullet(new Vector2(Position.X + 10, Position.Y + 8), direction);
                    Scene.Add(bullet);
                    bullet.Added(this.Scene);
                    shootCount++;
                }
            }
        }

        private void DashClampVelocity()
        {
            velocity.X = MathHelper.Clamp(velocity.X, -dashSpeedLimit, dashSpeedLimit);
            MovementHorizontal(velocity.X);
        }

        private void ClampVelocity()
        {
            velocity.X = MathHelper.Clamp(velocity.X, -horizontalSpeedLimit, horizontalSpeedLimit);
            MovementHorizontal(velocity.X);

            velocity.Y = MathHelper.Clamp(velocity.Y, initialJumpHeight, fallspeed);
            MovementVerical(velocity.Y);
        }

        private void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);

                    if (CollideFirst((int)GameTags.Tile, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);

                    if (CollideFirst((int)GameTags.Tile, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        private void MovementHorizontal(float amount)
        {
            remainder.X += amount;
            int move = (int)Math.Round((double)remainder.X);

            if (move != 0)
            {
                remainder.X -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(sign, 0);

                    if (this.CollideFirst((int)GameTags.Tile, newPosition) != null)
                    {
                        velocity.X = 0;
                        remainder.X = 0;
                        break;
                    }

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Engine.SpriteBatch.Draw(playerTexture, this.Position, Color.White);
        }
    }
}