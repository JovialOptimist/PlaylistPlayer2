// music player

using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using WMPLib;
using System.Windows.Forms;
using static System.Console;

namespace PlaylistPlayer2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*****************************************************
             * Part 1: Set Lists and Variables
             * - Set cover names for options list
             * - Set file paths for corresponding options
             ****************************************************/
            bool repeat;
            do
            {
                // Visible Options
                repeat = false;
                List<string> optionsList = new List<string>();
                Console.Title = "Jac's Music Player - Volume: 100";
                optionsList.Add("Chill Mellow");
                optionsList.Add("Chill Upbeat");
                optionsList.Add("Battle");
                optionsList.Add("Intense Battle");
                optionsList.Add("AAAHHHHHHHH");
                optionsList.Add("Special Song");
                optionsList.Add("No Preference");
                AddNumbersToList(optionsList);

                // File Paths
                List<string> urlsList = new List<string>();
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Mellow Quiet\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Mellow Upbeat\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Regular Battle\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Intense Battle!\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Intense Battle!\ABSOLUTE FINAL BOSS DEATH MUSIC\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Special Songs (Situational)\");
                urlsList.Add(@"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\");


                /*****************************************************
                 * Part 2: Option Display and Reporting
                 * - Display list
                 * - Set the precursor file path (fileStartGibberish)
                 * - Set the possible filePathsList to choose from
                 ****************************************************/

                // Choose
                DisplayOptions(optionsList);
                int userinput = ChooseItemFromList(optionsList);
                string playlistName = optionsList[userinput];
                ResetScreen();

                // Variable Pathing
                string fileStartGibberish = urlsList[userinput];
                string[] filePathsArray = Directory.GetFiles(fileStartGibberish, "*.mp3", SearchOption.AllDirectories);
                List<string> filePathsList = new List<string>(filePathsArray);


                /*****************************************************
                 * Part 3: Declare Playlists
                 * - Declare Playlists
                 * - Start the first song
                 ****************************************************/

                // Initialize Variables
                WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
                IWMPPlaylist masterPlaylist = wplayer.playlistCollection.newPlaylist("Master Playlist");
                int numOfSongs = filePathsArray.Length;


                /*****************************************************
                 * Part 4: Initialize Playlist and Placeholder Song
                 * - Add songs to the playlist (with a buffer)
                 * - Check for key to skip a song
                 * - Continue first song until it's over or skipped
                 * **************************************************/
                Random rand = new Random();
                for (int i = 0; i < numOfSongs; i++)
                {
                    // Add songs randomly
                    string randomListItem = filePathsList[rand.Next(0, filePathsList.Count)];
                    IWMPMedia songToLoad = wplayer.newMedia(randomListItem);
                    masterPlaylist.appendItem(@songToLoad);
                    filePathsList.Remove(randomListItem);
                }


                /*****************************************************
                 * Part 5: Play Playlist
                 * - Sets variables
                 * - Continually crosschecks current media name with
                 *   the registered name to automatically continue 
                 *   playing the next song in queue
                 * - Skip via any keypress
                 * **************************************************/

                wplayer.currentPlaylist = masterPlaylist;
                CursorVisible = false;
                string currentName = wplayer.currentMedia.name;
                int playlistCount = wplayer.currentPlaylist.count;
                wplayer.controls.next();
                WriteDisplayName(GetDisplayName(wplayer.currentMedia.name));

                int volume = 100;
                while (true)
                {
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.KeyChar == '.')
                    {
                        wplayer.controls.next();
                        WriteDisplayName(wplayer.currentMedia.name);
                    }
                    else if (key.KeyChar == ',')
                    {
                        wplayer.controls.previous();
                        WriteDisplayName(wplayer.currentMedia.name);
                    }
                    else if (key.KeyChar == '`')
                    {
                        repeat = true;
                        wplayer.controls.stop();
                        Console.Clear();
                        break;
                    }
                    else if (key.KeyChar == ' ')
                    {
                        if (wplayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            wplayer.controls.pause();
                        }
                        else
                        {
                            wplayer.controls.play();
                        }
                    }
                    else if (key.KeyChar == '-')
                    {
                        if (volume >= 30)
                        {
                            volume -= 10;
                        }
                        else
                        {
                            volume -= 2;
                        }
                        
                        wplayer.settings.volume = volume;
                    }
                    else if (key.KeyChar == '=')
                    {
                        if (volume >= 20)
                        {
                            volume += 10;
                        }
                        else
                        {
                            volume += 2;
                        }
                        
                        wplayer.settings.volume = volume;
                    }
                    else if (key.KeyChar == '7')
                    {
                        //wplayer.launchURL("https://www.youtube.com/watch?v=OLG5FesdDmE&ab_channel=VideoGameMusicTube");
                        
                        wplayer.controls.stop();
                        IWMPMedia newMedia = wplayer.newMedia("https://www.youtube.com/watch?v=OLG5FesdDmE&ab_channel=VideoGameMusicTube");
                        wplayer.controls.playItem(newMedia);
                    }
                    Console.Title = "Jac's Music Player - Volume: " + volume;
                }
            } while (repeat);
        }
        private static void ResetScreen()
        {
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
            int ogCursorY = CursorTop;
            int ogCursorX = CursorLeft;
            Clear();
            SetCursorPosition(ogCursorY, ogCursorX);
        }
        public static string GetDisplayName(string randomListItem)
        {
            string displayName;
            char[] charsToTrim1 = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', ' ', '.', '?' };
            displayName = randomListItem.Substring(randomListItem.LastIndexOf(@"\") + 1);

            // All other songs:
            if (!(displayName.Contains("a.m.") || displayName.Contains("p.m.")))
            {
                displayName = displayName.Trim(charsToTrim1);
            }
            else
            {
                displayName = displayName.Substring(displayName.LastIndexOf("00"));
            }
            return displayName;
        }
        public static void WriteDisplayName(string displayName, string playlistName)
        {
            ClearLine();
            playlistName = playlistName.Remove(0, 3);
            try
            {
                SetCursorPosition((WindowWidth / 2) - (playlistName.Length) / 2, (WindowHeight / 2) - 1);
                Write(playlistName);
                SetCursorPosition((WindowWidth / 2) - (displayName.Length) / 2, (WindowHeight / 2));
                Write(displayName);
            }
            catch (ArgumentOutOfRangeException)
            {
                SetCursorPosition((CursorLeft), (WindowHeight / 2));
            }
        }
        public static void WriteDisplayName(string displayName)
        {
            ClearLine();
            try
            {
                SetCursorPosition((WindowWidth / 2) - (displayName.Length) / 2, (WindowHeight / 2));
                Write(displayName);
            }
            catch (ArgumentOutOfRangeException)
            {
                SetCursorPosition((CursorLeft), (WindowHeight / 2));
            }
        }
        public static int ChooseItemFromList(List<string> optionsList)
        {
            PrepareFirstOption(optionsList);
            string userinput;
            do
            {
                // Some variables
                int currentCursorX = CursorLeft;
                int currentCursorY = CursorTop;
                BackgroundColor = ConsoleColor.Black;
                ForegroundColor = ConsoleColor.Black;


                /**********************************************************
                 * READ USER INPUT + REDRAW CURRENT LIST ITEM
                 * 1. Read user input as a character
                 * 2. If it's a number, return the number hotkey
                 * 3. If it's not S or W, return the current selected item
                 * 4. Redraw the current list item to be unselected
                 * *******************************************************/
                ConsoleKeyInfo userinputKEY = ReadKey();
                userinput = Convert.ToString(userinputKEY.KeyChar);
                if (int.TryParse(userinput, out int fakeuserinput))
                {
                    SetCursorPosition(0, fakeuserinput);
                    return CursorTop - 1;
                }
                if (!(userinput == "s") && !(userinput == "w"))
                {
                    return CursorTop - 1;
                }

                BackgroundColor = ConsoleColor.Black;
                ForegroundColor = ConsoleColor.White;
                Write(optionsList[CursorTop - 1]);
                SetCursorPosition(currentCursorX, currentCursorY);
                BackgroundColor = ConsoleColor.Blue;


                /********************************************************************
                 * MOVEMENT AND DUMMY CHECK
                 * If the user input is "W" and it isn't too high up : go up
                 * If the user input is "S" and it isn't too low : go down
                 * If the user input is "W" or "S" and it IS too high or low : redraw
                 * *****************************************************************/
                int cursorYMutator = 0;
                int lengthOfList = optionsList.Count;
                if (userinput == "w")
                {
                    if (CursorTop > 1)
                    {
                        cursorYMutator = currentCursorY - 1;
                    }
                    else
                    {
                        cursorYMutator = lengthOfList;
                    }
                }
                if (userinput == "s")
                {
                    if (CursorTop < lengthOfList)
                    {
                        cursorYMutator = currentCursorY + 1;
                    }
                    else
                    {
                        cursorYMutator = 1;
                    }
                }

                Console.SetCursorPosition(currentCursorX, cursorYMutator);
                Write(optionsList[CursorTop - 1]);
                BackgroundColor = ConsoleColor.Black;
                Write(" \n");
                Console.SetCursorPosition(currentCursorX, cursorYMutator);
            } while (userinput == "s" || userinput == "w");
            return CursorTop - 1;
        }
        private static void PrepareFirstOption(List<string> optionsList)
        {
            Console.SetCursorPosition(0, 1);
            BackgroundColor = ConsoleColor.Blue;
            Write(optionsList[0]);
            BackgroundColor = ConsoleColor.Black;
            Write(" \n");
            Console.SetCursorPosition(0, 1);
            Console.CursorVisible = false;
        }
        public static List<string> DisplayOptions(List<string> optionsList)
        {
            Console.SetWindowSize(50, 15);
            SetBufferSize(50, 15);
            WriteLine("Pick yer playlist! (WASD)");
            foreach (string option in optionsList)
            {
                WriteLine(" " + option);
            }
            SetCursorPosition(0, 1);
            return optionsList;
        }
        private static List<string> AddNumbersToList(List<string> optionsList)
        {
            for (int i = 0; i < optionsList.Count; i++)
            {
                optionsList[i] = (i + 1) + ". " + optionsList[i];
            }
            return optionsList;
        }
        private static void UpdateProgressBar(int i, int total)
        {
            // Loading Songs
            SetCursorPosition((WindowWidth / 2) - (14 + 2) / 2, (WindowHeight / 2));
            Write("Loading Songs:");

            // Clean Progress Bar
            string progressBar;
            string marker = "<";
            string behind = new string('-', (i * 10 / total));
            string inFront = new string('=', 9 - i * 10 / total);
            progressBar = "|" + behind + marker + inFront + "|";
            SetCursorPosition((WindowWidth / 2) - (progressBar.Length + 2) / 2, (WindowHeight / 2) + 1);
            Write(progressBar);
        }
        private static void ClearLine()
        {
            Console.SetCursorPosition(0, CursorTop);
            WriteLine(new string(' ', WindowWidth));
        }
    }
}
