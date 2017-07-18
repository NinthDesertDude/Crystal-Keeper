﻿using CrystalKeeper.Core;
using Microsoft.Win32;
using System.Windows;

namespace CrystalKeeper.Gui
{
    /// <summary>
    /// Wraps the functionality of a new project dialog.
    /// </summary>
    class DlgNewProject
    {
        #region Members
        /// <summary>
        /// Stores an instance of the new project gui.
        /// </summary>
        private DlgNewProjectGui gui;
        #endregion

        #region Constructors
        public DlgNewProject()
        {
            gui = new DlgNewProjectGui();
            gui.GuiNew.Click += GuiNew_Click;
            gui.GuiOpen.Click += GuiOpen_Click;
            gui.KeyDown += Gui_KeyDown;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Displays the gui.
        /// </summary>
        public void Show()
        {
            gui.Show();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Closes the gui if the user presses escape.
        /// </summary>
        private void CloseWindow(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                gui.Close();
            }
        }

        /// <summary>
        /// Responds to keyboard events for the gui.
        /// </summary>
        private void Gui_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl) ||
                    e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl))
            {
                if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.O))
                {
                    GuiOpen_Click(null, null);
                }
                if (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.N))
                {
                    GuiNew_Click(null, null);
                }
            }
        }

        /// <summary>
        /// Opens a project.
        /// </summary>
        private void GuiOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".mdat";
            dlg.Filter = "databases|*.mdat|all files|*.*";
            dlg.Title = "Load database";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                //Constructs the project from the file, then uses
                //it to construct the visuals.
                MainDisplay display = new MainDisplay(Project.Load(dlg.FileName), dlg.FileName);

                //Show the new display and close this one.
                display.Show();
                gui.Close();
            }
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        private void GuiNew_Click(object sender, RoutedEventArgs e)
        {
            MainDisplay display = new MainDisplay();

            //Show the new display and close this one.
            display.Show();
            gui.Close();
        }
        #endregion
    }
}