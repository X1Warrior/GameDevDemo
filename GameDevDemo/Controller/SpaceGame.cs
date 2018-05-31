using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GameDevDemo.Model;
using GameDevDemo.View;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
namespace SampleGame.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		public enum State
		{
			Menu,
			Playing,
			versus,
			Gameover
		}
		// Image used to display the static background
		private Texture2D mainBackground;
		private Texture2D menuItem;

		// Parallaxing Layers
		private ParallaxingBackground bgLayer1;
		private ParallaxingBackground bgLayer2;

		// Enemies
		private Texture2D enemyTexture;
		private List<Enemy> enemies;

		// The rate at which the enemies appear
		private TimeSpan enemySpawnTime;
		private TimeSpan previousSpawnTime;

		// A random number generator
		private Random random;

		// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState;
		// A movement speed for the player
		private float playerMoveSpeed;
		private Player player;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		private Texture2D projectileTexture;
		private List<Projectile> projectiles;

		public Texture2D texture, healthTexture;

		private Animation jet;

		private Player2 player2;
		private Animation player2Animation;

		private Player3 player3;
		private Animation player3Animation;
		private Animation plane;

		// The rate of fire of the player laser
		private TimeSpan fireTime;
		private TimeSpan previousFireTime;

		private Texture2D explosionTexture;
		private List<Animation> explosions;

		// The sound that is played when a laser is fired
		private SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		private SoundEffect explosionSound;

		// The music played during gameplay
		private Song gameplayMusic;

		//Number that holds the player score
		private int score;
		////// The font used to display UI elements
		//private SpriteFont font;

		public Texture2D gameoverImage;

		//Set first State
		State gameState = State.Menu;
		//HUD hud = new HUD();

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();
			// Initialize the enemies list
			enemies = new List<Enemy>();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			projectiles = new List<Projectile>();

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);

			explosions = new List<Animation>();



			//Set player's score to zero
			score = 0;

			player = new Player();

			player2 = new Player2();

			player3 = new Player3();

			// Set a constant player move speed
			playerMoveSpeed = 8.0f;
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Animation player2Animation = new Animation();
			Texture2D jet = Content.Load<Texture2D>("Animation/plane");
			player2Animation.Initialize(jet, Vector2.Zero, 125, 77, 8, 30, Color.White, 1f, true);

			Animation player3Animation = new Animation();
			Texture2D plane = Content.Load<Texture2D>("Animation/Animatedship");
			player3Animation.Initialize(plane, Vector2.Zero, 115, 62, 8, 30, Color.White, 1f, true);

			healthTexture = Content.Load<Texture2D>("Animation/red");
			Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition, healthTexture);
			player2.Initialize(player2Animation, playerPosition, healthTexture);
			player3.Initialize(player3Animation, playerPosition, healthTexture);
			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Texture/bgLayer2", GraphicsDevice.Viewport.Width, -2);

			spriteBatch = new SpriteBatch(GraphicsDevice);

			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");

			projectileTexture = Content.Load<Texture2D>("Texture/laser");

			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");

			explosionTexture = Content.Load<Texture2D>("Animation/explosion");

			menuItem = Content.Load<Texture2D>("Texture/MenuItem");

			gameoverImage = Content.Load<Texture2D>("Texture/gameOver");

			//// Load the score font
			//font = Content.Load("Font/gameFont");


			//Load the music
			gameplayMusic = Content.Load<Song>("Sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");

			// Start the music right away
			PlayMusic(gameplayMusic);

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		// Load the player resources 
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// TODO: Add your update logic here
			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);

			//UPDATING PLAYING STATE
			switch (gameState)
			{
				case State.Playing:
					{
						// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
						previousGamePadState = currentGamePadState;
						previousKeyboardState = currentKeyboardState;

						// Read the current state of the keyboard and gamepad and store it
						currentKeyboardState = Keyboard.GetState();
						currentGamePadState = GamePad.GetState(PlayerIndex.One);
						//Update the player
						UpdatePlayer(gameTime);
						//player.Update(gameTime);

						UpdatePlayer2(gameTime);
						//player2.Update(gameTime);

						// Update the parallaxing background
						bgLayer1.Update();
						bgLayer2.Update();

						// Update the enemies
						UpdateEnemies(gameTime);

						// Update the collision
						UpdateCollision();

						// Update the projectiles
						UpdateProjectiles();

						// Update the explosions
						UpdateExplosions(gameTime);

						if (player.Health <= 0)
							gameState = State.Gameover;
						if (player2.Health <= 0)
							gameState = State.Gameover;
						break;
					}

				case State.Menu:
					{
						//Get Keyboard State
						KeyboardState keyState = Keyboard.GetState();

						if (keyState.IsKeyDown(Keys.Enter))
						{
							gameState = State.Playing;
						}

						if (keyState.IsKeyDown(Keys.Space))
						{
							gameState = State.versus;
						}
						break;
					}

				case State.versus:
					{
						// Get Keyboard State
						KeyboardState KeyState = Keyboard.GetState();
						// Update the parallaxing background
						bgLayer1.Update();
						bgLayer2.Update();
						//Update the player
						UpdatePlayer(gameTime);
						UpdatePlayer3(gameTime);
								// Update the collision
						UpdateCollision();

						// Update the projectiles
						UpdateProjectiles();
						if (player.Health <= 0)
						{
							player.Health = 200;
							score = 0;
							gameState = State.Gameover;
						}
						if (player3.Health <= 0)
						{
							player3.Health = 200;
							score = 0;
							gameState = State.Gameover;
						}
							break;
					}

				case State.Gameover:
					{
						// Get Keyboard State
						KeyboardState KeyState = Keyboard.GetState();

						if (KeyState.IsKeyDown(Keys.Enter))
							gameState = State.Menu;
						// Stop Music
						MediaPlayer.Stop();
						break;
					}
			}
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Start drawing 
			spriteBatch.Begin();

			switch (gameState)
			{
				case State.Playing:
					{
						spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

						// Draw the moving background
						bgLayer1.Draw(spriteBatch);
						bgLayer2.Draw(spriteBatch);
						//TODO: Add your drawing code here
						// Draw the Enemies
						for (int i = 0; i < enemies.Count; i++)
						{
							enemies[i].Draw(spriteBatch);
						}
						// Draw the Projectiles
						for (int i = 0; i < projectiles.Count; i++)
						{
							projectiles[i].Draw(spriteBatch);
						}

						// Draw the explosions
						for (int i = 0; i < explosions.Count; i++)
						{
							explosions[i].Draw(spriteBatch);
						}
						//// Draw the score
						//spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
						// //Draw the player health
						//spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
						//Draw the Player 
						player.Draw(spriteBatch);
						player2.Draw(spriteBatch);
						break;
					}
				case State.Menu:
					{
						bgLayer1.Draw(spriteBatch);
						bgLayer2.Draw(spriteBatch);
						spriteBatch.Draw(menuItem, new Vector2(0, 0), Color.White);
						break;
					}
				case State.versus:
					{
						// Draw the moving background
						bgLayer1.Draw(spriteBatch);
						bgLayer2.Draw(spriteBatch);
						// Draw the Projectiles
						for (int i = 0; i<projectiles.Count; i++)
						{
							projectiles[i].Draw(spriteBatch);
						}

						// Draw the explosions
						for (int i = 0; i<explosions.Count; i++)
						{
							explosions[i].Draw(spriteBatch);
						}
						player.Draw(spriteBatch);
						player3.Draw(spriteBatch);
						break;
					}
				case State.Gameover:
					{
						spriteBatch.Draw(gameoverImage, new Vector2(0, 0), Color.White);
						//spriteBatch.DrawString("Your Final Schore was - " + new Vector2(235, 100), Color.Red);
						break;
					}
			}
			// Stop drawing 
			spriteBatch.End();

			base.Draw(gameTime);

		}


		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}


			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

			if (player.Health <= 0)
			{
				player.Health = 200;
				score = 0;
			}

		}


		private void UpdatePlayer2(GameTime gameTime)
		{
			player2.Update(gameTime);


			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.A))
			{
				player2.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D))
			{
				player2.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.W))
			{
				player2.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.S))
			{
				player2.Position.Y += playerMoveSpeed;
			}

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player2.Position + new Vector2(player2.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}


			// Make sure that the player does not go out of bounds
			player2.Position.X = MathHelper.Clamp(player2.Position.X, 0, GraphicsDevice.Viewport.Width - player2.Width);
			player2.Position.Y = MathHelper.Clamp(player2.Position.Y, 0, GraphicsDevice.Viewport.Height - player2.Height);

			if (player2.Health <= 0)
			{
				player2.Health = 200;
				score = 0;

			}
		}
		private void UpdatePlayer3(GameTime gameTime)
		{
			player3.Update(gameTime);


			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.A))
			{
				player3.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D))
			{
				player3.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.W))
			{
				player3.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.S))
			{
				player3.Position.Y += playerMoveSpeed;
			}

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player3.Position + new Vector2(player3.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}


			// Make sure that the player does not go out of bounds
			player3.Position.X = MathHelper.Clamp(player3.Position.X, 0, GraphicsDevice.Viewport.Width - player3.Width);
			player3.Position.Y = MathHelper.Clamp(player3.Position.Y, 0, GraphicsDevice.Viewport.Height - player3.Height);

			if (player3.Health <= 0)
			{
				player3.Health = 200;
				score = 0;

			}
		}
		private void AddEnemy()
		{
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position);

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}


		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--)
			{
				enemies[i].Update(gameTime);
				// If not active and health <= 0
				if (enemies[i].Health <= 0)
				{
					// Add an explosion
					AddExplosion(enemies[i].Position);
					// Play the explosion sound
					explosionSound.Play();
				}
				if (enemies[i].Active == false)
				{
					//Add to the player's score
					score += enemies[i].ScoreValue;
					enemies.RemoveAt(i);

				}
			}
		}
		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;
			Rectangle rectangle3;
			Rectangle rectangle4;
			Rectangle rectangle5;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
			rectangle3 = new Rectangle((int)player2.Position.X, (int)player2.Position.Y, player2.Width, player2.Height);
			rectangle5 = new Rectangle((int)player3.Position.X, (int)player3.Position.Y, player3.Width, player3.Height);


			// Do the collision between the player and the enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);
				rectangle4 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);

				// Determine if the two objects collided with each other
				if (rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
					{
						player.Active = false;
					}
					// Projectile vs Enemy Collision

				}

				if (rectangle3.Intersects(rectangle4))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player2.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player2.Health <= 0)
					{
						player2.Active = false;
					}
					// Projectile vs Enemy Collision

				}
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
			 projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2, (int)enemies[j].Position.Y - enemies[j].Height / 2, enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
			 projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

					rectangle5 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2, (int)enemies[j].Position.Y - enemies[j].Height / 2, enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle5))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}
		}
		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile();
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
			projectiles.Add(projectile);
		}
		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--)
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				}
			}
		}
		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
			explosions.Add(explosion);
		}
		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}
		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { } //No Exception is handled so it is an empty/anonymous exception
		}
	}
}


