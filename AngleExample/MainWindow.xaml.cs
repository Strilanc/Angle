using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Angle;

namespace AngleExample {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Point _lastMousePosition;
        public MainWindow() {
            InitializeComponent();
            this.MouseMove += (sender, arg) => this._lastMousePosition = arg.GetPosition(this);
            MainLoop();
        }

        private async void MainLoop() {
            // --- constants ---
            var MaxTankTurnPerSec = Turn.OneTurnClockwise / 4;
            var MaxTurretTurnPerSec = Turn.OneTurnClockwise / 2;
            var MaxTurretTurn = Turn.OneDegreeClockwise * 60;
            var MaxTankMovementPerSec = 70.0;
            var StopRadius = Base.Width * 0.5 + Target.Width * 0.5;
            var TooCloseRadius = StopRadius / 2;
            
            // define a custom basis for translating facings to the appropriate render transform angles
            // render transforms are expected to be in degrees, with 0 being right and 90 being up
            var renderTransformBasis = Basis.FromDirectionAndUnits(
                Dir.AlongPositiveX, // angle 0 points right
                Basis.DegreesPerRotation, // 360 degrees in a full turn
                isClockwisePositive: false); // rotates counter-clockwise as angles increase

            // --- variables storing tank state ---
            var tankPos = new Point(this.Width / 2, this.Height / 2);
            var tankDir = Dir.AlongPositiveX;
            var turretTurn = Turn.Zero;

            var lastTick = Environment.TickCount;
            while (true) {
                // --- show current state ---
                var m = new Matrix();
                // position the target
                m.SetIdentity();
                m.OffsetX = _lastMousePosition.X - Target.ActualWidth / 2;
                m.OffsetY = _lastMousePosition.Y - Target.ActualHeight / 2;
                Target.RenderTransform = new MatrixTransform(m);
                // position and rotate the tank control
                m.SetIdentity();
                m.Rotate(tankDir.GetSignedAngle(renderTransformBasis));
                m.OffsetX = tankPos.X - Base.ActualWidth / 2;
                m.OffsetY = tankPos.Y - Base.ActualHeight / 2;
                Base.RenderTransform = new MatrixTransform(m);
                // rotate the turret image (inside the tank control)
                m.SetIdentity();
                m.Rotate(turretTurn.GetAngle(renderTransformBasis));
                Turret.RenderTransform = new MatrixTransform(m);

                // --- wait a bit before updating ---
                await Task.Delay(TimeSpan.FromMilliseconds(30));
                TimeSpan dt;
                var tick = Environment.TickCount;
                unchecked { dt = TimeSpan.FromMilliseconds((uint)(tick - lastTick)); }
                lastTick = tick;

                // --- move the tank ---
                // measurements
                var dx = this._lastMousePosition.X - tankPos.X;
                var dy = this._lastMousePosition.Y - tankPos.Y;
                var dist = Math.Sqrt(dx * dx + dy * dy);
                var dirTowardsMouseFromTank = Dir.FromVector(dx, dy);

                // rotate tank towards target
                tankDir +=
                    // determine the turn necessary to rotate the tank to face the target
                    (dirTowardsMouseFromTank - tankDir) 
                    // force the turn to be within the allowed tank rotation rate
                    .ClampMagnitude(MaxTankTurnPerSec * dt.TotalSeconds); 

                // rotate turret towards target
                turretTurn += 
                    // determine the amount of turning necessary to rotate the turret to face the target
                    (dirTowardsMouseFromTank - tankDir - turretTurn)
                    // force the turn to be within the allowed turret rotation rate
                    .ClampMagnitude(MaxTurretTurnPerSec * dt.TotalSeconds);
                
                // force turret to stay within its allowed turning radius
                turretTurn = turretTurn.ClampMagnitude(MaxTurretTurn);

                // move tank forwards or backwards
                var speedSign = dist > StopRadius ? +1 // forwards when far away
                              : dist < TooCloseRadius ? -1 // backwards when too close
                              : 0; // sit still when near
                var displacement = speedSign*MaxTankMovementPerSec*dt.TotalSeconds;
                tankPos.X += tankDir.UnitX * displacement;
                tankPos.Y += tankDir.UnitY * displacement;
            }
        }
    }
    internal static class ComparableUtil {
        public static T MinBy<T>(this T value1, T value2, IComparer<T> comparer) {
            return comparer.Compare(value1, value2) <= 0 ? value1 : value2;
        }
    }
}
