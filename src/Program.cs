﻿// <copyright file="Program.cs" company="N/A">
// Copyright 2011 Scott M. Lerch
// 
// This file is part of HostsFileEditor.
// 
// HostsFileEditor is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 2 of the License, or (at your option)
// any later version.
// 
// HostsFileEditor is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
// 
// You should have received a copy of the GNU General Public   License along
// with HostsFileEditor. If not, see http://www.gnu.org/licenses/.
// </copyright>

namespace HostsFileEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    using HostsFileEditor.Properties;

    /// <summary>
    /// The program class containing the main entry point.
    /// </summary>
    internal static class Program
    {
        #region Constants and Fields

        /// <summary>
        /// The application's main form.
        /// </summary>
        private static Form mainForm;

        #endregion

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Ensure only one copy of the application is running at a time
            bool createdNew;
            using (new Mutex(true, "HostsFileEditor", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.ThreadException += OnApplicationThreadException;

                    mainForm = new MainForm();
                    Application.Run(mainForm);
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The on application thread exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                mainForm, 
                e.Exception.Message, 
                Resources.ErrorCaption, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error, 
                MessageBoxDefaultButton.Button1, 
                MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// <summary>
        /// The native methods.
        /// </summary>
        private static class NativeMethods
        {
            /// <summary>
            /// The set foreground window.
            /// </summary>
            /// <param name="hWnd">
            /// The hardware instance.
            /// </param>
            /// <returns>
            /// True is successful, false otherwise.
            /// </returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }
    }
}