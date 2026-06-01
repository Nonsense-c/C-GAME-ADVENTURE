using Gamaa;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Gamaa
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsOnGround;
        public bool IsFire;

        private bool _facingRight = true;

        private Texture2D _texture;
        private float _groundSpeed = 150f;
        private float _airSpeed = 300f;
        private float _jumpPower = -500f;
        private float _gravity = 1000f;

        public Player(Texture2D texture, Vector2 startPos, bool isFire)
        {
            _texture = texture;
            Position = startPos;
            IsFire = isFire;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }

        public void SetIsFire(bool isFire)
        {
            IsFire = isFire;
        }

        public void Update(float delta, KeyboardState kb, List<Rectangle> walls)
        {
            float currentSpeed = IsOnGround ? _groundSpeed : _airSpeed;

            // Горизонтальное движение (A/D или стрелки)
            float move = 0;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) move = -1;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) move = 1;
            Velocity.X = move * currentSpeed;

            if (move > 0) _facingRight = true;
            else if (move < 0) _facingRight = false;

            // Прыжок (W или Up)
            if ((kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)) && IsOnGround)
                Velocity.Y = _jumpPower;

            // Гравитация
            Velocity.Y += _gravity * delta;

            // Перемещение по X
            Position.X += Velocity.X * delta;
            CheckCollisionX(walls);

            // Перемещение по Y
            Position.Y += Velocity.Y * delta;
            IsOnGround = false;
            CheckCollisionY(walls);
        }

        public bool CanPassThrough(Hazard hazard)
        {
            if (IsFire)
            {
                // Огонь может проходить через лаву, но не через воду
                if (hazard.Type == "water") return false;
                if (hazard.Type == "lava") return true;
            }
            else
            {
                // Вода может проходить через воду, но не через лаву
                if (hazard.Type == "lava") return false;
                if (hazard.Type == "water") return true;
            }
            return true;
        }

        private void CheckCollisionX(List<Rectangle> walls)
        {
            Rectangle bounds = GetBounds();
            foreach (var wall in walls)
            {
                if (bounds.Intersects(wall))
                {
                    if (Velocity.X > 0)
                        Position.X = wall.Left - 32;
                    else if (Velocity.X < 0)
                        Position.X = wall.Right;
                }
            }
        }

        private void CheckCollisionY(List<Rectangle> walls)
        {
            Rectangle bounds = GetBounds();
            foreach (var wall in walls)
            {
                if (bounds.Intersects(wall))
                {
                    if (Velocity.Y > 0)
                    {
                        Position.Y = wall.Top - 32;
                        IsOnGround = true;
                        Velocity.Y = 0;
                    }
                    else if (Velocity.Y < 0)
                    {
                        Position.Y = wall.Bottom;
                        Velocity.Y = 0;
                    }
                }
            }
        }

        public Rectangle GetBounds() => new Rectangle((int)Position.X, (int)Position.Y, 32, 32);

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);

            // куда смотрим 
            SpriteEffects effect = _facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Рисуем с отражением
            spriteBatch.Draw(_texture, destRect, null, Color.White, 0f, Vector2.Zero, effect, 0f);
        }
    }
}