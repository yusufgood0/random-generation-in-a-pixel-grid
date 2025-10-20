using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace random_generation_in_a_pixel_grid
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private seedMapper _seedMapper;
        private Texture2D _blankTexture;
        private (int X, int Y) _mapSize;
        private int _pixelSize = 5;
        private int maxSeaLevel = 80;
        private int minSeaLevel = 0;
        private float _scrollstate;
        private int _seaLevel;
        private readonly int[] weights = new int[] { 10, 12 };
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _mapSize = (20, 20);
            _graphics.PreferredBackBufferWidth = _mapSize.X * _pixelSize;
            _graphics.PreferredBackBufferHeight = _mapSize.Y * _pixelSize;
            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            _seedMapper = new seedMapper(_mapSize.X, _mapSize.Y, weights, maxSeaLevel, null);
            
            for (int i = 0; i < 2; i++)
            {
                
                _seedMapper.SmoothenTerrain();
            }
            _seedMapper.SmoothenHeights(8);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData<Color>(0, new Rectangle(0, 0, 1, 1), new Color[] { Color.White }, 0, 1);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState mouse = Mouse.GetState();
            int x = Math.Clamp(mouse.X / _pixelSize, 0, _mapSize.X-1);
            int y = Math.Clamp(mouse.Y / _pixelSize, 0, _mapSize.Y - 1);
            if (mouse.ScrollWheelValue != _scrollstate)
            {
                if (mouse.ScrollWheelValue > _scrollstate)
                {
                    _seaLevel = Math.Min(_seaLevel + 1, maxSeaLevel);
                }
                else
                {
                    _seaLevel = Math.Max(_seaLevel - 1, 0);
                }
                _seedMapper.ApplySeaLevel(_seaLevel);
                _scrollstate = mouse.ScrollWheelValue;
            }
            Debug.WriteLine(_seedMapper.GetValue(x, y));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _seedMapper.draw(_spriteBatch, Point.Zero, _pixelSize, _pixelSize, _blankTexture, new Color[] {Color.Blue, Color.DarkGreen, Color.DarkGreen});

            base.Draw(gameTime);
        }
    }
}
