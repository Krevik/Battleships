using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleships
{
    public partial class Form1 : Form
    {
        
        struct Ship
        {
            public List<CoordsXY> coords;
        }

        struct CoordsXY
        {
            public int X;
            public int Y;
        }

        List<Ship> playerShips;
        List<Ship> computerShips;

        bool isTheGameStarted = false;
        int turn;

        List<Button> playerTableButtons;
        List<Button> enemyTableButtons;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            SetupLayout();
        }

        private void InitializeGame()
        {
            playerShips = new List<Ship>();
            computerShips = new List<Ship>();
            ConsoleLogger.AppendText("Game loaded without errors.");
            ConsoleLogger.AppendText(Environment.NewLine);
            StartGameButton.Visible = true;
            StartGameButton.Enabled = true;
        }




        private void SetupLayout()
        {

            var rowCount = 10;
            var columnCount = 10;
            playerTableButtons = new List<Button>();
            enemyTableButtons = new List<Button>();

            //create players buttons table
            this.tableLayoutPanel1.ColumnCount = columnCount;
            this.tableLayoutPanel1.RowCount = rowCount;

            this.tableLayoutPanel1.ColumnStyles.Clear();
            this.tableLayoutPanel1.RowStyles.Clear();

            for (int i = 0; i < columnCount; i++)
            {
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100 / columnCount));
            }
            for (int i = 0; i < rowCount; i++)
            {
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100 / rowCount));
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    string columnName = Encoding.ASCII.GetString(new byte[] { (byte)(65+j) });
                    var button = new Button();
                    button.Text = string.Format("{0}{1}", columnName, i);
                    button.Name = string.Format("button_{0}{1}", i, j);
                    button.Dock = DockStyle.Fill;
                    button.Enabled = false;
                    this.tableLayoutPanel1.Controls.Add(button, j, i);
                    playerTableButtons.Add(button);
                }
            }

            //create enemy buttons table
            this.tableLayoutPanel2.ColumnCount = columnCount;
            this.tableLayoutPanel2.RowCount = rowCount;

            this.tableLayoutPanel2.ColumnStyles.Clear();
            this.tableLayoutPanel2.RowStyles.Clear();

            for (int i = 0; i < columnCount; i++)
            {
                this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100 / columnCount));
            }
            for (int i = 0; i < rowCount; i++)
            {
                this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100 / rowCount));
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    string columnName = Encoding.ASCII.GetString(new byte[] { (byte)(65 + j) });
                    var button = new Button();
                    button.Text = string.Format("{0}{1}", columnName, i);
                    button.Name = string.Format("button_{0}{1}", i, j);
                    button.Dock = DockStyle.Fill;
                    button.Enabled = true;
                    this.tableLayoutPanel2.Controls.Add(button, j, i);
                    button.Click += buttonClicked;
                    enemyTableButtons.Add(button);
                }
            }
        }

        private Boolean isButtonOccupied(Button buttonToCheck, List<Button> buttonsList, List<Ship> ships)
        {
            Boolean result = false;
            foreach (Ship ship in ships)
            {
                foreach (CoordsXY coords in ship.coords)
                {
                    String buttonName = getButtonNameFromCoords(coords);
                    Button button = getButtonFromText(buttonsList, buttonName);
                    if(buttonToCheck.Text == button.Text)
                    {
                        return true;
                    }
                }
            }
            return result;
        }

        private Boolean checkForWin()
        {
            Boolean result = true;
            foreach (Ship ship in computerShips)
            {
                foreach (CoordsXY coords in ship.coords)
                {
                    String buttonName = getButtonNameFromCoords(coords);
                    Button button = getButtonFromText(enemyTableButtons, buttonName);
                    //one not destroyed ship coord is enough to not win 
                    if(button.BackColor != Color.Red)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private Boolean checkForLose()
        {
            Boolean result = true;
            foreach (Ship ship in playerShips)
            {
                foreach (CoordsXY coords in ship.coords)
                {
                    String buttonName = getButtonNameFromCoords(coords);
                    Button button = getButtonFromText(playerTableButtons, buttonName);
                    //one not destroyed ship coord is enough to not lose
                    if (button.BackColor != Color.Red)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private void buttonClicked(object sender, EventArgs e)
        {
            if (isTheGameStarted)
            {
                if(turn == 1)
                {
                    //check if the game is won/lose, set button color (match position shooted at), give turn to computer
                    //check if the game is won/lose
                    Button button = (sender as Button);
                    if (button.BackColor != Color.Gray && button.BackColor != Color.Red)
                    {
                        if (isButtonOccupied(button, enemyTableButtons, computerShips))
                        {
                            button.BackColor = Color.Red;
                        }
                        else
                        {
                            button.BackColor = Color.Gray;
                        }
                        ConsoleLogger.AppendText("Player made it's turn. Giving turn to computer if the game is not finished yet.");
                        ConsoleLogger.AppendText(Environment.NewLine);
                        if (!CheckForWinLoose())
                        {
                            turn = 0;
                            handleTurns(new Random());
                        }
                    }
                }
            }
        }

        private Boolean CheckForWinLoose()
        {
            Boolean result = false;
            bool isWin = checkForWin();
            bool isLose = checkForLose();
            if (isWin)
            {
                result = true;
                MessageBox.Show("You won! Please employ me.");
                ConsoleLogger.AppendText("Player has won the game.");
                ConsoleLogger.AppendText(Environment.NewLine);
                restartGameInitials();
            }
            if (isLose)
            {
                MessageBox.Show("You lost! But you can still win, by employing me.");
                ConsoleLogger.AppendText("Player has lost the game.");
                ConsoleLogger.AppendText(Environment.NewLine);
                restartGameInitials();
                result = true;
            }
            return result;
        }

        private void restartGameInitials()
        {
            //reinit list
            playerShips = new List<Ship>();
            computerShips = new List<Ship>();

            //reset some variables
            isTheGameStarted = false;
            shipToAttack = new Ship();
            //reset all the buttons to normal color
            resetButtonsColors(playerTableButtons);
            resetButtonsColors(enemyTableButtons);

            //set the game start button enabled again
            StartGameButton.Enabled = true;
            StartGameButton.Visible = true;

            ConsoleLogger.AppendText("Game re-initialized. You can play again.");
            ConsoleLogger.AppendText(Environment.NewLine);
        }

        private void resetButtonsColors(List<Button> buttons)
        {
            foreach(Button button in buttons)
            {
                button.BackColor = Color.White;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void switchButtons(List<Button> buttonsList, int onOff)
        {
            foreach(Button button in buttonsList)
            {
                if(onOff == 0)
                {
                    button.Enabled = false;
                }
                else
                {
                    button.Enabled=true;
                }
            }
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            StartGameButton.Visible = false;
            StartGameButton.Enabled = false;
            isTheGameStarted = true;
            PlaceShips();
            Random random = new Random();
            turn = random.Next(10) % 2; //50:50 computer starts or player starts
            //let's say 1 is our turn and computer's turn is 0
            handleTurns(random);
        }


        private void handleTurns(Random random)
        {
            //wait for click//allow for buttons clicking
            switchButtons(enemyTableButtons, turn);
            //if its computer's turn, do AI task and give turn to player
            if(turn == 0)
            {
                //do something, attack
                doComputerTurn(random);
                ConsoleLogger.AppendText("Computer made it's turn. Giving turn to player.");
                ConsoleLogger.AppendText(Environment.NewLine);
                if (!CheckForWinLoose())
                {
                    turn = 1;
                    handleTurns(random);
                }
            }
            if(turn == 1)
            {
                //do nothing, wait for switch from somewhere else and call
            }
        }

        Ship shipToAttack;
        private void doComputerTurn(Random random)
        {
            bool didAttack = false;
            while (!didAttack)
            {
                    int attackX = random.Next(10);
                    int attackY = random.Next(10);

                    CoordsXY coordToAttack = makeCoords(attackX, attackY);

                    Button buttonToAttack = null;

                    if (!isShipDestroyed(shipToAttack, playerTableButtons))
                    {
                        List<CoordsXY> nonDestroyedShipCoords = getNonDestroyedShipCoords(shipToAttack, playerTableButtons);
                        if(nonDestroyedShipCoords.Count > 0)
                        {   
                            coordToAttack = nonDestroyedShipCoords.First();
                            buttonToAttack = getButtonFromText(playerTableButtons, getButtonNameFromCoords(coordToAttack));
                        }
                        else
                        {
                            shipToAttack = new Ship();
                        }
                    }

                    if(buttonToAttack == null)
                    {
                       buttonToAttack = getButtonFromText(playerTableButtons, getButtonNameFromCoords(coordToAttack));
                    }

                    if (buttonToAttack != null)
                    {
                        if(buttonToAttack.BackColor != Color.Red && buttonToAttack.BackColor != Color.Gray)
                        {
                            //attack
                            if (isButtonOccupied(buttonToAttack, playerTableButtons, playerShips))
                            {
                                buttonToAttack.BackColor = Color.Red;
                                this.shipToAttack = getShipFromCoords(coordToAttack, playerShips);
                            }
                            else
                            {
                                buttonToAttack.BackColor = Color.Gray;
                            }
                            didAttack = true;
                        }
                        else
                        {
                            didAttack = false;
                        }
                    }
            }
        }

        private Ship getShipFromCoords(CoordsXY coords, List<Ship> ships)
        {
            Ship result = new Ship();
            if (ships.Count > 0)
            {
                foreach (Ship ship in ships)
                {
                    List<CoordsXY> shipCoords = ship.coords;
                    foreach(CoordsXY shipCoord in shipCoords)
                    {
                        if(shipCoord.X == coords.X && shipCoord.Y == coords.Y)
                        {
                            return ship;
                        }
                    }
                }
            }

            return result;
        }

        private Boolean isShipDestroyed(Ship ship, List<Button> buttons)
        {
            Boolean result = true;
            if (ship.coords != null)
            {
                if (ship.coords.Count > 0)
                {
                    foreach (CoordsXY coords in ship.coords)
                    {
                        Button button = getButtonFromText(buttons,getButtonNameFromCoords(coords));
                        if(button != null)
                        {
                            if (button.BackColor != Color.Red)
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private List<CoordsXY> getDestroyedShipCoords(Ship ship, List<Button> buttons)
        {
            List<CoordsXY> result = new List<CoordsXY>();
            if (ship.coords.Count > 0)
            {
                foreach (CoordsXY coords in ship.coords)
                {
                    Button button = getButtonFromText(buttons, getButtonNameFromCoords(coords));
                    if (button.BackColor == Color.Red)
                    {
                        result.Add(coords);
                    }
                }
            }
            return result;
        }

        private List<CoordsXY> getNonDestroyedShipCoords(Ship ship, List<Button> buttons)
        {
            List<CoordsXY> result = new List<CoordsXY>();
            if(ship.coords.Count > 0)
            {
                foreach(CoordsXY coords in ship.coords)
                {
                    Button button = getButtonFromText(buttons, getButtonNameFromCoords(coords));
                    if(button.BackColor != Color.Red)
                    {
                        result.Add(coords);
                    }
                }
            }
            return result;
        }

        private Button getButtonFromText(List<Button> buttonsList, String buttonText)
        {
            Button result = null;
            for(int x = 0; x < buttonsList.Count; x++)
            {
                Button tmp = buttonsList[x];
                if (tmp.Text.Contains(buttonText))
                {
                    result = tmp;
                    break;
                }
            }
            return result;
        }


        private String getButtonNameFromCoords(CoordsXY coords)
        {
            String result = "";
            //column - letter
            int x = coords.X;
            //row - number
            int y = coords.Y;
            //convert column number to letter
            String letter = "";
            switch (x)
            {
                case 0: letter = "A"; break;
                case 1: letter = "B"; break;
                case 2: letter = "C"; break;
                case 3: letter = "D"; break;
                case 4: letter = "E"; break;
                case 5: letter = "F"; break;
                case 6: letter = "G"; break;
                case 7: letter = "H"; break;
                case 8: letter = "I"; break;
                case 9: letter = "J"; break;
                default: letter = "null"; break;
            }
            if(letter == "null")
            {
                MessageBox.Show($"Something went wrong. Button letter returned null for x: {x}.");
            }
            result = $"{letter}{y}";
            return result;
        }

        private CoordsXY makeCoords(int x, int y)
        {
            CoordsXY result = new CoordsXY();
            result.X = x;
            result.Y = y;
            return result;
        }

        private Boolean canBePlacedHere(List<CoordsXY> occupiedCoords, List<CoordsXY> shipCoords)
        {
            //Checking if the new ship is not overlapping any of existing ship
            //Checking also if the new ship is not being placed exactly adjacent to one of already placed ships
            Boolean result = true;
            if(occupiedCoords != null && occupiedCoords.Count > 0){
                foreach (CoordsXY occupiedCoord in occupiedCoords)
                {
                    foreach (CoordsXY newShipCoord in shipCoords)
                    {
                        if (occupiedCoord.X == newShipCoord.X && occupiedCoord.Y == newShipCoord.Y)
                        {
                            return false;
                        }
                        if ( (occupiedCoord.X+1 == newShipCoord.X || occupiedCoord.X-1 == newShipCoord.X) && occupiedCoord.Y == newShipCoord.Y )
                        {
                            return false;
                        }
                        if ((occupiedCoord.Y + 1 == newShipCoord.Y || occupiedCoord.Y - 1 == newShipCoord.Y) && occupiedCoord.X == newShipCoord.X)
                        {
                            return false;
                        }
                    }
                }
            }
            return result;
        }

        private void placeShip(Random random, int shipLength, List<Ship> shipsList)
        {
            //get all occupied coords
            List<CoordsXY> occupiedCoords = new List<CoordsXY>();
            foreach(Ship ship in shipsList)
            {
                foreach(CoordsXY coords in ship.coords)
                {
                    occupiedCoords.Add(coords);
                }
            }

            //try to place a ship considering that all the choosen positions must be not occupied
            //just in case I will be stupid and forget about gridSize
            int gridSize = 9;
            if(shipLength < 0 || shipLength > gridSize)
            {
                return;
            }

            Boolean placed = false;
            //we will try until success
            while (!placed) {
                Ship newShip = new Ship();
                List<CoordsXY> coordsList = new List<CoordsXY>();

                //initial positions
                int x = random.Next(10);
                int y = random.Next(10);

                //let's randomly choose vertical or horizontal placement with 50:50 ratio
                if (random.Next(10) % 2 == 0)
                {
                    //try horizontal placement
                    //check if there's enough space in both horizontal directions to place the ship
                    if ( x > gridSize-shipLength)
                    {
                        //we have space to go into negative side
                        for (int x1 = 0; x1 < shipLength; x1++)
                        {
                            coordsList.Add(makeCoords(x - x1, y));
                        }
                        Boolean canPlaceHere = canBePlacedHere(occupiedCoords, coordsList);
                        if (canPlaceHere)
                        {
                            newShip.coords = coordsList;
                            shipsList.Add(newShip);
                            placed = true;
                        }
                        else
                        {
                            //clear the list
                            coordsList.Clear();
                        }
                    }
                    else
                    {
                        //we have space to go into positive side
                        for (int x1 = 0; x1 < shipLength; x1++)
                        {
                            coordsList.Add(makeCoords(x + x1, y));
                        }
                        Boolean canPlaceHere = canBePlacedHere(occupiedCoords, coordsList);
                        if (canPlaceHere)
                        {
                            newShip.coords = coordsList;
                            shipsList.Add(newShip);
                            placed = true;
                        }
                        else
                        {
                            //clear the list
                            coordsList.Clear();
                        }
                    }
                }
                //ship was not placed but we can still try vertical placement
                if (!placed)
                {
                    //try vertical placement
                    //check if there's enough space in both vertical directions to place the ship
                    if (y > gridSize - shipLength)
                    {
                        //we have space to go into negative side
                        for (int y1 = 0; y1 < shipLength; y1++)
                        {
                            coordsList.Add(makeCoords(x, y - y1));
                        }
                        Boolean canPlaceHere = canBePlacedHere(occupiedCoords, coordsList);
                        if (canPlaceHere)
                        {
                            newShip.coords = coordsList;
                            shipsList.Add(newShip);
                            placed = true;
                        }
                        else
                        {
                            //clear the list
                            coordsList.Clear();
                        }
                    }
                    else
                    {
                        //we have space to go into positive side
                        for (int y1 = 0; y1 < shipLength; y1++)
                        {
                            coordsList.Add(makeCoords(x, y + y1));
                        }
                        Boolean canPlaceHere = canBePlacedHere(occupiedCoords, coordsList);
                        if (canPlaceHere)
                        {
                            newShip.coords = coordsList;
                            shipsList.Add(newShip);
                            placed = true;
                        }
                        else
                        {
                            //clear the list
                            coordsList.Clear();
                        }
                    }
                }
            }
        }

        private void PlaceShips()
        {
            Random random = new Random();
            PlacePlayerShips(random);
            PlaceComputerShips(random);

        }

        private void PlaceComputerShips(Random random)
        {
            //place computer's ships
            //one battleship
            placeShip(random, 5, computerShips);
            //two destroyers
            placeShip(random, 4, computerShips);
            placeShip(random, 4, computerShips);


            //we don't want to cheat so we don't want to draw computer ships
            /*
                drawShips(computerShips, enemyTableButtons);
            */
            ConsoleLogger.AppendText("Computer Ships Placed.");
            ConsoleLogger.AppendText(Environment.NewLine);
        }

        private void PlacePlayerShips(Random random)
        {
            //place player's ships
            //one battleship
            placeShip(random, 5, playerShips);
            //two destroyers
            placeShip(random, 4, playerShips);
            placeShip(random, 4, playerShips);
            drawShips(playerShips, playerTableButtons);
            ConsoleLogger.AppendText("Player Ships Placed.");
            ConsoleLogger.AppendText(Environment.NewLine);

        }

        private void drawShips(List<Ship> shipsList, List<Button> buttonsTableList) {
            //draw all the ships that we created
            foreach (Ship ship in shipsList)
            {
                foreach (CoordsXY coords in ship.coords)
                {
                    String buttonName = getButtonNameFromCoords(coords);
                    Button button = getButtonFromText(buttonsTableList, buttonName);
                    button.BackColor = Color.Yellow;
                    button.ForeColor = Color.Black;
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
