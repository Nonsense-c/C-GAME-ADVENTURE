using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Gamaa
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private List<Hazard> _hazards;
        private List<Rectangle> _walls;
        private Texture2D _pixelTex;
        private Texture2D _fireTex;
        private Texture2D _waterTex;
        private Texture2D _waterTexture;
        private Texture2D _fireTexture;
        private Texture2D _wallTexture;
        private Texture2D _vinesTexture;
        private Texture2D _cobbleTexture;

        private List<Crystal> _crystals;
        private Texture2D _fireCrystalTex;
        private Texture2D _waterCrystalTex;
        private int _score = 0;  // ← Вернули счёт

        private Vector2 _startPos;
        private bool _isFire = true;
        private KeyboardState _previousKeyboard;
        private float _globalTimeScale = 0.5f;

        private Gate _gate;                  
        private Texture2D _gateTexture;
        private bool _levelCompleted = false;
        private float _completionTimer = 0f;

        private Texture2D _background;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _startPos = new Vector2(100, 560);
        }

        protected override void Initialize()
        {
            _walls = new List<Rectangle>();
            _hazards = new List<Hazard>();
            _crystals = new List<Crystal>();
            _gate = null;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixelTex = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTex.SetData(new Color[] { Color.White });

            _background = Content.Load<Texture2D>("result_background1");
            _wallTexture = Content.Load<Texture2D>("rect_gray1");
            _vinesTexture = Content.Load<Texture2D>("floor_vines5");
            _cobbleTexture = Content.Load<Texture2D>("cobble_blood12");

            _waterTexture = Content.Load<Texture2D>("water1");
            _fireTexture = Content.Load<Texture2D>("lava1");

            _fireTex = Content.Load<Texture2D>("firechr");
            _waterTex = Content.Load<Texture2D>("waterchr");

            _fireCrystalTex = Content.Load<Texture2D>("red-diamond");
            _waterCrystalTex = Content.Load<Texture2D>("blue_diamond");

            _gateTexture = Content.Load<Texture2D>("dngn_enter_labyrinth");

            _player = new Player(_fireTex, _startPos, true);

            CreateTestLevel();
        }

        private void CreateTestLevel()
        {
            _walls.Clear();
            _hazards.Clear();
            _crystals.Clear();

            _hazards.Add(new Hazard(new Rectangle(400, 555, 80, 32), "water"));
            _hazards.Add(new Hazard(new Rectangle(200, 560, 80, 32), "lava"));
            _hazards.Add(new Hazard(new Rectangle(390, 448, 60, 32), "lava"));
            //_hazards.Add(new Hazard(new Rectangle(400, 555, 80, 32), "water"));
            //_hazards.Add(new Hazard(new Rectangle(400, 555, 80, 32), "water"));

            _crystals.Add(new Crystal(new Rectangle(200, 390, 32, 32), "fire"));
            _crystals.Add(new Crystal(new Rectangle(400, 390, 32, 32), "water"));
            _crystals.Add(new Crystal(new Rectangle(700, 390, 32, 32), "fire"));
            _crystals.Add(new Crystal(new Rectangle(300, 280, 32, 32), "water"));
            _crystals.Add(new Crystal(new Rectangle(500, 280, 32, 32), "fire"));

            for (int x = 0; x < 25; x++)
            {
                _walls.Add(new Rectangle(x * 32, 568, 32, 32));
                _walls.Add(new Rectangle(x * 32, 0, 32, 32));
            }

            for (int y = 0; y < 25; y++)
            {
                _walls.Add(new Rectangle(0, y * 32, 32, 32));
                _walls.Add(new Rectangle(768, y * 32, 32, 32));
            }

            for (int y = 0; y < 5; y++)
            {
                _walls.Add(new Rectangle(600, y * 32, 32, 32));
            }

            for (int x = 0; x < 18; x++)
            {
                _walls.Add(new Rectangle(x * 32, 460, 32, 32));
            }

            for (int x = 0; x < 10; x++)
            {
                _walls.Add(new Rectangle(x * 32, 200, 32, 32));
            }

            for (int x = 25; x > 15; x--)
            {
                _walls.Add(new Rectangle(x * 32, 230, 32, 32));
            }

            for (int x = 25; x > 5; x--)
            {
                _walls.Add(new Rectangle(x * 32, 340, 32, 32));
            }

            _gate = new Gate(new Rectangle(100, 100, 100, 100));

        }

        protected override void Update(GameTime gameTime)
        {
            float realDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float delta = realDelta * _globalTimeScale;
            KeyboardState currentKeyboard = Keyboard.GetState();

            if (currentKeyboard.IsKeyDown(Keys.Escape)) Exit();

            // Переключение между огнём и водой
            if (currentKeyboard.IsKeyDown(Keys.Tab) && _previousKeyboard.IsKeyUp(Keys.Tab))
            {
                _isFire = !_isFire;

                if (_isFire)
                {
                    _player.SetTexture(_fireTex);
                    _player.SetIsFire(true);
                }
                else
                {
                    _player.SetTexture(_waterTex);
                    _player.SetIsFire(false);
                }
            }

            _player.Update(delta, currentKeyboard, _walls);

            // Проверка опасностей
            foreach (var hazard in _hazards)
            {
                if (!_player.CanPassThrough(hazard) && _player.GetBounds().Intersects(hazard.Area))
                {
                    _player.Position = _startPos;
                    _player.Velocity = Vector2.Zero;
                }
            }

            // Сбор кристаллов
            foreach (var crystal in _crystals)
            {
                if (!crystal.IsCollected && _player.GetBounds().Intersects(crystal.Bounds))
                {
                    if ((crystal.Type == "fire" && _isFire) || (crystal.Type == "water" && !_isFire))
                    {
                        crystal.IsCollected = true;
                        _score += 1000;
                    }
                }
            }

            // Перезапуск по R
            if (currentKeyboard.IsKeyDown(Keys.R))
            {
                _player.Position = _startPos;
                _player.Velocity = Vector2.Zero;
                _score = 0;
                CreateTestLevel();
            }

            // Проверка ворот (завершение уровня)
            if (_gate != null && _gate.IsActive && _player.GetBounds().Intersects(_gate.Bounds))
            {
                _levelCompleted = true;
                _gate.IsActive = false;
                _score += 5000;
            }

            // Если уровень пройден
            if (_levelCompleted)
            {
                _completionTimer += realDelta;
                if (_completionTimer >= 2f)
                {
                    _levelCompleted = false;
                    _completionTimer = 0f;
                    _score = 0;
                    _player.Position = _startPos;
                    _player.Velocity = Vector2.Zero;
                    CreateTestLevel();
                }
            }

            // Показываем счёт в заголовке окна
            this.Window.Title = $"Счёт: {_score} | Режим: {(_isFire ? "ОГОНЬ" : "ВОДА")} | Скорость: {_globalTimeScale * 100}%";

            _previousKeyboard = currentKeyboard;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);

            foreach (var wall in _walls)
            {
                int blockX = wall.X / 32;
                Texture2D currentTexture;

                if (blockX % 3 == 0)
                {
                    currentTexture = _cobbleTexture;
                }
                else if (blockX % 2 == 0)
                {
                    currentTexture = _vinesTexture;
                }
                else
                {
                    currentTexture = _wallTexture;
                }
                _spriteBatch.Draw(currentTexture, wall, Color.White);
            }

            foreach (var hazard in _hazards)
            {
                Texture2D hazardTexture = hazard.Type == "lava" ? _fireTexture : _waterTexture;
                _spriteBatch.Draw(hazardTexture, hazard.Area, Color.White);
            }

            foreach (var crystal in _crystals)
            {
                crystal.Draw(_spriteBatch, _fireCrystalTex, _waterCrystalTex);
            }

            if (_gate != null)
            {
                _gate.Draw(_spriteBatch, _gateTexture);
            }

            _player.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}