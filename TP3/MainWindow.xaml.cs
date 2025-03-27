using HugoWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TP3
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stopwatch _timer = new Stopwatch();
        private double _lastTime;
        private long _frameCounter;
        private GameState _gameState;

        System.Windows.Threading.DispatcherTimer drawTimer = new System.Windows.Threading.DispatcherTimer();


        public MainWindow()
        {
            InitializeComponent();
            //Startup the game state
            _gameState = new GameState();
            drawTimer.Tick += new EventHandler(drawTimer_Tick);
            drawTimer.Interval = new TimeSpan(0, 0, 0,0,30);
            drawTimer.Start();

            Initialize();

        }

        private void Initialize()
        {
            _gameState.Initialize();

            //Initialise and start the timer
            _lastTime = 0.0;
            _timer.Reset();
            _timer.Start();

        }

        private void drawTimer_Tick(object sender, EventArgs e)
        {
            DessinerJeu();
        }


        private void DessinerJeu()
        {
            //Work out how long since we were last here in seconds
            double gameTime = _timer.ElapsedMilliseconds / 1000.0;
            double elapsedTime = gameTime - _lastTime;
            _lastTime = gameTime;
            _frameCounter++;


            //Perform any animation and updates
            _gameState.Update(gameTime, elapsedTime);


            //Draw everything
            _gameState.Draw(imgJeu);


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            _gameState.KeyDown(e.Key);
        }
    }
}
