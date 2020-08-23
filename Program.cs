// Task 6.4HD
// John Ryder 219466419

// An implimentation of the board game Pandemic

using SplashKitSDK;

namespace Pandemic {
    
    // class to run the space game
    public class PlaySpaceGame {
        // create a new space game and play it
        public static void oldMain() {
            SpaceGame game = new SpaceGame();
            game.Play();
        }
    }

    // class for the space game
    public class SpaceGame {
        // Private Variables
        private Ship _myShip;
        private Window _gameWindow;
        
        // Constructor
        public SpaceGame() {
            LoadImages();
            _myShip = new Ship { X = 110, Y = 110 };
        }

        // Public Methods
        public void Play() {
            // create the game window
            _gameWindow = new Window("BlastOff", 600, 600);
            // while the user is not closing the window
            while (!_gameWindow.CloseRequested) {
                // look for keystrokes
                SplashKit.ProcessEvents();
                // respond to keystrokes
                if (SplashKit.KeyDown(KeyCode.UpKey)) {
                        _myShip.Move(4, 0);
                    }
                if (SplashKit.KeyDown(KeyCode.DownKey)) {
                    _myShip.Move(-4, 0);
                }
                if (SplashKit.KeyDown(KeyCode.LeftKey)) {
                    _myShip.Rotate(-4);
                }
                if (SplashKit.KeyDown(KeyCode.RightKey)) {
                    _myShip.Rotate(4);
                }
                if (SplashKit.KeyTyped(KeyCode.SpaceKey)) {
                    _myShip.Shoot();
                }
                // update the bullet position
                _myShip.UpdateBullet();
                DrawWindow();
            }
            // when user closes the window set the variable to null
            _gameWindow.Close();
            _gameWindow = null;
        }

        // Private Methods
        // load the bitmaps from files
        private void LoadImages() {
            SplashKit.LoadBitmap("Bullet", "Fire.png");
            SplashKit.LoadBitmap("Gliese", "Gliese.png");
            SplashKit.LoadBitmap("Pegasi", "Pegasi.png");
            SplashKit.LoadBitmap("Aquarii", "Aquarii.png");
        }
        
        // draw the window
        private void DrawWindow() {
            // clear the screen
            _gameWindow.Clear(Color.Black);
            // redraw the ship at the new position
            _myShip.Draw();
            // refresh the screen
            _gameWindow.Refresh(60);
        }
    }

    // class for the ship used in the space game        
    public class Ship {
        // Private Variables
        private double _x, _y;
        
        private double _angle;
        private Bitmap _shipBitmap;
        private Bullet _bullet = new Bullet();

        // Getters and Setters
        public double X {
            get { return _x; }
            set { _x = value; }
        }

        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double Angle {
            get { return _angle; }
            set { _angle = value; }
        }

        // Constructor
        public Ship() {
            Angle = 270; 
            _shipBitmap = SplashKit.BitmapNamed("Aquarii");
        }

        // rotate the angle (used by the ship) by a certain amount
        public void Rotate(double amount) {
            _angle = (_angle + amount) % 360;
        }

        // draw the ship and bullet
        public void Draw() {
            _shipBitmap.Draw(_x, _y, SplashKit.OptionRotateBmp(_angle));
            _bullet.Draw();
        }

        // shoot a bullet
        public void Shoot() {
            Matrix2D anchorMatrix = SplashKit.TranslationMatrix(SplashKit.PointAt(_shipBitmap.Width / 2, _shipBitmap.Height / 2));

            // Move centre point of picture to origin
            Matrix2D result = SplashKit.MatrixMultiply(SplashKit.IdentityMatrix(), SplashKit.MatrixInverse(anchorMatrix));
            // Rotate around origin
            result = SplashKit.MatrixMultiply(result, SplashKit.RotationMatrix(_angle));
            // Move it back...
            result = SplashKit.MatrixMultiply(result, anchorMatrix);

            // Now move to location on screen...
            result = SplashKit.MatrixMultiply(result, SplashKit.TranslationMatrix(X, Y));

            // Result can now transform a point to the ship's location
            // Get right/centre
            Vector2D vector = new Vector2D();
            vector.X = _shipBitmap.Width;
            vector.Y = _shipBitmap.Height / 2;
            // Transform it...
            vector = SplashKit.MatrixMultiply(result, vector);
            _bullet = new Bullet(vector.X, vector.Y, Angle);
        }

        // update the bullet on screen
        public void UpdateBullet() {
            _bullet.Update();
        }

        // move the position of the ship on screen
        public void Move(double amountForward, double amountStrafe) {
            Vector2D movement = new Vector2D();
            Matrix2D rotation = SplashKit.RotationMatrix(_angle);
            movement.X += amountForward;
            movement.Y += amountStrafe;
            movement = SplashKit.MatrixMultiply(rotation, movement);
            _x += movement.X;
            _y += movement.Y;
        }
    }

    // a class for the bullet object in the space game
    public class Bullet {
        // Private Variables
        private Bitmap _bulletBitmap;
        private double _x, _y, _angle;
        private bool _active = false;
        
        // Constructors
        public Bullet(double x, double y, double angle) {
            _bulletBitmap = SplashKit.BitmapNamed("Bullet");
            _x = x - _bulletBitmap.Width / 2;
            _y = y - _bulletBitmap.Height / 2;
            _angle = angle;_active = true;
        }

        public Bullet() {
            _active = false;
        }

        // update the bullet
        public void Update() {
            // the speed of the bullet
            const int XMOVEMENT = 8;
            Vector2D movement = new Vector2D();
            Matrix2D rotation = SplashKit.RotationMatrix(_angle); 
            movement.X += XMOVEMENT;
            movement = SplashKit.MatrixMultiply(rotation, movement);
            _x += movement.X;
            _y += movement.Y;
            // if the bullet is off screen, disable it
            if ((_x > SplashKit.ScreenWidth() || _x < 0) || _y > SplashKit.ScreenHeight() || _y < 0) {
                _active = false;
            }
        }

        // draw the bullet if it is active
        public void Draw() {
            if (_active) {
                DrawingOptions options = SplashKit.OptionRotateBmp(_angle);
                _bulletBitmap.Draw(_x, _y, options);
            }
        }
    }
}
