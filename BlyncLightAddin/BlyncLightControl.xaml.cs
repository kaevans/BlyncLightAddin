//------------------------------------------------------------------------------
// <copyright file="BlyncLightControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace BlyncLightAddin
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Interaction logic for BlyncLightControl.
    /// </summary>
    public partial class BlyncLightControl : UserControl
    {
        IBlyncWatcher debugWatcher;
        IBlyncWatcher activeWindowWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlyncLightControl"/> class.
        /// </summary>
        public BlyncLightControl()
        {
            this.InitializeComponent();
            debugWatcher = new DebuggerWatcher();
            debugWatcher.StatusChanged += DebugWatcher_StatusChanged;
            debugWatcher.Initialize();

            activeWindowWatcher = new ActiveWindowWatcher();
            activeWindowWatcher.StatusChanged += ActiveWindowWatcher_StatusChanged;
            activeWindowWatcher.Initialize();
        }

        private void ActiveWindowWatcher_StatusChanged(object sender, EventArgs e)
        {
            var es = e as StatusChangedEventArgs;

            switch (es.Status)
            {
                case Status.Default:
                    ChangeColor(Blynclight.BlynclightController.Color.Yellow);
                    break;
                    
                case Status.Busy:
                    ChangeColor(Blynclight.BlynclightController.Color.Red);
                    break;
                case Status.Available:
                    ChangeColor(Blynclight.BlynclightController.Color.Green);
                    break;
                default:
                    break;
            }
        }

        private void DebugWatcher_StatusChanged(object sender, EventArgs e)
        {
            var es = e as StatusChangedEventArgs;

            switch (es.Status)
            {
                case Status.Default:
                    ChangeColor(Blynclight.BlynclightController.Color.Yellow);
                    break;

                case Status.Busy:
                    activeWindowWatcher.Pause();
                    ChangeColor(Blynclight.BlynclightController.Color.Red);
                    break;
                case Status.Available:
                    activeWindowWatcher.Resume();
                    ChangeColor(Blynclight.BlynclightController.Color.Green);
                    break;
                default:
                    break;
            }
        }

        ~BlyncLightControl()
        {
            debugWatcher.Close();
            debugWatcher.StatusChanged -= DebugWatcher_StatusChanged;
            activeWindowWatcher.Close();
            activeWindowWatcher.StatusChanged -= ActiveWindowWatcher_StatusChanged;
        }


        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(Blynclight.BlynclightController.Color.Blue);
        }

        private void ChangeColor(Blynclight.BlynclightController.Color color)
        {
            Blynclight.BlynclightController controller = new Blynclight.BlynclightController();
            int numDevices = 0;
            try
            {
                numDevices = controller.InitBlyncDevices();
                if (numDevices > 0)
                {
                    switch (color)
                    {
                        case Blynclight.BlynclightController.Color.Blue:
                            controller.TurnOnBlueLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Cyan:
                            controller.TurnOnCyanLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Green:
                            controller.TurnOnGreenLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Off:
                            controller.ResetLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Purple:
                            controller.TurnOnMagentaLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Red:
                            controller.TurnOnRedLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.White:
                            controller.TurnOnWhiteLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Yellow:
                            controller.TurnOnYellowLight(0);
                            break;
                        case Blynclight.BlynclightController.Color.Orange:
                            controller.TurnOnOrangeLight(0);
                            break;
                        default:
                            controller.ResetLight(0);
                            break;
                    }
                }
            }
            catch (Exception oops)
            {

            }
            finally
            {
                controller.CloseDevices(numDevices);
            }
        }
    }
}