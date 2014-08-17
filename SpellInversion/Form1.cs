using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellInversion {
	public partial class Form1 : Form {
		#region Variables
		/// <summary>
		/// Random Number Generator
		/// </summary>
		public Random rng = new Random();
		/// <summary>
		/// fx,fy is the size of the form; fx2,fy2 is half that;
		/// so that (fx2, fy2) is the center of the form
		/// </summary>
		public int fx, fy, fx2, fy2;
		/// <summary>
		/// the canvas that holds the back buffer
		/// </summary>
		private Bitmap gi;
		/// <summary>
		/// The graphics for the back buffer that draws on gi
		/// </summary>
		private Graphics gb;
		/// <summary>
		/// The graphics for the front buffer that draws on the form
		/// </summary>
		private Graphics gf;
		/// <summary>
		/// The timer object for setting the heartbeat
		/// </summary>
		private Timer tim = new Timer() { Interval = 1000 / 180 };
		/// <summary>
		/// The start time for frame timings
		/// </summary>
		private DateTime st;
		/// <summary>
		/// The finish time for frame timings
		/// </summary>
		private TimeSpan ft;
		/// <summary>
		/// Total number of frames elapsed since start
		/// </summary>
		private static ulong _tfr = 0; public static ulong tfr { get { return _tfr; } }

		private static bool[] opt = new bool[10];

		#endregion Variables
		#region Events
		public Form1() { InitializeComponent(); }
		private void Form1_Load(object sender, EventArgs e) {
			Width = 1600; Left = 0;
			fx = Width; fy = Height; fx2 = fx / 2; fy2 = fy / 2;
			gi = new Bitmap(fx, fy); gb = Graphics.FromImage(gi);
			gf = CreateGraphics(); tim.Tick += tim_Tick;

			for(int q = 0 ; q < 10 ; q++) opt[q] = true;

			tim.Start();
			//Calc(); Draw();
		}
		private void Form1_KeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Escape: Close(); return;
				case Keys.Space: tim.Enabled = !tim.Enabled; break;
				case Keys.Left: _tfr--; Calc(); Draw(); break;
				case Keys.Right: _tfr++; Calc(); Draw(); break;
				case Keys.Oemtilde: for(int q = 0 ; q < 10 ; q++) opt[q] = false; break;

				default:
					int n = -1;
					if(e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9) n = e.KeyCode - Keys.D1;
					if(e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad9) n = e.KeyCode - Keys.NumPad1;
					if(e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0) n = 9;
					if(n > -1) opt[n] = !opt[n];					
					break;
			}
		}
		private void Form1_MouseClick(object sender, MouseEventArgs e) {

		}
		private MouseEventArgs mouse;
		private void Form1_MouseMove(object sender, MouseEventArgs e) {
			mouse = e;
		}
		private void Form1_Paint(object sender, PaintEventArgs e) { gf.DrawImage(gi, 0, 0); }
		void tim_Tick(object sender, EventArgs e) { Calc(); Draw(); _tfr++; }

		#endregion Events
		#region Calc
		public void Calc() {
			st = DateTime.Now;
			gb.Clear(Color.Black);


		}

		#endregion Calc
		#region Draw
		public Pen[] p = { Pens.Red, Pens.Orange, Pens.Yellow, Pens.Green, Pens.Blue, Pens.Purple, Pens.White, Pens.White, Pens.White, Pens.White };
		public Pen[] tp = { Pens.Red, Pens.Orange, Pens.Yellow, Pens.Green, Pens.SkyBlue, Pens.Violet, Pens.White, Pens.White, Pens.White, Pens.Gray };
		public void Draw() {
			int m = p.Length, o = (int)_tfr % fx;
			double[] a  = new double[m]; double[] oa = new double[m];
			double[] b  = new double[m]; double[] ob = new double[m];
			double[] f  = new double[m]; double[] of = new double[m];
			double[] g  = new double[m]; double[] og = new double[m];
			gb.DrawLine(Pens.Gray, 0, fy2, fx, fy2); m--;
			double amp = 150.0, wl = Math.Pow(2.0, m) / fx, t = 0.0, ot = 0.0;
			for(int q = 0 ; q < fx ; q++) 
			{//int q = Math.Min((int)_tfr, fx);
				ot = t; t = 0.0;
				for(int w = 0 ; w <= m ; w++) { of[w] = f[w]; f[w] = 0.0; og[w] = g[w]; g[w] = 0.0; }
				for(int w = 0 ; w <= (q < fx2 ? m : m-1) ; w++) {
					oa[w] = a[w]; ob[w] = b[w];
					a[w] = amp / Math.Pow(2, w) * Math.Sin(Math.PI * 2.0 * (wl / Math.Pow(2, m - w)) * ((q + (int)_tfr) % fx));
					b[w] = amp / Math.Pow(2, w) * Math.Sin(Math.PI * 2.0 * (wl / Math.Pow(2, m - w)) * ((fx - q + (int)_tfr) % fx));
					t += a[w] + b[w]; for(int ww = w ; ww <= m ; ww++) { f[ww] += a[w]; g[ww] += b[w]; }
					//if(w == w && Math.Abs(a[w]) > amp - 0.1) gb.DrawLine(p[w], q, 0, q, fy);
					if(opt[w]) {
						gb.DrawLine(p[w], q - 1, (int)(fy2 + oa[w]), q, (int)(fy2 + a[w]));
						gb.DrawLine(p[w], q - 1, (int)(fy2 + ob[w]), q, (int)(fy2 + b[w]));
					}
				}
				for(int w = m ; w <= (q < fx2 ? m : m - 0) ; w++) {
					gb.DrawLine(tp[w], q - 1, (int)(fy2 + of[w]), q - 0, (int)(fy2 + f[w]));
					gb.DrawLine(tp[w], q - 1, (int)(fy2 + og[w]), q - 0, (int)(fy2 + g[w]));
				}
				gb.DrawLine(Pens.White, q - 1, (int)(fy2 + ot), q - 0, (int)(fy2 + t));
				//gb.DrawLine(Pens.Gray, q - 1, (int)(fy2 + ot - oa[0]), q - 0, (int)(fy2 + t - a[0]));


			}




			ft = DateTime.Now - st;
			gb.DrawString(ft.TotalMilliseconds.ToString() + "ms", Font, Brushes.White, 0, 0);
			gb.DrawString((1000 / ft.TotalMilliseconds).ToString() + " FPS", Font, Brushes.White, 0, 16);
			gb.DrawString("(" + mouse.X.ToString() + ", " + (fy2-mouse.Y).ToString() + ") Mouse", Font, Brushes.White, 0, 32);
			gf.DrawImage(gi, 0, 0);
		}

		#endregion Draw
		#region Methods

		#endregion Methods

	}
}
