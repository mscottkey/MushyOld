using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MushyExtensionMethods
{
    public class GameOrchestrator
    {
        public TabControl _tabs;
        public List<Game> _games;
        public TabItem _currentTab;

        private static GameOrchestrator _instance = null;

        public static GameOrchestrator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameOrchestrator(MainWindow.Instance.MushyTabs);
                }
                return _instance;
            }
        }

        public GameOrchestrator(TabControl tabs)
        {
            // He gets the tabs from mainwindow
            _tabs = tabs;
            _games = new List<Game>();
            this.AddGame();
        }

        public void AddGame()
        {
            // Games are initially not connected
            // Give the game connector view a reference to ourself for when it's ready to connect.
            var newGame = new NewGameView(this);
            // A new blank game window always goes at the end
            _tabs.Items.Add(newGame);
            TabItem _currentTab = MainWindow.Instance.MushyTabs.SelectedItem as TabItem;
            MainWindow.Instance._tabItems.Add(_currentTab);
        }

        // Note - using the parent class UserControl lets you pass either type of view
        public void CloseGame(TabItem view)
        {
            _tabs.Items.Remove(view);
            var game = FindGameForView(view);
            if (game != null)
            {
                game.Model.Disconnect();
            }
        }

        public void ConnectGame(TabItem view, ConnectionInfo connectionInfo)
        {
            int index = FindTabIndex(view);

            // Remove the old connector view
            _tabs.Items.RemoveAt(index);

            // Make up a new game and add it to the games list
            var game = GameFactory.NewGame(connectionInfo);
            _games.Add(game);

            // Add the game view in the same position on the tab control
            _tabs.Items.Insert(index, game.View);
        }

        private int FindTabIndex(TabItem view)
        {
            for (int i = 0; i < _tabs.Items.Count - 1; i++)
            {
                if (_tabs.Items[i] == view)
                {
                    return i;
                }
            }
            return 0;
        }
        public Game FindGameForView(TabItem view)
        {
            foreach (var g in _games)
            {
                if (g.View == view)
                {
                    return g;
                }
            }
            return null;
        }
    }
}
