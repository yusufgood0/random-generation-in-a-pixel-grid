using System;
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
        private (int, int) _screenSize;
        private readonly int[] weights = new int[] { 10, 12 };
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _screenSize = (1000, 1000);
            _graphics.PreferredBackBufferWidth = _screenSize.Item1;
            _graphics.PreferredBackBufferHeight = _screenSize.Item2;
            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            _seedMapper = new seedMapper(200, 200, weights, 80, 30);
            
            for (int i = 0; i < 3; i++)
            {
                _seedMapper.SmoothenTerrain();
            }
            _seedMapper.SmoothenHeights(5);
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _seedMapper.draw(_spriteBatch, Point.Zero, 5, 5, _blankTexture, new Color[] {Color.Blue, Color.Green, Color.DarkGreen});

            base.Draw(gameTime);
        }
    }
}
